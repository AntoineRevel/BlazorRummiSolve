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
        Game game = new(Guid.Parse("f194c3b0-8088-452c-b180-66614ba2f6d7"));
        //Guid.Parse("d072484b-24e4-46f8-b60b-a37fe584b165")

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    public static void GetFirstSolution_ReturnsValidSolutionIfPossible()
    {
        // Arrange
        var tiles = new List<Tile>
        {
            new(3, TileColor.Red),
            new(4, TileColor.Red),
            new(5, TileColor.Red)
        };
        var set = new Set(tiles);

        // Act
        var solution = SolverSet.Create(set, set).GetSolution();

        solution.PrintSolution();
        // Assert
    }
}