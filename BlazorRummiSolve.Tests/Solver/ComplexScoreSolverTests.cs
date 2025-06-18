using RummiSolve;
using RummiSolve.Solver.BestScore;

namespace BlazorRummiSolve.Tests.Solver;

public class ComplexScoreSolverTests
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(30, bestScore);
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(20, bestScore);
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(4, bestScore);
    }

    [Fact]
    public void SearchSolution_ValidPlay1()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4 ,TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(1, bestScore);
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(25, bestScore);
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(36, bestScore);
    }

    [Fact]
    public void SearchSolution_ValidNWonIncrscorePlayer()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(10),
            new Tile(11),
            new Tile(12),
            new Tile(13),

            new Tile(11, TileColor.Mango),
            new Tile(true),
            new Tile(13, TileColor.Mango),
        ]);

        var playerSet = new Set([
            new Tile(9, TileColor.Mango),
            new Tile(13, TileColor.Red),
            new Tile(13),
        ]);

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(35, bestScore);
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

            new Tile(5, TileColor.Red),
            new Tile(true)
        ]);

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(35, bestScore);
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

        var solver = ComplexScoreSolver.Create(boardSet, playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.False(canPlay);
        Assert.Equal(0, bestScore);
    }
}