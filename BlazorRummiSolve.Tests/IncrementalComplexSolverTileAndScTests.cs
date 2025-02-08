using RummiSolve;
using RummiSolve.Solver.Incremental;

namespace BlazorRummiSolve.Tests;

public class IncrementalComplexSolverTileAndScTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(5),
            new Tile(5, TileColor.Red),
            new Tile(5, TileColor.Black),
            new Tile(5, TileColor.Mango),

            new Tile(6),
            new Tile(6, TileColor.Red),
            new Tile(6, TileColor.Black),
            new Tile(6, TileColor.Mango),
        ]);

        var playerSet = new Set([
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),

            new Tile(7)
        ]);
        var solver = IncrementalComplexSolverTileAndSc.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.False(won);
        Assert.True(solution.IsValid);
        Assert.Equal(2, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    // [Fact]
    // public void SearchSolution_ValidOneJ()
    // {
    //     // Arrange
    //     var boardSet = new Set([
    //         new Tile(5),
    //         new Tile(5, TileColor.Red),
    //         new Tile(5, TileColor.Black),
    //     ]);
    //
    //     var playerSet = new Set([
    //         new Tile(true)
    //     ]);
    //     var solver = IncrementalComplexSolverTileAndSc.Create(boardSet, playerSet);
    //
    //     // Act
    //     solver.SearchSolution();
    //     var won = solver.Won;
    //     var solution = solver.BestSolution;
    //     var tilesToPlay = solver.TilesToPlay.ToList();
    //     var jokerToPlay = solver.JokerToPlay;
    //
    //     // Assert
    //     Assert.True(won);
    //     Assert.True(solution.IsValid);
    //     Assert.Empty(tilesToPlay);
    //     Assert.Equal(1, jokerToPlay);
    // }
}