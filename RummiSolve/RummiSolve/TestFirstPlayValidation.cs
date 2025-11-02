using System.Diagnostics;
using RummiSolve;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;

/// <summary>
///     Test pour vérifier que le solver génétique respecte la contrainte des 30 points pour le premier coup
/// </summary>
public class TestFirstPlayValidation
{
    public static async Task RunTests()
    {
        Console.WriteLine("=== Test de validation du premier coup (30 points minimum) ===\n");

        // Test 1 : Premier coup avec assez de points
        await TestFirstPlayWithEnoughPoints();

        // Test 2 : Premier coup avec trop peu de tuiles (devrait échouer proprement)
        await TestFirstPlayWithInsufficientTiles();

        // Test 3 : Premier coup avec stratégie
        await TestFirstPlayWithStrategy();

        Console.WriteLine("\n=== Tests terminés ===");
    }

    private static async Task TestFirstPlayWithEnoughPoints()
    {
        Console.WriteLine("Test 1 : Premier coup avec 30+ points disponibles");
        Console.WriteLine("---------------------------------------------------");

        var playerTiles = new List<Tile>
        {
            new(10),
            new(11),
            new(12),
            new(5, TileColor.Red),
            new(5, TileColor.Black),
            new(5, TileColor.Mango),
            new(1, TileColor.Red),
            new(2, TileColor.Red)
        };

        var config = new GeneticConfiguration
        {
            PopulationSize = 50,
            PopulationCount = 2,
            MaxGenerations = 200,
            EnableLogging = true
        };

        var solver = ParallelGeneticSolver.Create(new Set(), new Set(playerTiles), true, config);

        var stopwatch = Stopwatch.StartNew();
        var result = solver.SearchSolution();
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");

        if (result.Found)
        {
            var score = result.TilesToPlay.Sum(t => t.Value);
            Console.WriteLine($"✓ Solution trouvée avec {score} points");

            if (score >= 30)
                Console.WriteLine("✓ Score >= 30 points : VALIDE pour le premier coup");
            else
                Console.WriteLine("✗ Score < 30 points : INVALIDE pour le premier coup !");

            Console.Write("Tuiles jouées : ");
            foreach (var tile in result.TilesToPlay) tile.PrintTile();

            Console.WriteLine("\n");
        }
        else
        {
            Console.WriteLine("✗ Aucune solution trouvée\n");
        }
    }

    private static async Task TestFirstPlayWithInsufficientTiles()
    {
        Console.WriteLine("Test 2 : Premier coup avec tuiles insuffisantes (<30 points possible)");
        Console.WriteLine("----------------------------------------------------------------------");

        var playerTiles = new List<Tile>
        {
            new(1),
            new(2),
            new(3, TileColor.Red),
            new(4, TileColor.Black)
        };

        var config = new GeneticConfiguration
        {
            PopulationSize = 30,
            PopulationCount = 2,
            MaxGenerations = 100,
            EnableLogging = false
        };

        var solver = ParallelGeneticSolver.Create(new Set(), new Set(playerTiles), true, config);

        var stopwatch = Stopwatch.StartNew();
        var result = solver.SearchSolution();
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");

        if (result.Found)
        {
            var score = result.TilesToPlay.Sum(t => t.Value);
            Console.WriteLine($"⚠ Solution trouvée avec {score} points (attendu : aucune solution)\n");
        }
        else
        {
            Console.WriteLine("✓ Aucune solution trouvée (comportement attendu car <30 points impossible)\n");
        }
    }

    private static async Task TestFirstPlayWithStrategy()
    {
        Console.WriteLine("Test 3 : Premier coup avec PureGeneticStrategy");
        Console.WriteLine("------------------------------------------------");

        var playerTiles = new List<Tile>
        {
            new(10),
            new(11),
            new(12),
            new(13),
            new(7, TileColor.Red),
            new(7, TileColor.Black),
            new(7, TileColor.Mango)
        };

        var strategy = new SimpleGeneticStrategy();
        var player = new Player("TestPlayer", playerTiles, strategy);

        var boardSolution = new Solution { IsValid = true };

        var stopwatch = Stopwatch.StartNew();
        var solution = await player.SolveAsync(boardSolution);
        stopwatch.Stop();

        Console.WriteLine($"Temps : {stopwatch.ElapsedMilliseconds}ms");

        if (solution.IsValid)
        {
            var score = player.TilesToPlay.Sum(t => t.Value);
            Console.WriteLine($"✓ Solution trouvée avec {score} points");

            if (score >= 30)
                Console.WriteLine("✓ Score >= 30 points : VALIDE");
            else
                Console.WriteLine("✗ Score < 30 points : INVALIDE !");

            Console.Write("Tuiles jouées : ");
            foreach (var tile in player.TilesToPlay) tile.PrintTile();

            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("✗ Aucune solution trouvée");
        }
    }
}