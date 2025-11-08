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

        foreach (var child in root.Children)
        {
            child.GetChildren();
            foreach (var childChild in child.Children) childChild.GetChildren();
        }


        Console.WriteLine();
        root.PrintTree();
    }
}