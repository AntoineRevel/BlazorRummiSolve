using System.Diagnostics;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;

namespace RummiSolve;

public static class Program
{
    private static async Task Main()
    {
        //BenchmarkRunner.Run<ParallelSolverBenchmark>();

        // Test de validation du premier coup
        await TestFirstPlayValidation.RunTests();

        // Test de la nouvelle stratégie simple (commenté)
        // await TestSimpleGeneticStrategy();

        // Test complet du solver génétique (commenté)
        // await TestGeneticSolver();

        // Exécution normale du benchmark (commenté)
        // await RummiBench.TestSimpleGame2();
    }

    private static async Task TestSimpleGeneticStrategy()
    {
        Console.WriteLine("=== Test de la stratégie PureGeneticStrategy (simple) ===\n");

        // Configuration du jeu
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red), // Peut étendre le run
            new(5, TileColor.Red),
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black) // Peut faire un groupe
        };

        Console.WriteLine("Test 1 : SimpleGeneticStrategy (adaptative automatique)");
        Console.WriteLine("-------------------------------------------------");

        // Utilisation la plus simple : stratégie adaptative automatique
        var simpleStrategy = new SimpleGeneticStrategy();
        var player1 = new Player("Joueur Simple", playerTiles, simpleStrategy);

        var stopwatch = Stopwatch.StartNew();
        var boardSolution1 = new Solution { IsValid = true };
        boardSolution1.Runs.Add(new Run { Tiles = boardSet.Tiles.ToArray() });
        var solution1 = await player1.SolveAsync(boardSolution1);
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");
        PrintSolution(solution1, player1);

        Console.WriteLine("\nTest 2 : PureGeneticStrategy avec profil Fast");
        Console.WriteLine("-------------------------------------------------");

        // Utilisation avec profil prédéfini
        var fastStrategy = PureGeneticStrategy.CreateWithProfile(GeneticProfile.Fast);
        var player2 = new Player("Joueur Fast", new List<Tile>(playerTiles), fastStrategy);

        stopwatch = Stopwatch.StartNew();
        var boardSolution2 = new Solution { IsValid = true };
        boardSolution2.Runs.Add(new Run { Tiles = boardSet.Tiles.ToArray() });
        var solution2 = await player2.SolveAsync(boardSolution2);
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");
        PrintSolution(solution2, player2);

        Console.WriteLine("\nTest 3 : PureGeneticStrategy avec configuration personnalisée");
        Console.WriteLine("-------------------------------------------------");

        // Configuration personnalisée minimaliste
        var customConfig = new GeneticConfiguration
        {
            PopulationSize = 30,
            PopulationCount = 2,
            MaxGenerations = 100,
            MutationRate = 0.15,
            EnableLogging = true
        };

        var customStrategy = new PureGeneticStrategy(customConfig, true);
        var player3 = new Player("Joueur Custom", new List<Tile>(playerTiles), customStrategy);

        stopwatch = Stopwatch.StartNew();
        var boardSolution3 = new Solution { IsValid = true };
        boardSolution3.Runs.Add(new Run { Tiles = boardSet.Tiles.ToArray() });
        var solution3 = await player3.SolveAsync(boardSolution3);
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");
        PrintSolution(solution3, player3);
    }

    private static void PrintSolution(Solution solution, Player player)
    {
        if (solution.IsValid && player.TilesToPlay.Any())
        {
            Console.WriteLine("✓ Solution trouvée!");
            Console.Write("Tuiles jouées : ");
            foreach (var tile in player.TilesToPlay) tile.PrintTile();

            Console.WriteLine($"\nNombre de tuiles : {player.TilesToPlay.Count}");
        }
        else
        {
            Console.WriteLine("✗ Aucune solution trouvée");
        }
    }

    private static Task TestGeneticSolver()
    {
        Console.WriteLine("=== Test du nouveau ParallelGeneticSolver ===\n");

        // Création d'un plateau avec quelques tuiles
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        // Main du joueur
        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red), // Peut étendre le run
            new(5, TileColor.Red), // Peut étendre le run
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black) // Peut faire un groupe
        };

        // Configuration rapide
        var config = new GeneticConfiguration
        {
            PopulationSize = 50,
            PopulationCount = 2,
            MaxGenerations = 100,
            EnableLogging = true
        };

        // Test du solver (pas le premier coup dans cet exemple)
        var solver = ParallelGeneticSolver.Create(boardSet, new Set(playerTiles), false, config);

        Console.WriteLine("Plateau initial :");
        boardSet.PrintAllTiles();

        Console.WriteLine("\nMain du joueur :");
        foreach (var tile in playerTiles) tile.PrintTile();

        Console.WriteLine("\n");

        var stopwatch = Stopwatch.StartNew();
        var result = solver.SearchSolution();
        stopwatch.Stop();

        Console.WriteLine($"\nRésultat trouvé en {stopwatch.ElapsedMilliseconds}ms");

        if (result.Found)
        {
            Console.WriteLine("✓ Solution trouvée !");
            Console.WriteLine($"Score : {result.Score}");
            Console.Write("Tuiles à jouer : ");
            foreach (var tile in result.TilesToPlay) tile.PrintTile();

            Console.WriteLine($"\nJokers à jouer : {result.JokerToPlay}");

            Console.WriteLine("\nSolution complète :");
            result.BestSolution.PrintSolution();
        }
        else
        {
            Console.WriteLine("✗ Aucune solution trouvée");
        }

        return Task.CompletedTask;
    }
}