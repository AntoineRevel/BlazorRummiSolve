using RummiSolve;
using RummiSolve.Solver.BestScore;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests all solvers with all common test cases.
///     To add a solver, add it to the Solvers list.
/// </summary>
public class AllSolversTests
{
    /// <summary>
    ///     List of solvers to test. Add or remove solvers here.
    /// </summary>
    private static readonly (string Name, Func<Set, Set, ISolver> Create)[] Solvers =
    [
        ("CombinationsSolver", CombinationsSolver.Create),
        ("ParallelCombinationsSolver", ParallelCombinationsSolver.Create),
        ("BestScoreComplexSolver", BestScoreComplexSolver.Create),
        ("IncrementalComplexSolver", IncrementalComplexSolver.Create),
        ("IncrementalScoreFieldComplexSolver", IncrementalScoreFieldComplexSolver.Create),
        ("IncrementalComplexSolverTileAndSc", IncrementalComplexSolverTileAndSc.Create)
    ];

    /// <summary>
    ///     Generate test data: all combinations of (solver, test case)
    /// </summary>
    public static TheoryData<string, Func<Set, Set, ISolver>, CommonTestCases.TestCase> GetTestData()
    {
        var data = new TheoryData<string, Func<Set, Set, ISolver>, CommonTestCases.TestCase>();

        foreach (var (solverName, solverCreate) in Solvers)
        foreach (var testCase in CommonTestCases.All)
            data.Add(solverName, solverCreate, testCase);

        return data;
    }

    [Theory]
    [MemberData(nameof(GetTestData), DisableDiscoveryEnumeration = true)]
    public void AllSolvers_AllTestCases_ShouldReturnExpectedResult(
        string solverName,
        Func<Set, Set, ISolver> createSolver,
        CommonTestCases.TestCase testCase)
    {
        // Arrange - Create new Set instances for each test to avoid mutation
        var boardSet = new Set(testCase.Board);
        var playerSet = new Set(testCase.Player);
        var solver = createSolver(boardSet, playerSet);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(
            testCase.Expected.IsValid == result.BestSolution.IsValid,
            $"{solverName} - {testCase.Name}: Expected IsValid={testCase.Expected.IsValid}, got {result.BestSolution.IsValid}"
        );

        // Compare TilesToPlay (order-insensitive comparison with multiplicity check)
        var expectedTiles = testCase.Expected.TilesToPlay.ToList();
        var actualTiles = result.TilesToPlay.ToList();

        Assert.True(
            expectedTiles.Count == actualTiles.Count,
            $"{solverName} - {testCase.Name}: Expected {expectedTiles.Count} tiles to play, got {actualTiles.Count}"
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
                $"{solverName} - {testCase.Name}: Expected tile [{expectedGroup.Key.Value}, {expectedGroup.Key.Color}, IsJoker={expectedGroup.Key.IsJoker}] not found in actual tiles"
            );

            Assert.True(
                actualGroups[expectedGroup.Key] == expectedGroup.Value,
                $"{solverName} - {testCase.Name}: Expected {expectedGroup.Value}x tile [{expectedGroup.Key.Value}, {expectedGroup.Key.Color}, IsJoker={expectedGroup.Key.IsJoker}], got {actualGroups[expectedGroup.Key]}x"
            );
        }

        // Check for unexpected tiles in actual results
        foreach (var actualGroup in actualGroups)
            Assert.True(
                expectedGroups.ContainsKey(actualGroup.Key),
                $"{solverName} - {testCase.Name}: Unexpected tile [{actualGroup.Key.Value}, {actualGroup.Key.Color}, IsJoker={actualGroup.Key.IsJoker}] found {actualGroup.Value}x in actual tiles"
            );

        Assert.True(
            testCase.Expected.JokerToPlay == result.JokerToPlay,
            $"{solverName} - {testCase.Name}: Expected JokerToPlay={testCase.Expected.JokerToPlay}, got {result.JokerToPlay}"
        );
    }
}