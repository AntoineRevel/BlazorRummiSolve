using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Genetic;

/// <summary>
///     Algorithme génétique parallèle pour résoudre le Rummikub
///     Utilise plusieurs populations évoluant en parallèle avec migration entre elles
/// </summary>
public class ParallelGeneticSolver : ISolver
{
    private const int MinScore = 30;
    private readonly int _boardJokers;
    private readonly Tile[] _boardTiles;
    private readonly GeneticConfiguration _config;
    private readonly ConcurrentDictionary<string, double> _fitnessCache = new();
    private readonly bool _isFirstPlay;
    private readonly int _playerJokers;
    private readonly Tile[] _playerTiles;
    private readonly Random _random = new();

    private ParallelGeneticSolver(
        List<Tile> boardTiles,
        int boardJokers,
        List<Tile> playerTiles,
        bool isFirstPlay,
        GeneticConfiguration? config = null)
    {
        _boardTiles = boardTiles.ToArray();
        _boardJokers = boardJokers;
        _isFirstPlay = isFirstPlay;

        var playerJokerCount = playerTiles.Count(t => t.IsJoker);
        _playerJokers = playerJokerCount;

        _playerTiles = playerTiles
            .Where(t => !t.IsJoker)
            .ToArray();

        _config = config ?? GeneticConfiguration.Default;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            var bestSolution = RunGeneticAlgorithm(cancellationToken);

            if (bestSolution != null)
            {
                // Récupère uniquement les tuiles du joueur effectivement utilisées dans la solution
                var usedPlayerTiles = GetUsedPlayerTiles(bestSolution);
                var usedPlayerJokers = CountUsedPlayerJokers(bestSolution);

                // Vérifie que le joueur a effectivement joué au moins une tuile
                // Une solution qui ne joue rien n'est pas une vraie solution
                if (usedPlayerTiles.Any() || usedPlayerJokers > 0)
                    return SolverResult.FromSolution(
                        GetType().Name,
                        bestSolution,
                        usedPlayerTiles,
                        usedPlayerJokers,
                        usedPlayerTiles.Count() == _playerTiles.Length && usedPlayerJokers == _playerJokers,
                        usedPlayerTiles.Sum(t => t.Value)
                    );
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout atteint
        }
        finally
        {
            sw.Stop();
            if (_config.EnableLogging)
            {
                Console.WriteLine($"[{GetType().Name}] Execution time: {sw.ElapsedMilliseconds}ms");
                Console.WriteLine($"[{GetType().Name}] Cache hits: {_fitnessCache.Count}");
            }
        }

        return SolverResult.Invalid(GetType().Name);
    }

