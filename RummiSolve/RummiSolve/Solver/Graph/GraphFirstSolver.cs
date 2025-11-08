using System.Collections.Concurrent;
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
        var currentLevel = new ConcurrentBag<RummiNode> { root };

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextLevel = new ConcurrentBag<RummiNode>();

            Parallel.ForEach(currentLevel,
                new ParallelOptions { CancellationToken = cancellationToken },
                node =>
                {
                    node.GetChildren();
                    foreach (var child in node.Children) nextLevel.Add(child);
                }
            );
            if (nextLevel.IsEmpty) break;
            currentLevel = nextLevel;
        }

        if (root.LeafNodes.IsEmpty) return SolverResult.Invalid("GraphFirstSolver");

        var bestNode = root.LeafNodes.OrderByDescending(node => node.Score).First();

        if (bestNode.Score <= ISolver.MinScore) return SolverResult.Invalid("GraphFirstSolver<30");

        var bestSolution = bestNode.GetSolution();

        var tilesToPlay = _tiles.Where((_, i) => bestNode.IsTileUsed[i]).ToArray();
        var jokerToPlay = _jokers - bestNode.Jokers;

        return SolverResult.FromSolution("GraphFirstSolver", bestSolution, tilesToPlay, jokerToPlay, bestNode.Score);
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