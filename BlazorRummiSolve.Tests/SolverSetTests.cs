using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class SolverSetTests
{
    [Fact]
    public void GetFirstSolution_ReturnsValidSolutionIfPossible()
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

        // Assert
        Assert.True(solution.IsValid);
    }
}