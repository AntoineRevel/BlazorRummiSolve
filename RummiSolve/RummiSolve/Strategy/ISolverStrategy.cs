using RummiSolve.Results;

namespace RummiSolve.Strategy;

public interface ISolverStrategy
{
    Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default);
}