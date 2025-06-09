using RummiSolve;
using RummiSolve.Solver;
using RummiSolve.Solver.Combinations;

namespace BlazorRummiSolve.Tests;

public class BinaryBaseSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var setToTry = new Tile[]
        {
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(3, TileColor.Red),
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
        };

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),
        };

        Array.Sort(setToTry);

        var solver = new BinaryBaseSolver(setToTry, 0)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = 0
        };

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
        var setToTry = new Tile[]
        {
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(3, TileColor.Red),
            new(10),
            new(10, TileColor.Red),
            // new Tile(true)
        };

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
        };

        Array.Sort(setToTry);

        var solver = new BinaryBaseSolver(setToTry, 1)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = 1
        };

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
        var setToTry = new Tile[]
        {
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(3, TileColor.Red),
            new(4, TileColor.Red),
        };

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red),
        };

        Array.Sort(setToTry);

        var solver = new BinaryBaseSolver(setToTry, 1)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = 0
        };

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
        var setToTry = new Tile[]
        {
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(3, TileColor.Red),
            new(4, TileColor.Red),
        };

        var playerTiles = new List<Tile>
        {
            new(4, TileColor.Red),
        };

        Array.Sort(setToTry);

        var solver = new BinaryBaseSolver(setToTry, 1)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = 1
        };

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(2, tilesToPlay.Count + solver.JokerToPlay);

        foreach (var tile in playerTiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }

    [Fact]
    public void SearchSolution_Invalid()
    {
        // Arrange
        var setToTry = new Tile[]
        {
            new(1, TileColor.Red),
            new(2, TileColor.Red),
            new(3, TileColor.Red),
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),

            new(5)
        };

        var playerTiles = new List<Tile>
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),

            new(5)
        };

        Array.Sort(setToTry);

        var solver = new BinaryBaseSolver(setToTry, 1)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = 0
        };

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }
}