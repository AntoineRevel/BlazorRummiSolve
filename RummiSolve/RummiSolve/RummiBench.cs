using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace RummiSolve;

[MemoryDiagnoser]
public class RummiBench
{
    private readonly List<string> _playerNames = ["Antoine", "Matthieu", "Maguy"];
    private Game _game;

    [IterationSetup]
    public void IterationSetup()
    {
        _game = new Game(Guid.Parse("f194c3b0-8088-452c-b180-66614ba2f6d7"));
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

    public static void DontGetError()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(9, TileColor.Blue, true),
            new Tile(10),
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
        ]);

        // Act
        var solver = SolverSet.Create(boardSet, playerSet,true);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;

        solution.PrintSolution();
        // Assert
    }

    public static void DontGetError2()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(4, TileColor.Red),
            new Tile(true),
            new Tile(5,TileColor.Red)
        ]);


        var solver = SolverSet.Create(boardSet, playerSet);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;

        var jokersToPlay = solver.JokerToPlay;

        solution.PrintSolution();
        foreach (var tile in tileToPlay)
        {
            tile.PrintTile();
        }

        Console.WriteLine();
        Console.WriteLine(jokersToPlay);
    }
}