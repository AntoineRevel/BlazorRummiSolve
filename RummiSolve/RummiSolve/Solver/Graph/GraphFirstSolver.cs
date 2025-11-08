using RummiSolve.Results;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Graph;

public class GraphFirstSolver : ISolver
{
    private readonly int _jokers;
    private readonly Tile[] _tiles;

    private GraphFirstSolver(Tile[] tiles, int jokers)
    {
        _tiles = tiles;
        _jokers = jokers;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        var root = RummiNode.CreateRoot(_tiles, _jokers);
        root.GetChildren();
        foreach (var child in root.Children)
        {
            child.GetChildren();
            foreach (var childChild in child.Children) childChild.GetChildren();
        }


        Console.WriteLine();
        root.PrintTree();
        return SolverResult.Invalid("TestGraph");
    }

    public static GraphFirstSolver Create(Set playerSet)
    {
        playerSet.Tiles.Sort();
        return new GraphFirstSolver(
            playerSet.Tiles.ToArray(),
            playerSet.Jokers
        );
    }
}