using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Strategy;

namespace RummiSolve;

public class RummiBench
{
    private readonly List<string> _playerNames = ["Antoine", "Matthieu", "Maguy"];
    private Game _game;


    public static async Task TestSimpleGame()
    {
        Game game = new(Guid.Parse("32ba6c8d-3bb2-41f6-8292-655f9459d53c"));

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        while (!game.IsGameOver) await game.PlayAsync();
    }


    public static async Task TestSimpleGame2()
    {
        var game = new Game(Guid.Parse("8f53d490-db85-4962-8886-8a49c0e2afb8"));
        var playerNames = new List<string> { "Antoine", "Matthieu", "Maguy" };

        game.InitializeGame(playerNames);

        Console.WriteLine("=== DÉBUT DE LA PARTIE ===");
        Console.WriteLine($"Joueurs: {string.Join(", ", playerNames)}");
        Console.WriteLine($"Tuiles complètes: {game.AllTiles()}\n");

        while (!game.IsGameOver)
        {
            var currentPlayer = game.Players[game.PlayerIndex];

            Console.WriteLine($"=== TOUR {game.Turn} ===");
            Console.WriteLine($"Joueur actuel: {currentPlayer.Name}");
            Console.WriteLine("Main du joueur:");
            currentPlayer.Rack.PrintAllTiles();

            await game.PlayAsync();

            Console.WriteLine("\nRésultat du tour:");
            Console.WriteLine($"Tuiles complètes: {game.AllTiles()}");


            Console.WriteLine("Main du joueur:");
            currentPlayer.Rack.PrintAllTiles();


            Console.WriteLine("\nPlateau actuel:");
            game.Board.PrintSolution();

            Console.WriteLine("\n--------------------------------\n");
        }

        Console.WriteLine("=== FIN DE LA PARTIE ===");
    }


