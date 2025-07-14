using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

public class IncrementalSolverStrategy : ISolverStrategy
{
    public Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        ISolver solver = hasPlayed
            ? IncrementalComplexSolver.Create(board, rack)
            : IncrementalFirstBaseSolver.Create(rack);

        var result = solver.SearchSolution(cancellationToken);

        stopwatch.Stop();
        Console.WriteLine($"IncrementalSolverStrategy executed in {stopwatch.ElapsedMilliseconds}ms");

        return Task.FromResult(result);
    }
}