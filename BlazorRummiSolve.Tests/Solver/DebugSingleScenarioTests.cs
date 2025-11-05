using RummiSolve;
using RummiSolve.Solver.Incremental;
using Xunit.Abstractions;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Simple test class for debugging a specific scenario with a specific solver.
///     Useful for stepping through the solver logic in the debugger.
/// </summary>
public class DebugSingleScenarioTests(ITestOutputHelper testOutputHelper)
{
    /// <summary>
    ///     Quick test to debug a specific scenario.
    ///     Change the solver and test case name as needed.
    /// </summary>
    [Fact]
    public void DebugSpecificScenario()
    {
        //var testCase = CommonTestCases.All.First(t => t.Name == "Valid");
        var testCase = CommonTestFirstCases.All.First(t => t.Name == "ThreeTilesOneJokerGroup");

        var solver = IncrementalFirstBaseSolver.Create(new Set(testCase.Player));

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.Equal(testCase.Expected.IsValid, result.BestSolution.IsValid);

        // Print result for debugging
        testOutputHelper.WriteLine($"Test: {testCase.Name}");
        testOutputHelper.WriteLine($"IsValid: {result.BestSolution.IsValid}");
        testOutputHelper.WriteLine($"Tiles to play: {result.TilesToPlay.Count()}");
        testOutputHelper.WriteLine($"Jokers to play: {result.JokerToPlay}");
        testOutputHelper.WriteLine($"Score: {result.Score}");
        testOutputHelper.WriteLine(
            $"Tiles: {string.Join(", ", result.TilesToPlay.Select(t => $"{t.Value}{t.Color}"))}");
    }
}