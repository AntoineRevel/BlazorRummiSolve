using BenchmarkDotNet.Attributes;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;

namespace RummiSolve;

[MemoryDiagnoser]
public class RummiBench
{
    private readonly List<string> _playerNames = ["Antoine", "Matthieu", "Maguy"];
    private Game _game;

    [IterationSetup]
    public void IterationSetup()
    {
        _game = new Game(Guid.Parse("74cdccda-9261-460c-9414-31d7270ad2a1"));
        _game.InitializeGame(_playerNames);
    }


    public static void TestSimpleGame()
    {
        Game game = new(Guid.Parse("8f53d490-db85-4962-8886-8a49c0e2afb8"));

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var player = game.Players[game.PlayerIndex];

        Console.WriteLine(game.AllTiles());
        player.Rack.PrintAllTiles();
        game.Play();
        Console.WriteLine(game.AllTiles());
        game.Board.PrintSolution();

        Console.WriteLine();

        player = game.Players[game.PlayerIndex];
        player.Rack.PrintAllTiles();
        game.Play();
        Console.WriteLine(game.AllTiles());
        game.Players[game.PrevPlayerIndex].Rack.PrintAllTiles();
        game.Board.PrintSolution();

        Console.WriteLine();

        player = game.Players[game.PlayerIndex];
        Console.WriteLine(game.AllTiles());
        player.Rack.PrintAllTiles();
        game.Play();
        game.Players[game.PrevPlayerIndex].Rack.PrintAllTiles();
        Console.WriteLine(game.AllTiles());
        game.Board.PrintSolution();
    }


    public static void TestSimpleGame2()
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

            game.Play();

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
}