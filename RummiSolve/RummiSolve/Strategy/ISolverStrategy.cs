using RummiSolve.Results;

namespace RummiSolve.Strategy;

public interface ISolverStrategy
{
    Task<SolverResult> GetSolverResult(Solution boardSolution, Set rack, bool hasPlayed,
        CancellationToken cancellationToken);
}