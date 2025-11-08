using RummiSolve.Results;

namespace RummiSolve.Strategies;

public class GraphStrategy : IStrategy
{
    public Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}