using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using RummiSolve.Solver;

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

    [Benchmark]
    public void TestMultiPlayerGame_Benchmark()
    {
        _game.Start();
    }

    public static void TestMultiPlayerGame()
    {
        Game game = new(Guid.Parse("74cdccda-9261-460c-9414-31d7270ad2a1"));
        //Guid.Parse("74cdccda-9261-460c-9414-31d7270ad2a1")

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
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

        var solver = IncrementalFirstSolver.Create(playerSet);

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

        var solver = IncrementalSolver.Create(boardSet, playerSet);

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
            new Tile(2, TileColor.Red),
            new Tile(8),
            new Tile(2, TileColor.Black),
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

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

    public static void TestBestScore()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
        ]);

        var playerSet = new Set([
            new Tile(4),
            new Tile(5),
        ]);

        var solver = ScoreSolver.Create(boardSet, playerSet);
        solver.SearchBestScore();
        Console.WriteLine(solver.BestScore);
    }
}