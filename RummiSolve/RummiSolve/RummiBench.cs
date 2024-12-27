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
}