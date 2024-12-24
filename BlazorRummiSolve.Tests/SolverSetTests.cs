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
        var solution = SolverSet.Create(boardSet, playerSet).GetSolution();

        // Assert
        Assert.True(solution.IsValid);
    }
    
    [Fact]
    public void GetSolution_1()
    {
        // Arrange
        var boardSet = new Set([
            new Tile(8),
            new Tile(9, TileColor.Blue, true),
            new Tile(10),
        ]);
        
        var playerSet = new Set([

            new Tile(11),
            new Tile(13)
        ]);

        // Act
        var solution = SolverSet.Create(boardSet, playerSet).GetSolution();

        // Assert
        Assert.True(solution.IsValid);
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
        var solution = SolverSet.Create(boardSet, playerSet,true).GetSolution();

        // Assert
        Assert.True(solution.IsValid);
    }
}