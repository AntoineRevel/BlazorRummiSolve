using RummiSolve;
using RummiSolve.Solver.BestScore;
using RummiSolve.Solver.Combinations;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Simple test class for debugging a specific scenario with a specific solver.
///     Useful for stepping through the solver logic in the debugger.
/// </summary>
public class DebugSingleScenarioTests
{
    /// <summary>
    ///     Quick test to debug a specific scenario.
    ///     Change the solver and test case name as needed.
    /// </summary>
    [Fact]
    public void DebugSpecificScenario()
    {
        // ===== CONFIGURE YOUR TEST HERE =====

        // Choose a test case from CommonTestCases by name
        var testCase = CommonTestCases.All.First(t => t.Name == "Valid");

        // Choose a solver to test
        // Options: CombinationsSolver, ParallelCombinationsSolver, BestScoreComplexSolver,
        //          IncrementalComplexSolver, IncrementalScoreFieldComplexSolver, IncrementalComplexSolverTileAndSc
        var solver = CombinationsSolver.Create(new Set(testCase.Board), new Set(testCase.Player));

        // ===== END CONFIGURATION =====

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.Equal(testCase.Expected.IsValid, result.BestSolution.IsValid);

        // Print result for debugging
        Console.WriteLine($"Test: {testCase.Name}");
        Console.WriteLine($"IsValid: {result.BestSolution.IsValid}");
        Console.WriteLine($"Tiles to play: {result.TilesToPlay.Count()}");
        Console.WriteLine($"Jokers to play: {result.JokerToPlay}");
        Console.WriteLine($"Score: {result.Score}");
        Console.WriteLine($"Tiles: {string.Join(", ", result.TilesToPlay.Select(t => $"{t.Value}{t.Color}"))}");
    }

    /// <summary>
    ///     Debug with a custom scenario (not from CommonTestCases).
    ///     Useful for testing specific edge cases or bugs.
    /// </summary>
    [Fact]
    public void DebugCustomScenario()
    {
        // ===== CONFIGURE YOUR CUSTOM SCENARIO HERE =====

        var board = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var player = new Set([
            new Tile(4, TileColor.Red)
        ]);

        // Choose a solver
        var solver = BestScoreComplexSolver.Create(board, player);

        // ===== END CONFIGURATION =====

        // Act
        var result = solver.SearchSolution();

        // Assert and Debug Output
        Console.WriteLine($"IsValid: {result.BestSolution.IsValid}");
        Console.WriteLine($"Tiles to play: {result.TilesToPlay.Count()}");
        Console.WriteLine($"Jokers to play: {result.JokerToPlay}");
        Console.WriteLine($"Score: {result.Score}");
        Console.WriteLine($"Solution groups: {result.BestSolution.Groups.Count}");
        Console.WriteLine($"Solution runs: {result.BestSolution.Runs.Count}");

        foreach (var group in result.BestSolution.Groups)
            Console.WriteLine(
                $"  Group: {string.Join(", ", group.Tiles.Select(t => t.IsJoker ? "J" : $"{t.Value}{t.Color}"))}");

        foreach (var run in result.BestSolution.Runs)
            Console.WriteLine(
                $"  Run: {string.Join(", ", run.Tiles.Select(t => t.IsJoker ? "J" : $"{t.Value}{t.Color}"))}");
    }
}