using System.Diagnostics;
using RummiSolve;
using RummiSolve.Examples;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;

/// <summary>
///     Programme de test rapide du nouveau ParallelGeneticSolver
/// </summary>
public class TestGeneticSolver
{
    public static async Task RunFullTest(string[] args)
    {
        Console.WriteLine("=== Test du ParallelGeneticSolver ===\n");

        // Test simple avec un jeu de base
        await TestBasicGame();

        // Test de la démonstration complète (optionnel)
        if (args.Contains("--full"))
        {
            await GeneticSolverDemo.RunDemo();
            await GeneticSolverDemo.TestAdaptiveStrategy();
        }

        Console.WriteLine("\n=== Test terminé ===");
    }

    private static async Task TestBasicGame()
    {
        Console.WriteLine("Test d'une partie simple avec le solver génétique :\n");

        // Création du plateau initial (vide pour le premier tour)
        var boardSet = new Set();

        // Création de la main du joueur
        var playerTiles = new List<Tile>
        {
            // Un run potentiel
            new(10),
            new(11),
            new(12),

            // Un groupe potentiel
            new(5, TileColor.Red),
            new(5, TileColor.Black),
            new(5, TileColor.Mango),

            // Tuiles supplémentaires
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(7, TileColor.Black)
        };

        // Configuration du solver génétique
        var config = new GeneticConfiguration
        {
            PopulationSize = 50,
            PopulationCount = 3,
            MaxGenerations = 200,
            MutationRate = 0.15,
            EnableLogging = true
        };

        // Test avec la stratégie génétique standalone
        Console.WriteLine("1. Test avec ParallelGeneticSolver (standalone) :");
        var geneticStrategy = new GeneticSolverStrategy(config, false, true);
        var player1 = new Player("Alice (Génétique)", new List<Tile>(playerTiles), geneticStrategy);

        var stopwatch = Stopwatch.StartNew();
        var solution1 = await player1.SolveAsync(new Solution { IsValid = true });
        stopwatch.Stop();

        Console.WriteLine($"   Temps d'exécution : {stopwatch.ElapsedMilliseconds}ms");
        if (solution1.IsValid)
        {
            Console.WriteLine("   ✓ Solution trouvée!");
            Console.WriteLine($"   Tuiles à jouer : {player1.TilesToPlay.Count}");
            Console.Write("   ");
            foreach (var tile in player1.TilesToPlay) tile.PrintTile();

            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("   ✗ Pas de solution trouvée");
        }

        // Test avec la stratégie hybride
        Console.WriteLine("\n2. Test avec stratégie hybride (Génétique + autres solvers) :");
        var hybridStrategy = new GeneticSolverStrategy(config);
        var player2 = new Player("Bob (Hybride)", new List<Tile>(playerTiles), hybridStrategy);

        stopwatch = Stopwatch.StartNew();
        var solution2 = await player2.SolveAsync(new Solution { IsValid = true });
        stopwatch.Stop();

        Console.WriteLine($"   Temps d'exécution : {stopwatch.ElapsedMilliseconds}ms");
        if (solution2.IsValid)
        {
            Console.WriteLine("   ✓ Solution trouvée!");
            Console.WriteLine($"   Tuiles à jouer : {player2.TilesToPlay.Count}");
        }
        else
        {
            Console.WriteLine("   ✗ Pas de solution trouvée");
        }

        // Test avec la stratégie adaptative
        Console.WriteLine("\n3. Test avec stratégie adaptative :");
        var adaptiveStrategy = new AdaptiveGeneticSolverStrategy();
        var player3 = new Player("Charlie (Adaptatif)", playerTiles, adaptiveStrategy);

        stopwatch = Stopwatch.StartNew();
        var solution3 = await player3.SolveAsync(new Solution { IsValid = true });
        stopwatch.Stop();

        Console.WriteLine($"   Temps d'exécution : {stopwatch.ElapsedMilliseconds}ms");
        if (solution3.IsValid)
        {
            Console.WriteLine("   ✓ Solution trouvée!");
            Console.WriteLine($"   Tuiles à jouer : {player3.TilesToPlay.Count}");
        }
        else
        {
            Console.WriteLine("   ✗ Pas de solution trouvée");
        }
    }
}