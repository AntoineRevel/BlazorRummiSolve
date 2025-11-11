using RummiSolve.Results;

namespace RummiSolve.Solver.Interfaces;

public interface ISolver
{
    const int MinScore = 30;
    SolverResult SearchSolution(CancellationToken cancellationToken = default);
}