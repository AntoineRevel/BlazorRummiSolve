using RummiSolve.Results;

namespace RummiSolve.Solver.Interfaces;

public interface IBinarySolver
{
    SolverResult SearchSolution(CancellationToken cancellationToken = default);
}