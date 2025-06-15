using RummiSolve;
using RummiSolve.Solver.Incremental;

namespace BlazorRummiSolve.Tests.Solver;

public class IncrementalScoreFieldComplexSolverTests
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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
    public void SearchSolution_ValidPlay1()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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
    public void SearchSolution_ValidRunJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(true)
        ]);

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(won);
        Assert.True(solution.IsValid);
        Assert.Single(tilesToPlay);
        Assert.Equal(1, jokerToPlay);
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
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

        var solver = IncrementalScoreFieldComplexSolver.Create(boardSet, playerSet);

        // Act
        solver.SearchSolution();
        var won = solver.Won;
        var solution = solver.BestSolution;

        // Assert
        Assert.False(won);
        Assert.False(solution.IsValid);
    }
}