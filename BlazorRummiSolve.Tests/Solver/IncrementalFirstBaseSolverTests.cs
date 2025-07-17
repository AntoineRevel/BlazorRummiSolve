using RummiSolve;
using RummiSolve.Solver.Incremental;

namespace BlazorRummiSolve.Tests.Solver;

public class IncrementalFirstBaseSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
        ]);

        var solver = IncrementalFirstBaseSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(3, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidMaxScore()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(1),
            new Tile(2),
            new Tile(3),
        ]);

        var solver = IncrementalFirstBaseSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(6, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidGroupJoker()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(true)
        ]);

        var solver = IncrementalFirstBaseSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(2, tilesToPlay.Count);
        Assert.Equal(1, jokerToPlay);
    }


    [Fact]
    public void SearchSolution_ValidGroupAndJoker()
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
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(6, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ExFromBug()
    {
        // Arrange
        var playerSet = new Set([
            new Tile(3, TileColor.Mango),
            new Tile(4, TileColor.Black),
            new Tile(12, TileColor.Red),
            new Tile(1),
            new Tile(4, TileColor.Black),
            new Tile(1, TileColor.Black),
            new Tile(8, TileColor.Black),
            new Tile(13, TileColor.Black),
            new Tile(12, TileColor.Red),
            new Tile(11, TileColor.Red),
            new Tile(5, TileColor.Red),
            new Tile(10),
            new Tile(3, TileColor.Black),
            new Tile(1, TileColor.Mango),
            new Tile(4, TileColor.Red),
            new Tile(8, TileColor.Mango),
            new Tile(4, TileColor.Red),
            new Tile(2, TileColor.Black),
            new Tile(9),
            new Tile(12),
            new Tile(1, TileColor.Red),
            new Tile(9, TileColor.Red),
            new Tile(2),
            new Tile(12, TileColor.Black)
        ]);

        var solver = IncrementalFirstBaseSolver.Create(playerSet);

        // Act
        var result = solver.SearchSolution();
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(solution.GetSet().Tiles.Count, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }
}