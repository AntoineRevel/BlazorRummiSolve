using RummiSolve.Solver;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class ParallelSolverStrategy : ISolverStrategy
{
    public SolverResult GetSolverResult(Solution boardSolution, Set rack, bool hasPlayed,
        CancellationToken externalToken)
    {
        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(boardSolution.GetSet(), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(boardSolution.GetSet(), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

        var incrementalTask = Task.Run(() => incrementalSolver.SearchSolution(), cts.Token);
        var combiTask = Task.Run(() => combiSolver.SearchSolution(), cts.Token);

        var completedTaskIndex = Task.WaitAny([incrementalTask, combiTask], externalToken);

        cts.Cancel();

        return completedTaskIndex == 0
            ? incrementalTask.Result
            : combiTask.Result;
    }
}