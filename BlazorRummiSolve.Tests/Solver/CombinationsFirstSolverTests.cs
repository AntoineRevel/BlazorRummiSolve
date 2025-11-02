using RummiSolve;
using RummiSolve.Solver.Combinations.First;

namespace BlazorRummiSolve.Tests.Solver;

public class CombinationsFirstSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var solver = CombinationsFirstSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerSet.Tiles.Count, tilesToPlay.Count);

        foreach (var tile in playerSet.Tiles) Assert.Contains(tile, tilesToPlay);
    }

    [Fact]
    public void SearchSolution_ValidNoWon()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(1),

            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var solver = CombinationsFirstSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(3, tilesToPlay.Count);
    }
}