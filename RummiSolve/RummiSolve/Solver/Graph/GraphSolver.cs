using System.Collections.Concurrent;
using RummiSolve.Results;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Graph;

public class GraphSolver : ISolver
{
    private readonly int _boardJokers;
    private readonly int _boardTile;
    private readonly bool[] _isPlayerTile;
    private readonly int _jokers;
    private readonly Tile[] _tiles;

    private GraphSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers, int boardTile)
    {
        _tiles = tiles;
        _jokers = jokers;
        _isPlayerTile = isPlayerTile;
        _boardJokers = boardJokers;
        _boardTile = boardTile;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        var root = RummiNode.CreateRoot(_tiles, _jokers, _isPlayerTile, _boardTile, _boardJokers);
        var currentLevel = new ConcurrentBag<RummiNode> { root };

        var level = 0; //debug

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextLevel = new ConcurrentBag<RummiNode>();

            level++;

            Parallel.ForEach(currentLevel,
                new ParallelOptions { CancellationToken = cancellationToken },
                node =>
                {
                    node.GetChildren();
                    foreach (var child in node.Children) nextLevel.Add(child);
                }
            );

            Console.WriteLine(level);
            if (nextLevel.IsEmpty) break;
            currentLevel = nextLevel;
        }

        root.PrintTree();

        if (root.LeafNodes.IsEmpty) return SolverResult.Invalid("GraphFirstSolver");

        var bestNode = root.LeafNodes.MaxBy(node => (node.PlayerTilePlayed, node.Score));

        var bestSolution = bestNode!.GetSolution();

        var tilesToPlay = _tiles.Where((_, i) => bestNode.IsTileUsed[i] && _isPlayerTile[i]).ToArray();
        var jokerToPlay = _jokers - bestNode.Jokers - _boardJokers;

        return SolverResult.FromSolution("GraphFirstSolver", bestSolution, tilesToPlay, jokerToPlay, bestNode.Score);
    }

    public static GraphSolver Create(Set boardSet, Set playerSet)
    {
        var boardTile = boardSet.Tiles.Count;
        var capacity = boardSet.Tiles.Count + playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(boardSet.Tiles.Select(tile => (tile, false)));
        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = boardSet.Jokers + playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new GraphSolver(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers,
            boardTile
        );
    }
}