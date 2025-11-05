using RummiSolve;
using RummiSolve.Results;
using Xunit.Abstractions;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Shared helper methods for solver tests to avoid code duplication
/// </summary>
public static class SolverTestHelpers
{
    /// <summary>
    ///     Validates that the solver result matches the expected result.
    ///     Checks IsValid, tiles to play (with multiplicity), and jokers to play.
    /// </summary>
    public static void AssertSolverResult(
        string solverName,
        string testName,
        SolverResult result,
        CommonTestCases.ExpectedResult expected,
        ITestOutputHelper? output = null)
    {
        // Check IsValid
        Assert.True(
            expected.IsValid == result.BestSolution.IsValid,
            $"{solverName} - {testName}: Expected IsValid={expected.IsValid}, got {result.BestSolution.IsValid}"
        );

        // Compare TilesToPlay (order-insensitive comparison with multiplicity check)
        var expectedTiles = expected.TilesToPlay.ToList();
        var actualTiles = result.TilesToPlay.ToList();

        Assert.True(
            expectedTiles.Count == actualTiles.Count,
            $"{solverName} - {testName}: Expected {expectedTiles.Count} tiles to play, got {actualTiles.Count}"
        );

        // Group tiles by their properties to compare multiplicities
        var expectedGroups = expectedTiles
            .GroupBy(t => new { t.Value, t.Color, t.IsJoker })
            .ToDictionary(g => g.Key, g => g.Count());

        var actualGroups = actualTiles
            .GroupBy(t => new { t.Value, t.Color, t.IsJoker })
            .ToDictionary(g => g.Key, g => g.Count());

        // Check that both have the same tile groups
        foreach (var expectedGroup in expectedGroups)
        {
            Assert.True(
                actualGroups.ContainsKey(expectedGroup.Key),
                $"{solverName} - {testName}: Expected tile [{expectedGroup.Key.Value}, {expectedGroup.Key.Color}, IsJoker={expectedGroup.Key.IsJoker}] not found in actual tiles"
            );

            Assert.True(
                actualGroups[expectedGroup.Key] == expectedGroup.Value,
                $"{solverName} - {testName}: Expected {expectedGroup.Value}x tile [{expectedGroup.Key.Value}, {expectedGroup.Key.Color}, IsJoker={expectedGroup.Key.IsJoker}], got {actualGroups[expectedGroup.Key]}x"
            );
        }

        // Check for unexpected tiles in actual results
        foreach (var actualGroup in actualGroups)
            Assert.True(
                expectedGroups.ContainsKey(actualGroup.Key),
                $"{solverName} - {testName}: Unexpected tile [{actualGroup.Key.Value}, {actualGroup.Key.Color}, IsJoker={actualGroup.Key.IsJoker}] found {actualGroup.Value}x in actual tiles"
            );

        // Check JokerToPlay
        Assert.True(
            expected.JokerToPlay == result.JokerToPlay,
            $"{solverName} - {testName}: Expected JokerToPlay={expected.JokerToPlay}, got {result.JokerToPlay}"
        );

        // Output diagnostic information if an output helper is provided
        if (output == null) return;
        {
            output.WriteLine($"✓ {solverName} - {testName}");
            output.WriteLine($"  IsValid: {result.BestSolution.IsValid}");
            output.WriteLine($"  Tiles to play: {actualTiles.Count}");
            output.WriteLine($"  Jokers to play: {result.JokerToPlay}");
            output.WriteLine($"  Score: {result.Score}");
            output.WriteLine($"  Source: {result.Source}");

            if (actualTiles.Count > 0)
                output.WriteLine(
                    $"  Tiles played: {string.Join(", ", actualTiles.Select(t => t.IsJoker ? "J" : $"{t.Value}{GetColorShort(t.Color)}"))}");
        }
    }

    private static string GetColorShort(TileColor color)
    {
        return color switch
        {
            TileColor.Red => "R",
            TileColor.Black => "B",
            TileColor.Blue => "Bl",
            TileColor.Mango => "M",
            _ => "?"
        };
    }
}