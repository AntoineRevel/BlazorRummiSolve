using RummiSolve;
using RummiSolve.Solver.BestScore.First;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Graph;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;
using Xunit.Abstractions;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests all First-turn solvers with all common test cases.
///     First-turn solvers have special rules: no board, higher minimum score requirements.
///     To add a solver, add it to the Solvers list.
/// </summary>
public class AllFirstSolversTests(ITestOutputHelper output)
{
    /// <summary>
    ///     List of First-turn solvers to test. Add or remove solvers here.
    ///     Note: ScoreFirstBaseSolver is excluded as it implements IScoreSolver, not ISolver
    /// </summary>
    private static readonly (string Name, Func<Set, ISolver> Create)[] Solvers =
    [
        ("CombinationsFirstSolver", CombinationsFirstSolver.Create),
        ("BestScoreFirstBaseSolver", BestScoreFirstBaseSolver.Create),
        ("IncrementalFirstBaseSolver", IncrementalFirstBaseSolver.Create),
        ("GraphFirstSolver", GraphSolver.CreateFirst)
    ];

    /// <summary>
    ///     Generate test data: all combinations of (solver, test case)
    /// </summary>
    public static TheoryData<string, Func<Set, ISolver>, CommonTestFirstCases.TestCase> GetTestData()
    {
        var data = new TheoryData<string, Func<Set, ISolver>, CommonTestFirstCases.TestCase>();

        foreach (var (solverName, solverCreate) in Solvers)
        foreach (var testCase in CommonTestFirstCases.All)
            data.Add(solverName, solverCreate, testCase);

        return data;
    }

    [Theory]
    [MemberData(nameof(GetTestData), DisableDiscoveryEnumeration = true)]
    public void AllFirstSolvers_AllTestCases_ShouldReturnExpectedResult(
        string solverName,
        Func<Set, ISolver> createSolver,
        CommonTestFirstCases.TestCase testCase)
    {
        // Arrange - Create a new Set instance for each test to avoid mutation
        var playerSet = new Set(testCase.Player);
        var solver = createSolver(playerSet);

        // Act
        var result = solver.SearchSolution();

        // Assert
        SolverTestHelpers.AssertSolverResult(solverName, testCase.Name, result, testCase.Expected, output);
    }
}