    private Solution? RunGeneticAlgorithm(CancellationToken cancellationToken)
    {
        var migrationChannel = Channel.CreateUnbounded<Individual>();
        var bestSolutionChannel = Channel.CreateUnbounded<Solution>();

        var tasks = new Task[_config.PopulationCount];

        // Lance les populations parallèles
        for (var i = 0; i < _config.PopulationCount; i++)
        {
            var populationId = i;
            tasks[i] = Task.Run(() =>
                    EvolvePopulation(
                        populationId,
                        migrationChannel,
                        bestSolutionChannel,
                        cancellationToken),
                cancellationToken);
        }

        // Collecte la meilleure solution trouvée par toutes les populations
        Solution? globalBest = null;
        double globalBestFitness = 0;

        var collectorTask = Task.Run(async () =>
        {
            await foreach (var solution in bestSolutionChannel.Reader.ReadAllAsync(cancellationToken))
            {
                var fitness = EvaluateFitness(solution);
                if (fitness > globalBestFitness)
                {
                    globalBestFitness = fitness;
                    globalBest = solution;

                    // Si on a trouvé une solution parfaite, on arrête
                    if (fitness >= _config.PerfectFitnessThreshold) break;
                }
            }
        }, cancellationToken);

        try
        {
            Task.WaitAll(tasks, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Normal si timeout
        }

        migrationChannel.Writer.TryComplete();
        bestSolutionChannel.Writer.TryComplete();

        try
        {
            collectorTask.Wait(TimeSpan.FromSeconds(1));
        }
        catch (OperationCanceledException)
        {
            // Normal si cancelled
        }
        catch (AggregateException)
        {
            // Normal si cancelled
        }

        return globalBest;
    }

    private async Task EvolvePopulation(
        int populationId,
        Channel<Individual> migrationChannel,
        Channel<Solution> bestSolutionChannel,
        CancellationToken cancellationToken)
    {
        var population = InitializePopulation();
        var generation = 0;
        var stagnationCount = 0;
        double previousBestFitness = 0;

        while (generation < _config.MaxGenerations && !cancellationToken.IsCancellationRequested)
        {
            // Évalue la population
            EvaluatePopulation(population);

            // Trouve le meilleur individu
            var currentBest = population.MaxBy(ind => ind.Fitness)!;

            // Envoie la meilleure solution au collecteur
            if (currentBest.Solution.IsValid)
                await bestSolutionChannel.Writer.WriteAsync(currentBest.Solution, cancellationToken);

            // Vérifie la stagnation
            if (Math.Abs(currentBest.Fitness - previousBestFitness) < 0.001)
            {
                stagnationCount++;
            }
            else
            {
                stagnationCount = 0;
                previousBestFitness = currentBest.Fitness;
            }

            // Migration entre populations
            if (generation % _config.MigrationInterval == 0)
                await PerformMigration(population, migrationChannel, cancellationToken);

            // Nouvelle génération
            population = CreateNextGeneration(population, stagnationCount > _config.StagnationThreshold);

            generation++;
        }
    }

    private List<Individual> InitializePopulation()
    {
        var population = new List<Individual>(_config.PopulationSize);

        for (var i = 0; i < _config.PopulationSize; i++) population.Add(CreateRandomIndividual());

        return population;
    }

    private Individual CreateRandomIndividual()
    {
        var allTiles = _boardTiles.Concat(_playerTiles).OrderBy(t => t.Color).ThenBy(t => t.Value).ToArray();
        var totalJokers = _boardJokers + _playerJokers;

        var solver = new InternalSolver(allTiles, totalJokers);
        var solution = solver.GenerateRandomSolution(_random);

        return new Individual { Solution = solution, Fitness = 0 };
    }

    private void EvaluatePopulation(List<Individual> population)
    {
        Parallel.ForEach(population, individual => { individual.Fitness = EvaluateFitness(individual.Solution); });
    }

    private double EvaluateFitness(Solution solution)
    {
        var key = GetSolutionKey(solution);

        return _fitnessCache.GetOrAdd(key, _ =>
        {
            double fitness = 0;

            if (!solution.IsValid)
                return 0;

            // CRITIQUE: Vérifie que TOUTES les tuiles du plateau sont présentes dans la solution
            // Sans cette vérification, des tuiles du plateau peuvent disparaître!
            if (!AllBoardTilesPresent(solution))
                // Pénalité MASSIVE si des tuiles du plateau manquent
                return -100000;

            // Calcule le score des tuiles du joueur
            var usedPlayerTilesEnum = GetUsedPlayerTiles(solution);
            var usedPlayerTilesList = usedPlayerTilesEnum.ToList();
            var usedPlayerTilesCount = usedPlayerTilesList.Count;
            var playerScore = usedPlayerTilesList.Sum(t => t.Value);

            // Pour le premier coup, vérifie le score minimum de 30 points
            if (_isFirstPlay && playerScore < MinScore)
                // Pénalité très forte si le score est insuffisant pour le premier coup
                return -10000;

            // Score basé sur les tuiles du joueur utilisées
            fitness += usedPlayerTilesCount * 100;

            // Score total des combinaisons
            var totalScore = solution.GetSet().GetScore();
            fitness += totalScore * 10;

            // Bonus pour avoir vidé sa main
            if (usedPlayerTilesCount == _playerTiles.Length) fitness += 1000;

            // Qualité des combinaisons (préférer les longues séquences)
            fitness += solution.Runs.Sum(r => r.Tiles.Length * r.Tiles.Length);
            fitness += solution.Groups.Sum(g => g.Tiles.Length * 10);

            // Pénalité pour les jokers non utilisés
            var unusedJokers = CountUnusedJokers(solution);
            fitness -= unusedJokers * 20;

            return fitness;
        });
    }

    /// <summary>
    ///     Vérifie que toutes les tuiles du plateau sont présentes dans la solution
    /// </summary>
    private bool AllBoardTilesPresent(Solution solution)
    {
        // Collecte toutes les tuiles de la solution (hors jokers)
        var solutionTiles = new List<Tile>();

        foreach (var run in solution.Runs) solutionTiles.AddRange(run.Tiles.Where(t => !t.IsJoker));

        foreach (var group in solution.Groups) solutionTiles.AddRange(group.Tiles.Where(t => !t.IsJoker));

        // Vérifie que chaque tuile du plateau est dans la solution
        foreach (var boardTile in _boardTiles)
        {
            var index = solutionTiles.FindIndex(t =>
                t.Value == boardTile.Value && t.Color == boardTile.Color);

            if (index < 0)
                // Une tuile du plateau manque!
                return false;

            // Retire la tuile trouvée pour éviter les doubles comptages
            solutionTiles.RemoveAt(index);
        }

        // Vérifie aussi que les jokers du plateau sont présents
        var totalJokersInSolution = solution.Runs.Sum(r => r.Jokers) + solution.Groups.Sum(g => g.Jokers);
        if (totalJokersInSolution < _boardJokers)
            // Des jokers du plateau manquent
            return false;

        return true;
    }

    private int CountUsedPlayerTiles(Solution solution)
    {
        return GetUsedPlayerTiles(solution).Count();
    }

    private IEnumerable<Tile> GetUsedPlayerTiles(Solution solution)
    {
        var usedPlayerTiles = new List<Tile>();
        var remainingPlayerTiles = _playerTiles.ToList();

        // Collecte toutes les tuiles de la solution
        var solutionTiles = new List<Tile>();
        foreach (var run in solution.Runs)
            solutionTiles.AddRange(run.Tiles.Where(t => !t.IsJoker));
        foreach (var group in solution.Groups)
            solutionTiles.AddRange(group.Tiles.Where(t => !t.IsJoker));

        // D'abord, retire les tuiles du plateau de la solution
        // Cela évite de confondre les tuiles du plateau avec celles du joueur
        foreach (var boardTile in _boardTiles)
        {
            var index = solutionTiles.FindIndex(t => t.Value == boardTile.Value && t.Color == boardTile.Color);
            if (index >= 0) solutionTiles.RemoveAt(index);
        }

        // Ensuite, les tuiles restantes dans solutionTiles sont celles du joueur
        foreach (var tile in solutionTiles)
        {
            var index = remainingPlayerTiles.FindIndex(t => t.Value == tile.Value && t.Color == tile.Color);
            if (index >= 0)
            {
                usedPlayerTiles.Add(remainingPlayerTiles[index]);
                remainingPlayerTiles.RemoveAt(index);
            }
        }

        return usedPlayerTiles;
    }

    private int CountUsedPlayerJokers(Solution solution)
    {
        var totalJokersInSolution = solution.Runs.Sum(r => r.Jokers) + solution.Groups.Sum(g => g.Jokers);

        // On doit déterminer combien de jokers viennent du joueur vs du plateau
        // Si on a plus de jokers utilisés que de jokers sur le plateau, la différence vient du joueur
        var playerJokersUsed = Math.Max(0, totalJokersInSolution - _boardJokers);

        // On ne peut pas utiliser plus de jokers que le joueur n'en a
        return Math.Min(playerJokersUsed, _playerJokers);
    }

    private int CountUnusedJokers(Solution solution)
    {
        var usedJokers = solution.Runs.Sum(r => r.Jokers) + solution.Groups.Sum(g => g.Jokers);
        return _boardJokers + _playerJokers - usedJokers;
    }

    private string GetSolutionKey(Solution solution)
    {
        var runs = string.Join("|", solution.Runs.Select(r =>
            string.Join(",", r.Tiles.Select(t => t.GetHashCode()))));
        var groups = string.Join("|", solution.Groups.Select(g =>
            string.Join(",", g.Tiles.Select(t => t.GetHashCode()))));
        return $"{runs}#{groups}";
    }

    private List<Individual> CreateNextGeneration(List<Individual> population, bool increaseMutation)
    {
        var newPopulation = new List<Individual>(_config.PopulationSize);

        // Élitisme : garde les meilleurs individus
        var elite = population
            .OrderByDescending(ind => ind.Fitness)
            .Take(_config.EliteSize)
            .Select(ind => ind.Clone())
            .ToList();

        newPopulation.AddRange(elite);

        // Remplis le reste par crossover et mutation
        while (newPopulation.Count < _config.PopulationSize)
        {
            var parent1 = TournamentSelection(population);
            var parent2 = TournamentSelection(population);

            var child = Crossover(parent1, parent2);

            var mutationRate = increaseMutation ? _config.MutationRate * 2 : _config.MutationRate;
            if (_random.NextDouble() < mutationRate) child = Mutate(child);

            newPopulation.Add(child);
        }

        return newPopulation;
    }

    private Individual TournamentSelection(List<Individual> population)
    {
        var tournament = new List<Individual>();

        for (var i = 0; i < _config.TournamentSize; i++) tournament.Add(population[_random.Next(population.Count)]);

        return tournament.MaxBy(ind => ind.Fitness)!;
    }

    private Individual Crossover(Individual parent1, Individual parent2)
    {
        // Crossover uniforme : prend aléatoirement des runs/groups de chaque parent
        var childSolution = new Solution { IsValid = true };

        // Mélange les runs
        var allRuns = parent1.Solution.Runs.Concat(parent2.Solution.Runs).ToList();
        var selectedRuns = new HashSet<Run>();
        var usedTiles = new HashSet<Tile>();
        var usedJokers = 0;
        var totalAvailableJokers = _boardJokers + _playerJokers;

        foreach (var run in allRuns.OrderBy(_ => _random.Next()))
        {
            var canAdd = true;

            // Vérifie si on a assez de jokers disponibles
            if (usedJokers + run.Jokers > totalAvailableJokers) canAdd = false;

            // Vérifie les tuiles
            if (canAdd)
                foreach (var tile in run.Tiles.Where(t => !t.IsJoker))
                    if (usedTiles.Contains(tile))
                    {
                        canAdd = false;
                        break;
                    }

            if (canAdd)
            {
                selectedRuns.Add(run);
                usedJokers += run.Jokers;
                foreach (var tile in run.Tiles.Where(t => !t.IsJoker)) usedTiles.Add(tile);
            }
        }

        childSolution.Runs.AddRange(selectedRuns);

        // Mélange les groups
        var allGroups = parent1.Solution.Groups.Concat(parent2.Solution.Groups).ToList();
        var selectedGroups = new HashSet<Group>();

        foreach (var group in allGroups.OrderBy(_ => _random.Next()))
        {
            var canAdd = true;

            // Vérifie si on a assez de jokers disponibles
            if (usedJokers + group.Jokers > totalAvailableJokers) canAdd = false;

            // Vérifie les tuiles
            if (canAdd)
                foreach (var tile in group.Tiles.Where(t => !t.IsJoker))
                    if (usedTiles.Contains(tile))
                    {
                        canAdd = false;
                        break;
                    }

            if (canAdd)
            {
                selectedGroups.Add(group);
                usedJokers += group.Jokers;
                foreach (var tile in group.Tiles.Where(t => !t.IsJoker)) usedTiles.Add(tile);
            }
        }

        childSolution.Groups.AddRange(selectedGroups);

        return new Individual { Solution = childSolution, Fitness = 0 };
    }

    private Individual Mutate(Individual individual)
    {
        var mutated = individual.Clone();

        // Mutation : supprime aléatoirement une combinaison et essaye d'en créer de nouvelles
        if (_random.NextDouble() < 0.5 && mutated.Solution.Runs.Count > 0)
        {
            var indexToRemove = _random.Next(mutated.Solution.Runs.Count);
            mutated.Solution.Runs.RemoveAt(indexToRemove);
        }
        else if (mutated.Solution.Groups.Count > 0)
        {
            var indexToRemove = _random.Next(mutated.Solution.Groups.Count);
            mutated.Solution.Groups.RemoveAt(indexToRemove);
        }

        // Essaye d'ajouter de nouvelles combinaisons avec les tuiles libérées
        TryAddNewCombinations(mutated.Solution);

        return mutated;
    }

    private void TryAddNewCombinations(Solution solution)
    {
        var usedTiles = new HashSet<Tile>();

        foreach (var run in solution.Runs)
        foreach (var tile in run.Tiles.Where(t => !t.IsJoker))
            usedTiles.Add(tile);

        foreach (var group in solution.Groups)
        foreach (var tile in group.Tiles.Where(t => !t.IsJoker))
            usedTiles.Add(tile);

        var availableTiles = _boardTiles.Concat(_playerTiles)
            .Where(t => !usedTiles.Contains(t))
            .OrderBy(t => t.Color)
            .ThenBy(t => t.Value)
            .ToArray();

        if (availableTiles.Length < 3)
            return;

        // Calcule le nombre de jokers déjà utilisés dans la solution
        var usedJokers = solution.Runs.Sum(r => r.Jokers) + solution.Groups.Sum(g => g.Jokers);
        var totalAvailableJokers = _boardJokers + _playerJokers;
        var remainingJokers = Math.Max(0, totalAvailableJokers - usedJokers);

        var solver = new InternalSolver(availableTiles, remainingJokers);

        // Essaye de trouver des runs
        for (var i = 0; i < availableTiles.Length - 2; i++)
        {
            var runs = solver.FindRunsStartingAt(i);
            if (runs.Any())
            {
                solution.AddRun(runs.First());
                break;
            }
        }
    }

    private async Task PerformMigration(
        List<Individual> population,
        Channel<Individual> migrationChannel,
        CancellationToken cancellationToken)
    {
        // Envoie les meilleurs individus pour migration
        var best = population.OrderByDescending(ind => ind.Fitness).Take(2).ToList();

        foreach (var individual in best)
            await migrationChannel.Writer.WriteAsync(individual.Clone(), cancellationToken);

        // Reçoit des migrants d'autres populations
        var migrants = new List<Individual>();
        while (migrants.Count < 2 &&
               migrationChannel.Reader.TryRead(out var migrant))
            migrants.Add(migrant);

        // Remplace les pires individus par les migrants
        if (migrants.Any())
        {
            var worst = population.OrderBy(ind => ind.Fitness).Take(migrants.Count).ToList();
            foreach (var w in worst) population.Remove(w);

            population.AddRange(migrants);
        }
    }

    public static ParallelGeneticSolver Create(Set boardSet, Set playerSet, bool isFirstPlay = false,
        GeneticConfiguration? config = null)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new ParallelGeneticSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles, isFirstPlay, config);
    }

    // Classe interne pour générer des solutions
    private class InternalSolver : BaseSolver
    {
        public InternalSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
        {
        }

        public Solution GenerateRandomSolution(Random random)
        {
            var solution = new Solution { IsValid = true };
            var usedIndices = new bool[Tiles.Length];

            // Génère des runs aléatoires
            for (var i = 0; i < Tiles.Length; i++)
            {
                if (usedIndices[i]) continue;

                if (random.NextDouble() < 0.5) // 50% de chance d'essayer un run
                {
                    var runs = GetRuns(i).ToList();
                    if (runs.Any())
                    {
                        var selectedRun = runs[random.Next(runs.Count)];
                        solution.AddRun(selectedRun);
                        MarkTilesAsUsed(selectedRun, i - 1);
                        for (var j = i; j < Tiles.Length; j++) usedIndices[j] = UsedTiles[j];
                    }
                }
            }

            // Génère des groups aléatoires
            for (var i = 0; i < Tiles.Length; i++)
            {
                if (usedIndices[i]) continue;

                if (random.NextDouble() < 0.5) // 50% de chance d'essayer un group
                {
                    var groups = GetGroups(i).ToList();
                    if (groups.Any())
                    {
                        var selectedGroup = groups[random.Next(groups.Count)];
                        solution.AddGroup(selectedGroup);
                        MarkTilesAsUsed(selectedGroup, i - 1);
                        for (var j = i; j < Tiles.Length; j++) usedIndices[j] = UsedTiles[j];
                    }
                }
            }

            return solution;
        }

        public List<Run> FindRunsStartingAt(int index)
        {
            return GetRuns(index).ToList();
        }
    }

    // Classe pour représenter un individu
    private class Individual
    {
        public required Solution Solution { get; set; }
        public double Fitness { get; set; }

        public Individual Clone()
        {
            var clonedSolution = new Solution { IsValid = Solution.IsValid };
            clonedSolution.Runs.AddRange(Solution.Runs);
            clonedSolution.Groups.AddRange(Solution.Groups);
            return new Individual { Solution = clonedSolution, Fitness = Fitness };
        }
    }
}

