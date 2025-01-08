using RummiSolve;
using RummiSolve.Solver.Combinations.First;

namespace BlazorRummiSolve.Tests;

public class CombinationsFirstSolverTests
{
    [Fact]
    public void SearchSolution_ValidMaxScore()
    {
        // Arrange
        var playerSet = new Set([
        
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
            
        ]);

        var solver = CombinationsFirstSolver.Create(playerSet);

        // Act
        solver.SearchSolution();
        var solution = solver.BestSolution;
        var tilesToPlay = solver.TilesToPlay.ToList();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(playerSet.Tiles.Count, tilesToPlay.Count);

        foreach (var tile in playerSet.Tiles)
        {
            Assert.Contains(tile, tilesToPlay);
        }
    }
}