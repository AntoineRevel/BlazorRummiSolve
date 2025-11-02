using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Genetic;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

/// <summary>
///     Stratégie utilisant l'algorithme génétique parallèle en compétition avec d'autres solvers
/// </summary>
public class GeneticSolverStrategy : ISolverStrategy
{
    private readonly bool _enableMeasurements;
    private readonly GeneticConfiguration? _geneticConfig;
    private readonly bool _useHybridMode;

    public GeneticSolverStrategy(
        GeneticConfiguration? geneticConfig = null,
        bool useHybridMode = true,
        bool enableMeasurements = false)
    {
        _geneticConfig = geneticConfig;
        _useHybridMode = useHybridMode;
        _enableMeasurements = enableMeasurements;
    }

    public async Task<SolverResult> GetSolverResult(
        Set board,
        Set rack,
        bool hasPlayed,
        CancellationToken externalToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // Crée le solver génétique
        var geneticSolver = hasPlayed
            ? ParallelGeneticSolver.Create(new Set(board), rack, false, _geneticConfig)
            : CreateFirstPlayGeneticSolver(rack);

        if (!_useHybridMode)
        {
            // Mode standalone : utilise uniquement le solver génétique
            var result = await Task.Run(() => geneticSolver.SearchSolution(externalToken), externalToken);

            stopwatch.Stop();
            if (_enableMeasurements)
            {
                Console.WriteLine($"[GeneticSolverStrategy] Standalone mode - {stopwatch.ElapsedMilliseconds}ms");
                PrintResult(result);
            }

            return result;
        }

        // Mode hybride : fait concourir le solver génétique avec d'autres solvers
        ISolver combiSolver = hasPlayed
            ? ParallelCombinationsSolver.Create(new Set(board), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(new Set(board), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var token = cts.Token;

        // Lance tous les solvers en parallèle
        var tasks = new List<Task<SolverResult>>
        {
            Task.Run(() => geneticSolver.SearchSolution(token), token),
            Task.Run(() => combiSolver.SearchSolution(token), token),
            Task.Run(() => incrementalSolver.SearchSolution(token), token)
        };

        // Attend le premier qui trouve une solution valide
        SolverResult? bestResult = null;
        var completedTasks = new HashSet<Task<SolverResult>>();

        while (tasks.Count > completedTasks.Count && bestResult == null)
        {
            var completedTask = await Task.WhenAny(tasks.Where(t => !completedTasks.Contains(t)));
            completedTasks.Add(completedTask);

            try
            {
                var result = await completedTask;
                if (result.Found)
                {
                    bestResult = result;
                    await cts.CancelAsync();
                }
            }
            catch (OperationCanceledException)
            {
                // Normal si un autre solver a déjà trouvé
            }
        }

        // Si aucun n'a trouvé de solution valide dans le temps imparti
        if (bestResult == null)
            // Prend le premier résultat disponible
            foreach (var task in completedTasks)
                try
                {
                    var result = await task;
                    bestResult = result;
                    break;
                }
                catch
                {
                    // Ignore les erreurs
                }

        stopwatch.Stop();

        if (_enableMeasurements)
        {
            Console.WriteLine($"\n[GeneticSolverStrategy] Hybrid mode - Total time: {stopwatch.ElapsedMilliseconds}ms");
            if (bestResult != null)
            {
                Console.WriteLine($"Winner: {bestResult.Source}");
                PrintResult(bestResult);
            }

            // Affiche les résultats de tous les solvers (si disponibles)
            await PrintAllResults(completedTasks);
        }

        return bestResult ?? SolverResult.Invalid("GeneticSolverStrategy");
    }

    private ISolver CreateFirstPlayGeneticSolver(Set rack)
    {
        // Pour le premier coup, utilise une configuration plus légère
        var firstPlayConfig = _geneticConfig ?? new GeneticConfiguration
        {
            PopulationSize = 50,
            PopulationCount = 2,
            MaxGenerations = 200,
            MutationRate = 0.15
        };

        return ParallelGeneticSolver.Create(new Set(), rack, true, firstPlayConfig);
    }

    private void PrintResult(SolverResult result)
    {
        if (result.Found)
        {
            Console.WriteLine($"  ✓ Solution found - Score: {result.Score}");
            Console.Write("  Tiles to play: ");
            foreach (var tile in result.TilesToPlay) tile.PrintTile();

            Console.WriteLine($"\n  Jokers to play: {result.JokerToPlay}");
            Console.WriteLine($"  Won: {result.Won}");
        }
        else
        {
            Console.WriteLine("  ✗ No solution found");
        }
    }

    private async Task PrintAllResults(HashSet<Task<SolverResult>> completedTasks)
    {
        Console.WriteLine("\n=== All Solver Results ===");
        foreach (var task in completedTasks)
            try
            {
                var result = await task;
                Console.WriteLine($"\n{result.Source}:");
                PrintResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting result: {ex.Message}");
            }

        Console.WriteLine("========================\n");
    }
}

/// <summary>
///     Version améliorée avec configuration adaptative basée sur la complexité du jeu
/// </summary>
public class AdaptiveGeneticSolverStrategy : ISolverStrategy
{
    private readonly bool _enableMeasurements;

    public AdaptiveGeneticSolverStrategy(bool enableMeasurements = false)
    {
        _enableMeasurements = enableMeasurements;
    }

    public async Task<SolverResult> GetSolverResult(
        Set board,
        Set rack,
        bool hasPlayed,
        CancellationToken externalToken = default)
    {
        // Adapte la configuration en fonction de la complexité
        var config = DetermineOptimalConfiguration(board, rack, hasPlayed);

        var strategy = new GeneticSolverStrategy(
            config,
            ShouldUseHybridMode(board, rack),
            _enableMeasurements);

        return await strategy.GetSolverResult(board, rack, hasPlayed, externalToken);
    }

    private GeneticConfiguration DetermineOptimalConfiguration(Set board, Set rack, bool hasPlayed)
    {
        var complexity = EstimateComplexity(board, rack, hasPlayed);

        return complexity switch
        {
            < 10 => GeneticConfiguration.Fast,
            < 50 => GeneticConfiguration.Default,
            _ => GeneticConfiguration.Aggressive
        };
    }

    private int EstimateComplexity(Set board, Set rack, bool hasPlayed)
    {
        var boardTiles = board.Tiles.Count - board.Jokers;
        var rackTiles = rack.Tiles.Count - rack.Jokers;
        var totalTiles = boardTiles + rackTiles;
        var jokers = board.Jokers + rack.Jokers;

        // Formule heuristique pour estimer la complexité
        var complexity = totalTiles * (hasPlayed ? 2 : 1);
        complexity += jokers * 5; // Les jokers augmentent beaucoup la complexité
        complexity += hasPlayed ? boardTiles / 3 : 0; // Plus de tuiles sur le plateau = plus complexe

        return complexity;
    }

    private bool ShouldUseHybridMode(Set board, Set rack)
    {
        // Utilise le mode hybride si le problème n'est pas trop complexe
        // Pour les problèmes très complexes, le génétique seul peut être meilleur
        var totalTiles = board.Tiles.Count + rack.Tiles.Count;
        return totalTiles < 60;
    }
}