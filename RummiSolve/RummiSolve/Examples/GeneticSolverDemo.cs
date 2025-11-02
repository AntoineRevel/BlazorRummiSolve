using System.Diagnostics;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;

namespace RummiSolve.Examples;

/// <summary>
///     Programme de démonstration du ParallelGeneticSolver intégré avec un joueur
/// </summary>
public class GeneticSolverDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("=== Démonstration du ParallelGeneticSolver ===\n");

        // Scénario 1 : Premier coup
        await TestFirstPlay();

        // Scénario 2 : Jeu avec plateau existant
        await TestWithExistingBoard();

        // Scénario 3 : Comparaison de configurations
        await TestDifferentConfigurations();

        // Scénario 4 : Mode hybride vs standalone
        await TestHybridVsStandalone();

        Console.WriteLine("\n=== Fin de la démonstration ===");
    }

    private static async Task TestFirstPlay()
    {
        Console.WriteLine("\n--- Test 1: Premier coup ---");

        // Crée un joueur avec des tuiles pour faire au moins 30 points
        var playerTiles = new List<Tile>
        {
            new(10),
            new(11),
            new(12),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
            new(7, TileColor.Mango),
            new(8, TileColor.Mango),
            new(9, TileColor.Mango)
        };

        // Crée une stratégie génétique
        var strategy = new GeneticSolverStrategy(
            GeneticConfiguration.Fast,
            false,
            true);

        var player = new Player("Alice (Genetic)", playerTiles, strategy);

        // Le joueur joue son premier coup
        var boardSolution = new Solution { IsValid = true };
        var solution = await player.SolveAsync(boardSolution);

        if (solution.IsValid)
        {
            Console.WriteLine($"✓ {player.Name} a trouvé une solution!");
            Console.Write("  Tuiles jouées: ");
            foreach (var tile in player.TilesToPlay) tile.PrintTile();

            Console.WriteLine($"\n  Nombre de tuiles jouées: {player.TilesToPlay.Count}");
            Console.WriteLine($"  Victoire: {player.Won}");

            player.Play();
        }
        else
        {
            Console.WriteLine($"✗ {player.Name} n'a pas trouvé de solution");
        }
    }

    private static async Task TestWithExistingBoard()
    {
        Console.WriteLine("\n--- Test 2: Jeu avec plateau existant ---");

        // Plateau avec des combinaisons existantes
        var boardSolution = new Solution { IsValid = true };
        boardSolution.AddRun(new Run
        {
            Tiles =
            [
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red),
                new Tile(4, TileColor.Red)
            ]
        });
        boardSolution.AddGroup(new Group
        {
            Tiles =
            [
                new Tile(5),
                new Tile(5, TileColor.Red),
                new Tile(5, TileColor.Black)
            ]
        });

        var playerTiles = new List<Tile>
        {
            new(5, TileColor.Mango), // Peut compléter le groupe
            new(5, TileColor.Red), // Peut étendre le run ou faire un nouveau groupe
            new(6, TileColor.Red), // Peut étendre le run
            new(10),
            new(11),
            new(12) // Peut faire un nouveau run
        };

        var config = new GeneticConfiguration
        {
            PopulationSize = 100,
            PopulationCount = 4,
            MaxGenerations = 300,
            EnableLogging = true
        };

        var strategy = new GeneticSolverStrategy(config, false, true);
        var player = new Player("Bob (Genetic Custom)", playerTiles, strategy);
        // Note: Dans un vrai jeu, le joueur aurait déjà joué son premier coup

        var solution = await player.SolveAsync(boardSolution);

        if (solution.IsValid)
        {
            Console.WriteLine($"✓ {player.Name} a trouvé une solution!");
            Console.WriteLine($"  Solution trouvée avec {player.TilesToPlay.Count} tuiles");
            player.Play();
        }
        else
        {
            Console.WriteLine($"✗ {player.Name} n'a pas trouvé de solution");
        }
    }

    private static async Task TestDifferentConfigurations()
    {
        Console.WriteLine("\n--- Test 3: Comparaison de configurations ---");

        var boardSolution = new Solution { IsValid = true };
        var playerTiles = new List<Tile>
        {
            new(1),
            new(2),
            new(3),
            new(4),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
            new(10, TileColor.Mango)
        };

        var configs = new[]
        {
            ("Fast", GeneticConfiguration.Fast),
            ("Default", GeneticConfiguration.Default),
            ("Aggressive", GeneticConfiguration.Aggressive)
        };

        foreach (var (name, config) in configs)
        {
            var strategy = new GeneticSolverStrategy(config, false);
            var player = new Player($"Player ({name})", new List<Tile>(playerTiles), strategy);

            var stopwatch = Stopwatch.StartNew();
            var solution = await player.SolveAsync(boardSolution);
            stopwatch.Stop();

            Console.WriteLine($"  Configuration {name}: {stopwatch.ElapsedMilliseconds}ms - Found: {solution.IsValid}");
        }
    }

    private static async Task TestHybridVsStandalone()
    {
        Console.WriteLine("\n--- Test 4: Mode Hybride vs Standalone ---");

        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red),
            new(5, TileColor.Red),
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black)
        };

        // Test mode standalone
        var standaloneStrategy = new GeneticSolverStrategy(
            GeneticConfiguration.Default,
            false,
            true);

        var player1 = new Player("Player (Standalone)", new List<Tile>(playerTiles), standaloneStrategy);

        Console.WriteLine("\n  Mode Standalone:");
        var stopwatch1 = Stopwatch.StartNew();
        var solution1 = await player1.SolveAsync(new Solution { IsValid = true }.AddSolution(new Solution
        {
            IsValid = true,
            Runs = { new Run { Tiles = boardSet.Tiles.ToArray() } }
        }));
        stopwatch1.Stop();
        Console.WriteLine($"    Temps: {stopwatch1.ElapsedMilliseconds}ms - Found: {solution1.IsValid}");

        // Test mode hybride
        var hybridStrategy = new GeneticSolverStrategy(
            GeneticConfiguration.Default,
            true,
            true);

        var player2 = new Player("Player (Hybrid)", new List<Tile>(playerTiles), hybridStrategy);

        Console.WriteLine("\n  Mode Hybride (Génétique + Autres solvers):");
        var stopwatch2 = Stopwatch.StartNew();
        var solution2 = await player2.SolveAsync(new Solution { IsValid = true }.AddSolution(new Solution
        {
            IsValid = true,
            Runs = { new Run { Tiles = boardSet.Tiles.ToArray() } }
        }));
        stopwatch2.Stop();
        Console.WriteLine($"    Temps: {stopwatch2.ElapsedMilliseconds}ms - Found: {solution2.IsValid}");
    }

    /// <summary>
    ///     Teste la stratégie adaptative qui ajuste automatiquement la configuration
    /// </summary>
    public static async Task TestAdaptiveStrategy()
    {
        Console.WriteLine("\n=== Test de la Stratégie Adaptative ===\n");

        var scenarios = new[]
        {
            (
                "Complexité faible",
                new Set([
                    new Tile(1, TileColor.Red),
                    new Tile(2, TileColor.Red),
                    new Tile(3, TileColor.Red)
                ]),
                new List<Tile>
                {
                    new(4, TileColor.Red),
                    new(5, TileColor.Red)
                }
            ),
            (
                "Complexité moyenne",
                new Set([
                    new Tile(1, TileColor.Red),
                    new Tile(2, TileColor.Red),
                    new Tile(3, TileColor.Red),
                    new Tile(5),
                    new Tile(5, TileColor.Red),
                    new Tile(5, TileColor.Black),
                    new Tile(7, TileColor.Mango),
                    new Tile(8, TileColor.Mango),
                    new Tile(9, TileColor.Mango)
                ]),
                new List<Tile>
                {
                    new(4, TileColor.Red),
                    new(5, TileColor.Mango),
                    new(10, TileColor.Mango),
                    new(11),
                    new(12),
                    new(13)
                }
            ),
            (
                "Complexité élevée avec jokers",
                new Set([
                    new Tile(1, TileColor.Red),
                    new Tile(2, TileColor.Red),
                    new Tile(3, TileColor.Red),
                    new Tile(4, TileColor.Red),
                    new Tile(5, TileColor.Red),
                    new Tile(true), // Joker
                    new Tile(7),
                    new Tile(8),
                    new Tile(9),
                    new Tile(10),
                    new Tile(11),
                    new Tile(12)
                ]),
                new List<Tile>
                {
                    new(6, TileColor.Red),
                    new(13),
                    new(true), // Joker
                    new(10, TileColor.Red),
                    new(10, TileColor.Black),
                    new(10, TileColor.Mango),
                    new(11, TileColor.Red),
                    new(11, TileColor.Black)
                }
            )
        };

        var adaptiveStrategy = new AdaptiveGeneticSolverStrategy(true);

        foreach (var (description, boardSet, playerTiles) in scenarios)
        {
            Console.WriteLine($"\n--- {description} ---");

            var player = new Player("Player (Adaptive)", playerTiles, adaptiveStrategy);

            var boardSolution = new Solution { IsValid = true };
            if (boardSet.Tiles.Any()) boardSolution.AddRun(new Run { Tiles = boardSet.Tiles.ToArray() });

            var stopwatch = Stopwatch.StartNew();
            var solution = await player.SolveAsync(boardSolution);
            stopwatch.Stop();

            Console.WriteLine($"  Temps: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"  Solution trouvée: {solution.IsValid}");
            if (solution.IsValid) Console.WriteLine($"  Tuiles jouées: {player.TilesToPlay.Count}");
        }
    }
}