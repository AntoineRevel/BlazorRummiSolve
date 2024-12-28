using RummiSolve;
using RummiSolve.Solver;

namespace BlazorRummiSolve.Tests;

public class IncrementalSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(3, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(true)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(2, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }


    [Fact]
    public void SearchSolution_ValidRun()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red)
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Single(tilesToPlay);
        Assert.Equal(0, jokerToPlay);
    }
    
    [Fact]
    public void SearchSolution_ValidRunEnd()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(true)
        ]);

        var playerSet = new Set([
            new Tile(12),
            new Tile(13),
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(2,tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidNotWon()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(5),
            new Tile(5),
            
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.False(won);
        Assert.True(solution.IsValid);
        Assert.Equal(6, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }
    
    [Fact]
    public void SearchSolution_ValidWinJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(5,TileColor.Red),
            new Tile(true)
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(4, tilesToPlay.Count);
        Assert.Equal(1, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_Invalid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(5)
        ]);

        var solver = IncrementalSolver.Create(boardSet, playerSet);

        // Act
        var won = solver.SearchSolution();
        var solution = solver.BestSolution;

        // Assert
        Assert.False(won);
        Assert.False(solution.IsValid);
    }
}