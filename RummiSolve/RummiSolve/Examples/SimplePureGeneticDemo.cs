using System.Diagnostics;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;

namespace RummiSolve.Examples;

/// <summary>
///     Démonstration simple de l'utilisation de la stratégie génétique pure
/// </summary>
public class SimplePureGeneticDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("=== Démonstration de PureGeneticStrategy ===\n");

        // Scénario 1 : Utilisation la plus simple possible
        await TestSimplestUsage();

        // Scénario 2 : Utilisation avec différents profils
        await TestWithProfiles();

        // Scénario 3 : Utilisation avec configuration personnalisée
        await TestWithCustomConfig();

        Console.WriteLine("\n=== Fin de la démonstration ===");
    }

    /// <summary>
    ///     Test le plus simple : utilise SimpleGeneticStrategy qui s'adapte automatiquement
    /// </summary>
    private static async Task TestSimplestUsage()
    {
        Console.WriteLine("1. Usage le plus simple - SimpleGeneticStrategy (adaptatif automatique)\n");

        var playerTiles = new List<Tile>
        {
            new(10),
            new(11),
            new(12),
            new(5, TileColor.Red),
            new(5, TileColor.Black),
            new(5, TileColor.Mango),
            new(7, TileColor.Red),
            new(8, TileColor.Red),
            new(9, TileColor.Red)
        };

        // Stratégie la plus simple : s'adapte automatiquement
        var strategy = new SimpleGeneticStrategy();
        var player = new Player("Alice", playerTiles, strategy);

        var boardSolution = new Solution { IsValid = true };
        var solution = await player.SolveAsync(boardSolution);

        PrintResult("Alice (Auto-adaptatif)", solution, player);
    }

    /// <summary>
    ///     Test avec les différents profils prédéfinis
    /// </summary>
    private static async Task TestWithProfiles()
    {
        Console.WriteLine("\n2. Test avec profils prédéfinis\n");

        // Configuration du jeu
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

        var profiles = new[]
        {
            (GeneticProfile.UltraFast, "Ultra Rapide"),
            (GeneticProfile.Fast, "Rapide"),
            (GeneticProfile.Balanced, "Équilibré")
        };

        var boardSolution = new Solution { IsValid = true };
        boardSolution.Runs.Add(new Run { Tiles = boardSet.Tiles.ToArray() });

        foreach (var (profile, name) in profiles)
        {
            var strategy = PureGeneticStrategy.CreateWithProfile(profile);
            var player = new Player($"Joueur ({name})", new List<Tile>(playerTiles), strategy);

            // Simule que le joueur a déjà joué son premier coup
            var stopwatch = Stopwatch.StartNew();
            var solution = await player.SolveAsync(boardSolution);
            stopwatch.Stop();

            Console.WriteLine($"Profil {name} : {stopwatch.ElapsedMilliseconds}ms - Solution: {solution.IsValid}");
            if (solution.IsValid && player.TilesToPlay.Any())
            {
                Console.Write("  Tuiles: ");
                foreach (var tile in player.TilesToPlay) tile.PrintTile();

                Console.WriteLine();
            }
        }
    }

    /// <summary>
    ///     Test avec configuration personnalisée
    /// </summary>
    private static async Task TestWithCustomConfig()
    {
        Console.WriteLine("\n3. Configuration personnalisée\n");

        var playerTiles = new List<Tile>
        {
            new(1),
            new(2),
            new(3),
            new(4),
            new(5),
            new(6),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
            new(10, TileColor.Mango),
            new(true) // Joker
        };

        // Configuration personnalisée pour problème complexe avec joker
        var customConfig = new GeneticConfiguration
        {
            PopulationSize = 100,
            PopulationCount = 4,
            MaxGenerations = 300,
            MutationRate = 0.12,
            EliteSize = 15,
            MigrationInterval = 25,
            StagnationThreshold = 15,
            PerfectFitnessThreshold = 15000,
            EnableLogging = true
        };

        var strategy = new PureGeneticStrategy(customConfig, true);
        var player = new Player("Bob (Config personnalisée)", playerTiles, strategy);

        var boardSolution = new Solution { IsValid = true };
        var solution = await player.SolveAsync(boardSolution);

        PrintResult("Bob", solution, player);
    }

    private static void PrintResult(string playerName, Solution solution, Player player)
    {
        Console.WriteLine($"\nRésultat pour {playerName}:");
        if (solution.IsValid)
        {
            Console.WriteLine("  ✓ Solution trouvée!");
            Console.Write("  Tuiles à jouer: ");
            foreach (var tile in player.TilesToPlay) tile.PrintTile();

            Console.WriteLine($"\n  Nombre de tuiles: {player.TilesToPlay.Count}");
            Console.WriteLine($"  Victoire: {player.Won}");

            if (solution.Runs.Any() || solution.Groups.Any())
            {
                Console.WriteLine("  Solution:");
                solution.PrintSolution();
            }
        }
        else
        {
            Console.WriteLine("  ✗ Pas de solution trouvée");
        }
    }

    /// <summary>
    ///     Exemple d'intégration dans un jeu complet
    /// </summary>
    public static async Task RunGameExample()
    {
        Console.WriteLine("=== Exemple de partie avec PureGeneticStrategy ===\n");

        // Crée 3 joueurs avec différentes stratégies génétiques
        var players = new List<Player>
        {
            new(
                "Alice (Ultra rapide)",
                GenerateRandomTiles(14),
                PureGeneticStrategy.CreateWithProfile(GeneticProfile.UltraFast)
            ),
            new(
                "Bob (Équilibré)",
                GenerateRandomTiles(14),
                PureGeneticStrategy.CreateWithProfile(GeneticProfile.Balanced)
            ),
            new(
                "Charlie (Simple auto)",
                GenerateRandomTiles(14),
                new SimpleGeneticStrategy()
            )
        };

        var boardSolution = new Solution { IsValid = true };
        var turn = 1;

        // Simule quelques tours
        for (var i = 0; i < 3; i++)
        {
            Console.WriteLine($"\n--- Tour {turn++} ---");

            foreach (var player in players)
            {
                Console.WriteLine($"\n{player.Name} joue...");

                var stopwatch = Stopwatch.StartNew();
                var solution = await player.SolveAsync(boardSolution);
                stopwatch.Stop();

                Console.WriteLine($"  Temps: {stopwatch.ElapsedMilliseconds}ms");

                if (solution.IsValid && player.TilesToPlay.Any())
                {
                    Console.WriteLine($"  ✓ {player.TilesToPlay.Count} tuiles jouées");
                    player.Play();
                    boardSolution = solution;

                    if (player.Won)
                    {
                        Console.WriteLine($"\n🎉 {player.Name} a gagné!");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("  ✗ Passe son tour");
                    // Simule une pioche
                    player.Drew(GenerateRandomTiles(1)[0]);
                }
            }
        }
    }

    private static List<Tile> GenerateRandomTiles(int count)
    {
        var random = new Random();
        var tiles = new List<Tile>();
        var colors = new[] { TileColor.Blue, TileColor.Red, TileColor.Black, TileColor.Mango };

        for (var i = 0; i < count; i++)
            if (random.Next(100) < 5) // 5% de chance d'avoir un joker
            {
                tiles.Add(new Tile(true));
            }
            else
            {
                var value = random.Next(1, 14);
                var color = colors[random.Next(colors.Length)];
                tiles.Add(new Tile(value, color));
            }

        return tiles;
    }
}