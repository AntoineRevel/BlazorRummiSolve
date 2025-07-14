using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

public class IncrementalSolverStrategy : ISolverStrategy
{
    public async Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        ISolver solver = hasPlayed
            ? IncrementalComplexSolver.Create(new Set(board), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        var result = await Task.Run(() => solver.SearchSolution(cancellationToken), cancellationToken);

        stopwatch.Stop();
        Console.WriteLine($"IncrementalSolverStrategy executed in {stopwatch.ElapsedMilliseconds}ms");

        return result;
    }
}