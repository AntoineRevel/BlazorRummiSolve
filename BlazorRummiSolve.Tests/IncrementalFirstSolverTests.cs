using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class IncrementalFirstSolverTests
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

        var solver = IncrementalFirstSolver.Create(playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(3, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }
    
    [Fact]
    public void SearchSolution_ValidMaxScore()
    {
        // Arrange
        var playerSet = new Set([
        
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),

            new Tile(1),
            new Tile(2),
            new Tile(3),
        ]);

        var solver = IncrementalFirstSolver.Create(playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(6, tilesToPlay.Count);
        Assert.Equal(0, jokerToPlay);
    }
    
    [Fact]
    public void SearchSolution_ValidGroupJoker()
    {
        // Arrange
        var playerSet = new Set([
        
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(true)
        ]);

        var solver = IncrementalFirstSolver.Create(playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();
        var jokerToPlay = solver.JokerToPlay;

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(2, tilesToPlay.Count);
        Assert.Equal(1, jokerToPlay);
    }
    
}