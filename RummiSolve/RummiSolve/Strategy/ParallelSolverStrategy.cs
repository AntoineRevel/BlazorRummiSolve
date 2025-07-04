using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class ParallelSolverStrategy : ISolverStrategy
{
    public async Task<SolverResult> GetSolverResult(Solution boardSolution, Set rack, bool hasPlayed,
        CancellationToken externalToken)
    {
        var solutionSet = boardSolution.GetSet();

        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(new Set(solutionSet), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(new Set(solutionSet), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        var token = cts.Token;

        var incrementalTask = Task.Run(() => incrementalSolver.SearchSolution(token), token);
        var combiTask = Task.Run(() => combiSolver.SearchSolution(token), token);

        var completedTask = await Task.WhenAny(incrementalTask, combiTask);

        await cts.CancelAsync();

        return await completedTask;
    }
}