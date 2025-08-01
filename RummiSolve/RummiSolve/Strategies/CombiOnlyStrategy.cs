using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

public class CombiOnlyStrategy : ISolverStrategy
{
    public Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed, CancellationToken token)
    {
        ISolver combiSolver = hasPlayed
            ? ParallelCombinationsSolver.Create(board, rack)
            : CombinationsFirstSolver.Create(rack);

        return Task.Run(() => combiSolver.SearchSolution(token), token);
    }
}