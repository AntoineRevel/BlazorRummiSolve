using RummiSolve.Solver;

namespace RummiSolve.Strategy;

public interface ISolverStrategy
{
    SolverResult GetSolverResult(Solution boardSolution, Set rack, bool hasPlayed, CancellationToken cancellationToken);
}