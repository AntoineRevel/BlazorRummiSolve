using RummiSolve;
using RummiSolve.Solver.Combinations.First;

namespace BlazorRummiSolve.Tests.Solver;

public class BinaryFirstBaseSolverTests
{
    [Fact]
    public void SearchSolution_Valid()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 0);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_ValidMaxScore()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black),

            new(1),
            new(2),
            new(3)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 0);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_ValidGroupJoker()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(10),
            new(10, TileColor.Red)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_ValidGroupMaxScoreJoker()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(10),
            new(10, TileColor.Red),
            new(10, TileColor.Black)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_ValidRunJoker()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(9),
            new(10)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_InvalidRunJoker()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(8),
            new(9)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

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
        var playerTiles = new Tile[]
        {
            new(1),
            new(2),
            new(3)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 0);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }


    [Fact]
    public void SearchSolution_InvalidGroupJoker()
    {
        // Arrange
        var playerTiles = new Tile[]
        {
            new(9),
            new(9, TileColor.Red),
            new(true)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

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
        var playerTiles = new Tile[]
        {
            new((byte)26),
            new((byte)36),
            new((byte)43),
            new((byte)45),
            new((byte)57),
            new((byte)58),
            new((byte)59)
        };

        var solver = new BinaryFirstBaseSolver(playerTiles, 1);

        // Act
        solver.SearchSolution();
        var solution = solver.BinarySolution;

        // Assert
        Assert.False(solution.IsValid);
    }
}