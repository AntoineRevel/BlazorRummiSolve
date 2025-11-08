using RummiSolve.Results;

namespace RummiSolve.Strategies;

public class HumanPlayerStrategy(Func<Set, bool, CancellationToken, Task<SolverResult>> getPlayerChoice)
    : IStrategy
{
    public async Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        return await getPlayerChoice(board, hasPlayed, cancellationToken);
    }
}