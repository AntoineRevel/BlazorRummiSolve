using RummiSolve.Results;

namespace RummiSolve.Strategy;

public interface ISolverStrategy
{
    Task<SolverResult> GetSolverResult(Set boardSet, Set rack, bool hasPlayed,
        CancellationToken cancellationToken);
}