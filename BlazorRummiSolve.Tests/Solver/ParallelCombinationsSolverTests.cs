using RummiSolve;
using RummiSolve.Results;
using RummiSolve.Solver.Combinations;

namespace BlazorRummiSolve.Tests.Solver;

public class ParallelCombinationsSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(3, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidPlay1()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Single(tilesToPlay);
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
            new Tile(10, TileColor.Red)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

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
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Single(tilesToPlay);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidRunJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(true)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Single(tilesToPlay);
        Assert.Equal(1, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidNotWon()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(5),
            new Tile(5),

            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.False(won);
        Assert.True(solution.IsValid);
        Assert.Equal(6, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidNotWon2()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(9),
            new Tile(10),
            new Tile(11)
        ]);

        var playerSet = new Set([
            new Tile(8, TileColor.Black),
            new Tile(8, TileColor.Red),

            new Tile(1)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.False(won);
        Assert.True(solution.IsValid);
        Assert.Equal(2, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
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
            new Tile(13, TileColor.Mango)
        ]);

        var playerSet = new Set([
            new Tile(9, TileColor.Mango),
            new Tile(13, TileColor.Red),
            new Tile(13)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(3, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidWinJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(5, TileColor.Red),
            new Tile(true)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Equal(4, tilesToPlay.Count);
        Assert.Equal(1, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_ValidOneJ()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(5),
            new Tile(5, TileColor.Red),
            new Tile(5, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(true)
        ]);
        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;
        var tilesToPlay = result.TilesToPlay.ToList();
        var jokerToPlay = result.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Empty(tilesToPlay);
        Assert.Equal(1, jokerToPlay);
    }

    [Fact]
    public void SearchSolution_Invalid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(5)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();
        var won = result.Won;
        var solution = result.BestSolution;

        // Assert
        Assert.False(won);
        Assert.False(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_WithCancellation()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
            new Tile(11),
            new Tile(11, TileColor.Red),
            new Tile(11, TileColor.Black),
            new Tile(12),
            new Tile(12, TileColor.Red),
            new Tile(12, TileColor.Black)
        ]);

        var solver = ParallelCombinationsSolver.Create(boardSet, playerSet);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        // Act
        var result = solver.SearchSolution(cts.Token);

        // Assert
        Assert.IsType<SolverResult>(result);
    }
}