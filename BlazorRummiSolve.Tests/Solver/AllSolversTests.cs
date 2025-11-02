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
        Assert.True(
            testCase.Expected.TilesToPlayCount == result.TilesToPlay.Count(),
            $"{solverName} - {testCase.Name}: Expected TilesToPlayCount={testCase.Expected.TilesToPlayCount}, got {result.TilesToPlay.Count()}"
        );
        Assert.True(
            testCase.Expected.JokerToPlay == result.JokerToPlay,
            $"{solverName} - {testCase.Name}: Expected JokerToPlay={testCase.Expected.JokerToPlay}, got {result.JokerToPlay}"
        );
    }
}