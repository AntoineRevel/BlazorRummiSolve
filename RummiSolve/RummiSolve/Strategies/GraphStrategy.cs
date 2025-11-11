using RummiSolve.Results;
using RummiSolve.Solver.Graph;
using RummiSolve.Solver.Graph.First;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

public class GraphStrategy : IStrategy
{
    public Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        ISolver graphSolver = hasPlayed
            ? GraphSolver.Create(board, rack)
            : GraphFirstSolver.Create(rack);

        return Task.Run(() => graphSolver.SearchSolution(cancellationToken), cancellationToken);
    }
}