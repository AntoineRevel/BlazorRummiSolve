using RummiSolve;
using RummiSolve.Solver;

namespace BlazorRummiSolve.Tests;

public class ScoreFirstBaseSolverTests
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

        var solver = ScoreFirstBaseSolver.Create(playerSet);

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
        var playerSet = new Set([
            new Tile(1),
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(true)
        ]);

        var solver = ScoreFirstBaseSolver.Create(playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(30, bestScore);
    }

    [Fact]
    public void SearchSolution_InvalidRun()
    {
        var playerSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Red),
            new Tile(7, TileColor.Red),
        ]);

        var solver = ScoreFirstBaseSolver.Create(playerSet);

        // Act
        var canPlay = solver.SearchBestScore();

        // Assert
        Assert.False(canPlay);
    }

    [Fact]
    public void SearchSolution_ValidRun()
    {
        var playerSet = new Set([
            new Tile(1),
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Red),
            new Tile(7, TileColor.Red),
            new Tile(8, TileColor.Red),
        ]);

        var solver = ScoreFirstBaseSolver.Create(playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(36, bestScore);
    }

    [Fact]
    public void SearchSolution_ValidRunEnd()
    {
        // Arrange

        var playerSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(true),
            new Tile(12),
            new Tile(13),
        ]);

        var solver = ScoreFirstBaseSolver.Create(playerSet);

        // Act
        var canPlay = solver.SearchBestScore();
        var bestScore = solver.BestScore;

        // Assert
        Assert.True(canPlay);
        Assert.Equal(39, bestScore);
    }


    [Fact]
    public void SearchSolution_Cantplay()
    {

        // Arrange
        var playerSet = new Set([
            new Tile(9, TileColor.Mango),
            new Tile(13, TileColor.Red),
            new Tile(13),
        ]);

        var solver = ScoreFirstBaseSolver.Create(playerSet);

        // Act
        var canPlay = solver.SearchBestScore();


        // Assert
        Assert.False(canPlay);

    }
}