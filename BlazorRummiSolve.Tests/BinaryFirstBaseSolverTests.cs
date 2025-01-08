using RummiSolve;
using RummiSolve.Solver;

namespace BlazorRummiSolve.Tests;

public class BinaryFirstBaseSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerTiles.Count, tilesToPlay.Count);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_ValidMaxScore()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),

            new(1),
            new(2),
            new(3),
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerTiles.Count, tilesToPlay.Count);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_ValidGroupJoker()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(true)
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerTiles.Count, tilesToPlay.Count);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_ValidGroupMaxScoreJoker()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
            new(true)
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerTiles.Count, tilesToPlay.Count);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_ValidRunJoker()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(9),
            new(10),
            new(true)
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerTiles.Count, tilesToPlay.Count);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_InvalidRunJoker()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(8),
            new(9),
            new(true)
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_Invalid()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(1),
            new(2),
            new(3),
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }


    [Fact]
    public void SearchSolution_InvalidJoker()
    {
        // Arrange
        var playerTiles = new List<Tile>
        {
            new(9),
            new(9, TileColor.Red),
            new(true)
        };

        var solver = BinaryFirstBaseSolver.Create(playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }
}