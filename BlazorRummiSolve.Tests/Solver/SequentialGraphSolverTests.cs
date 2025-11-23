using RummiSolve;
using RummiSolve.Solver.Graph;
using Xunit.Abstractions;

namespace BlazorRummiSolve.Tests.Solver;

public class SequentialGraphSolverTests(ITestOutputHelper output)
{
    [Fact]
    public void SequentialGraphSolver_SimpleCase_ShouldReturnValidSolution()
    {
        // Arrange
        var boardSet = new Set();
        boardSet.AddTile(new Tile(1, TileColor.Red));
        boardSet.AddTile(new Tile(2, TileColor.Red));
        boardSet.AddTile(new Tile(3, TileColor.Red));

        var playerSet = new Set();
        playerSet.AddTile(new Tile(4, TileColor.Red));
        playerSet.AddTile(new Tile(5, TileColor.Red));

        // Act
        var solver = SequentialGraphSolver.Create(boardSet, playerSet);
        var result = solver.SearchSolution();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Found);
        output.WriteLine($"Solution found with score: {result.Score}");
        output.WriteLine($"Tiles to play: {result.TilesToPlay.Count()}");
        output.WriteLine($"Jokers to play: {result.JokerToPlay}");
    }

    [Fact]
    public void SequentialGraphSolver_ComplexCase_ShouldReturnValidSolution()
    {
        // Arrange - Complex case from Turn 8
        var boardSet = new Set();

        // Run 1: 8, 9, 10 (Black)
        boardSet.AddTile(new Tile(8, TileColor.Black));
        boardSet.AddTile(new Tile(9, TileColor.Black));
        boardSet.AddTile(new Tile(10, TileColor.Black));

        // Run 2: 6, 7, 8, 9, 10 (Mango)
        boardSet.AddTile(new Tile(6, TileColor.Mango));
        boardSet.AddTile(new Tile(7, TileColor.Mango));
        boardSet.AddTile(new Tile(8, TileColor.Mango));
        boardSet.AddTile(new Tile(9, TileColor.Mango));
        boardSet.AddTile(new Tile(10, TileColor.Mango));

        // Group 1: 2, 2, 2 (Red, Mango, Black)
        boardSet.AddTile(new Tile(2, TileColor.Red));
        boardSet.AddTile(new Tile(2, TileColor.Mango));
        boardSet.AddTile(new Tile(2, TileColor.Black));

        // Player's rack
        var playerSet = new Set();
        playerSet.AddTile(new Tile(11, TileColor.Black));
        playerSet.AddTile(new Tile(5, TileColor.Mango));
        playerSet.AddTile(new Tile(7, TileColor.Black));
        playerSet.AddTile(new Tile(1, TileColor.Red));

        // Act
        var solver = SequentialGraphSolver.Create(boardSet, playerSet);
        var result = solver.SearchSolution();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Found);
        output.WriteLine($"Solution found with score: {result.Score}");
        output.WriteLine($"Tiles to play: {result.TilesToPlay.Count()}");
        output.WriteLine($"Jokers to play: {result.JokerToPlay}");
        output.WriteLine($"Sets in solution: {result.BestSolution.Groups.Count + result.BestSolution.Runs.Count}");
    }

    [Fact]
    public void SequentialGraphSolver_ShouldProduceSameResultAsParallelGraphSolver()
    {
        // Arrange
        var boardSet = new Set();
        boardSet.AddTile(new Tile(1, TileColor.Red));
        boardSet.AddTile(new Tile(2, TileColor.Red));
        boardSet.AddTile(new Tile(3, TileColor.Red));
        boardSet.AddTile(new Tile(4, TileColor.Red));

        boardSet.AddTile(new Tile(5));
        boardSet.AddTile(new Tile(6));
        boardSet.AddTile(new Tile(7));

        var playerSet = new Set();
        playerSet.AddTile(new Tile(8));
        playerSet.AddTile(new Tile(9));
        playerSet.AddTile(new Tile(5, TileColor.Red));

        // Act
        var parallelSolver = GraphSolver.Create(boardSet, playerSet);
        var sequentialSolver = SequentialGraphSolver.Create(boardSet, playerSet);

        var parallelResult = parallelSolver.SearchSolution();
        var sequentialResult = sequentialSolver.SearchSolution();

        // Assert
        Assert.Equal(parallelResult.Found, sequentialResult.Found);
        Assert.Equal(parallelResult.Score, sequentialResult.Score);
        Assert.Equal(parallelResult.TilesToPlay.Count(), sequentialResult.TilesToPlay.Count());
        Assert.Equal(parallelResult.JokerToPlay, sequentialResult.JokerToPlay);

        output.WriteLine("Both solvers produced same result:");
        output.WriteLine($"  Score: {sequentialResult.Score}");
        output.WriteLine($"  Tiles to play: {sequentialResult.TilesToPlay.Count()}");
        output.WriteLine($"  Valid: {sequentialResult.Found}");
    }
}