    public static void DontGetError2()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(4),
            new Tile(8),
            new Tile(11),
            new Tile(11),
            new Tile(13),
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Black),
        ]);

        var solver = IncrementalFirstBaseSolver.Create(playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        solution.PrintSolution();

        foreach (var tile in tilesToPlay)
        {
            tile.PrintTile();
        }
    }

    public static void SearchSolution_ValidNotWon()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(6, TileColor.Mango),
            new Tile(true),
            new Tile(8, TileColor.Mango),

            new Tile(10, TileColor.Red),
            new Tile(11, TileColor.Red),
            new Tile(12, TileColor.Red),

            new Tile(13),
            new Tile(13, TileColor.Black),
            new Tile(13, TileColor.Red),

            new Tile(12),
            new Tile(12, TileColor.Black),
            new Tile(12, TileColor.Mango),

            new Tile(11),
            new Tile(11, TileColor.Black),
            new Tile(11, TileColor.Red),

            new Tile(10),
            new Tile(10, TileColor.Black),
            new Tile(10, TileColor.Mango),

            new Tile(9),
            new Tile(9, TileColor.Mango),
            new Tile(9, TileColor.Red),

            new Tile(6),
            new Tile(6, TileColor.Black),
            new Tile(6, TileColor.Red),

            new Tile(2),
            new Tile(2, TileColor.Black),
            new Tile(2, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Black),
            new Tile(8),
            new Tile(3, TileColor.Mango),
            //new Tile(2,TileColor.Red),
            new Tile(8),
            new Tile(2, TileColor.Black),
        ]);

        var solver = IncrementalComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        playerSet.PrintAllTiles();
        Console.WriteLine();
        boardSet.PrintAllTiles();
        Console.WriteLine();
        Console.WriteLine();
        solution.PrintSolution();
        Console.WriteLine("Tile to play :");
        foreach (var tile in tilesToPlay)
        {
            tile.PrintTile();
        }

        Console.WriteLine("Joker :");
        Console.WriteLine(jokerToPlay);
    }

    public static void SearchSolution_ValidNotBugWon()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(10, TileColor.Black),
            new Tile(11, TileColor.Black),
            new Tile(12, TileColor.Black),
            new Tile(13, TileColor.Black),

            new Tile(true),
            new Tile(12),
            new Tile(13),

            new Tile(6),
            new Tile(7),
            new Tile(8),

            new Tile(8, TileColor.Red),
            new Tile(8, TileColor.Mango),
            new Tile(8, TileColor.Black),

            new Tile(5),
            new Tile(5, TileColor.Mango),
            new Tile(5, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(10, TileColor.Black),
            new Tile(5, TileColor.Black),

            new Tile(13, TileColor.Mango),
            new Tile(13, TileColor.Mango),
            new Tile(12, TileColor.Mango),
            new Tile(9, TileColor.Mango),
            new Tile(7, TileColor.Mango),
            new Tile(5, TileColor.Mango),
            new Tile(3, TileColor.Mango),
            new Tile(2, TileColor.Mango),

            new Tile(10, TileColor.Red),
            new Tile(7, TileColor.Red),

            new Tile(11),
            new Tile(9)
        ]);

        var solver = IncrementalComplexSolver.Create(boardSet, playerSet);

        // Act

        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        playerSet.PrintAllTiles();
        Console.WriteLine();
        boardSet.PrintAllTiles();
        Console.WriteLine();
        Console.WriteLine();
        solution.PrintSolution();
        Console.WriteLine("Tile to play :");
        foreach (var tile in tilesToPlay)
        {
            tile.PrintTile();
        }

        // Console.WriteLine("Joker :");
        // Console.WriteLine(jokerToPlay);
    }

    public static void TestBestScore()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new((byte)26),
            new((byte)36),
            new((byte)43),
            new((byte)45),
            new((byte)57),
            new((byte)58),
            new((byte)59),
        };

        foreach (var tile in playerTiles) tile.PrintTile();

        Console.WriteLine();

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        solution.PrintSolution();
    }


    public static async Task RunGamesUntilErrorAsync(int maxGames = 1000)
    {
        var tasks = new List<Task>();


        for (var i = 0; i < maxGames; i++)
            tasks.Add(Task.Run(async () =>
            {
                var game = new Game();
                game.InitializeGame(["Antoine", "Matthieu", "Maguy"]);

                try
                {
                    if (game.AllTiles() != 106)
                        throw new Exception("Erreur au départ");

                    while (!game.IsGameOver)
                    {
                        await game.PlayAsync();
                        if (game.AllTiles() != 106)
                            throw new Exception("Erreur en cours de partie");
                    }

                    if (game.AllTiles() != 106)
                        throw new Exception("Erreur en fin de partie");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"🛑 Game ID {game.Id} a échoué : {ex.Message}");

                    throw;
                }
            }));

        try
        {
            await Task.WhenAll(tasks);
            Console.WriteLine("✅ Toutes les parties se sont bien déroulées !");
        }
        catch
        {
            Console.WriteLine("❌ Une erreur a été détectée, arrêt des tests.");
        }
    }

    public static async Task Test_CharlieTurn15_ParallelSolverStrategy()
    {
        // Arrange - État du tour 15 de Charlie
        var boardTiles = new List<Tile>
        {
            new(10, TileColor.Black),
            new(11, TileColor.Black),
            new(12, TileColor.Black),
            new(8, TileColor.Black),
            new(9, TileColor.Black),
            new(10, TileColor.Black),
            new(5, TileColor.Black),
            new(6, TileColor.Black),
            new(7, TileColor.Black),
            new(1, TileColor.Mango),
            new(2, TileColor.Mango),
            new(3, TileColor.Mango),
            new(7),
            new(8),
            new(9),
            new(true),
            new(11),
            new(6),
            new(7),
            new(8),
            new(2),
            new(3),
            new(4),
            new(13, TileColor.Red),
            new(13, TileColor.Mango),
            new(13, TileColor.Black),
            new(8, TileColor.Red),
            new(8, TileColor.Mango),
            new(8, TileColor.Black),
            new(13),
            new(13, TileColor.Mango),
            new(13, TileColor.Black),
            new(12),
            new(12, TileColor.Red),
            new(12, TileColor.Mango),
            new(5),
            new(5, TileColor.Mango),
            new(5, TileColor.Black),
            new(4),
            new(4, TileColor.Red),
            new(4, TileColor.Black),
            new(2),
            new(2, TileColor.Red),
            new(2, TileColor.Mango),
            new(2, TileColor.Black),
            new(1),
            new(1, TileColor.Red),
            new(1, TileColor.Black)
        };
        var rackTiles = new List<Tile>
        {
            new(13),
            new(7, TileColor.Mango),
            new(4, TileColor.Mango),
            new(12, TileColor.Black),
            new(12),
            new(3, TileColor.Black),
            new(10),
            new(11, TileColor.Red),
            new(5, TileColor.Red),
            new(3, TileColor.Red),
            new(3, TileColor.Red),
            new(7, TileColor.Mango),
            new(6),
            new(9, TileColor.Red),
            new(5),
            new(10),
            new(8, TileColor.Red),
            new(11, TileColor.Black)
        };


        var strategy = new ParallelSolverStrategy();

        // Act
        var result = await strategy.GetSolverResult(new Set(boardTiles), new Set(rackTiles), true);

        Console.WriteLine(result.Source);

        result.BestSolution.PrintSolution();
    }
}