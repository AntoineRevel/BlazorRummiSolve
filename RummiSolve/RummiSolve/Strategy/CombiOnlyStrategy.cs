using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class CombiOnlyStrategy : ISolverStrategy
{
    public Task<SolverResult> GetSolverResult(Set boardSet, Set rack, bool hasPlayed, CancellationToken token = default)
    {
        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(boardSet, rack)
            : CombinationsFirstSolver.Create(rack);

        return Task.Run(() => combiSolver.SearchSolution(token), token);
    }
}