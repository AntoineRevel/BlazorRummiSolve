using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class TimeoutTestGeneratorStrategy : ISolverStrategy
{
    private readonly TimeSpan _timeout;

    public TimeoutTestGeneratorStrategy(TimeSpan? timeout = null)
    {
        _timeout = timeout ?? TimeSpan.FromSeconds(10);
    }

    public async Task<SolverResult> GetSolverResult(Set boardSet, Set rack, bool hasPlayed,
        CancellationToken externalToken = default)
    {
        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(new Set(boardSet), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(new Set(boardSet), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        using var timeoutCts = new CancellationTokenSource(_timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken, timeoutCts.Token);
        var token = linkedCts.Token;

        var incrementalTask = Task.Run(() => incrementalSolver.SearchSolution(token), token);
        //var combiTask = Task.Run(() => combiSolver.SearchSolution(token), token);

        try
        {
            var completedTask = await Task.WhenAny(incrementalTask); //, combiTask);
            await linkedCts.CancelAsync();
            return await completedTask;
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            return new SolverResult("TimeoutTestGenerator - Timeout exceeded, test case generated");
        }
    }
}