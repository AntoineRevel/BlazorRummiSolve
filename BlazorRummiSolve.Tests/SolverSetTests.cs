using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class SolverSetTests
{
    [Fact]
    public void GetSolution_Full()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(true),
            new Tile(10),
            
        ]);
        
        var playerSet = new Set([

            new Tile(11)
        ]);

        // Act
        var solver = SolverSet.Create(boardSet, playerSet);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;
        
        // Assert
        Assert.True(solution.IsValid);
        var t = Assert.Single(tileToPlay);
        Assert.Equal(11, t.Value);
    }
    
    [Fact]
    public void GetSolution_1()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(true),
            new Tile(10),
        ]);
        
        var playerSet = new Set([

            new Tile(11),
            new Tile(13)
        ]);

        // Act
        var solver = SolverSet.Create(boardSet, playerSet);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;

        // Assert
        Assert.True(solution.IsValid);
        var t = Assert.Single(tileToPlay);
        Assert.Equal(11, t.Value);
    }
    
    [Fact]
    public void GetSolution_First()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(9, TileColor.Blue, true),
            new Tile(10),
        ]);
        
        var playerSet = new Set([

            new Tile(11),
            new Tile(12),
            new Tile(13)
        ]);

        // Act
        var solver = SolverSet.Create(boardSet, playerSet, true);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void SearchSolution_First()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(4, TileColor.Red),
        ]);
        
        // Act
        var solver = SolverSet.Create(boardSet, playerSet);
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tileToPlay = solver.TilesToPlay;
        
        // Assert
        Assert.True(solution.IsValid);
        var t = Assert.Single(tileToPlay);
        Assert.Equal(4, t.Value);
    }

}