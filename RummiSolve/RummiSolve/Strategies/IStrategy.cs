using RummiSolve.Results;

namespace RummiSolve.Strategies;

public interface IStrategy
{
    Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default);
}