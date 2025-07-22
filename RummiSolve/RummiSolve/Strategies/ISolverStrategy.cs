using RummiSolve.Results;

namespace RummiSolve.Strategies;

public interface ISolverStrategy
{
    Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default);
}