using RummiSolve;
using RummiSolve.Solver;

namespace BlazorRummiSolve.Tests;

public class BinarySolverTests
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

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
        };

        var solver = BinarySolver.Create(boardSet, playerTiles);

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
    public void SearchSolution_ValidJoker()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(true)
        ]);

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
        };

        var solver = BinarySolver.Create(boardSet, playerTiles);

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
    public void SearchSolution_ValidRun()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red),
        };

        var solver = BinarySolver.Create(boardSet, playerTiles);

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
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red),
            new(true)
        };

        var solver = BinarySolver.Create(boardSet, playerTiles);

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
    public void SearchSolution_Invalid()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),

            new(5)
        };

        var solver = BinarySolver.Create(boardSet, playerTiles);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }
}