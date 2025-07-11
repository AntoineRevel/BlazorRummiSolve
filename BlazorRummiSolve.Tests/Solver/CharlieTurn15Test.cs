using RummiSolve;
using RummiSolve.Strategy;

namespace BlazorRummiSolve.Tests.Solver;

public class CharlieTurn15Test
{
    [Fact]
    public async Task Test_CharlieTurn15_ParallelSolverStrategy()
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


        var boardSet = new Set(boardTiles);
        var rack = new Set(rackTiles);

        var strategy = new CombiOnlyStrategy();

        // Act
        var result = await strategy.GetSolverResult(boardSet, rack, true);

        // Assert
        Assert.True(result.Found, "Should find a solution for Charlie turn 15");
        Assert.True(result.BestSolution.IsValid, "Solution should be valid");
        Assert.NotEmpty(result.TilesToPlay);
    }
}