using RummiSolve.Results;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Graph;

public class GraphSolver(Tile[] tiles, int jokers) : ISolver
{
    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Test()
    {
        var root = RummiNode.CreateRoot(tiles, jokers);
        root.GetChildren();
        root.PrintTree();
    }
}