// Configuration pour l'algorithme génétique
public record GeneticConfiguration
{
    public int PopulationSize { get; init; } = 100;
    public int PopulationCount { get; init; } = 4; // Nombre de populations parallèles
    public int MaxGenerations { get; init; } = 500;
    public int EliteSize { get; init; } = 10;
    public int TournamentSize { get; init; } = 5;
    public double MutationRate { get; init; } = 0.1;
    public int MigrationInterval { get; init; } = 20; // Tous les X générations
    public int StagnationThreshold { get; init; } = 10;
    public double PerfectFitnessThreshold { get; init; } = 10000;
    public bool EnableLogging { get; init; } = false;

    public static GeneticConfiguration Default => new();

    public static GeneticConfiguration Aggressive => new()
    {
        PopulationSize = 200,
        PopulationCount = 8,
        MaxGenerations = 1000,
        MutationRate = 0.2,
        EliteSize = 20
    };

    /// <summary>
    ///     Configuration ultra-agressive pour recherche exhaustive de la meilleure solution
    ///     Attention: beaucoup plus lent (10-30 secondes par coup)
    /// </summary>
    public static GeneticConfiguration UltraAggressive => new()
    {
        PopulationSize = 300,
        PopulationCount = 12,
        MaxGenerations = 2000,
        MutationRate = 0.25,
        EliteSize = 30,
        StagnationThreshold = 20
    };

    public static GeneticConfiguration Fast => new()
    {
        PopulationSize = 50,
        PopulationCount = 2,
        MaxGenerations = 100,
        MutationRate = 0.15
    };
}