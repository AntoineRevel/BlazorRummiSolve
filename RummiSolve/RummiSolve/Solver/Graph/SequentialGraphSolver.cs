using RummiSolve.Results;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Graph;

public class SequentialGraphSolver : ISolver
{
    private readonly int _boardJokers;
    private readonly int _boardTile;
    private readonly bool[] _isPlayerTile;
    private readonly int _jokers;
    private readonly Tile[] _tiles;

    private SequentialGraphSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers, int boardTile)
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
        var currentLevel = new List<RummiNode> { root };
        var leafNodes = new List<RummiNode>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var nextLevel = new List<RummiNode>();

            foreach (var node in currentLevel)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var isLeaf = node.GetChildren();
                if (isLeaf)
                    leafNodes.Add(node);
                else
                    foreach (var child in node.Children)
                        nextLevel.Add(child);
            }

            if (nextLevel.Count == 0) break;
            currentLevel = nextLevel;
        }

        if (leafNodes.Count == 0) return SolverResult.Invalid("SequentialGraphSolver");

        var bestNode = leafNodes.MaxBy(node => (node.PlayerTilePlayed, node.Score));

        var bestSolution = bestNode!.GetSolution();

        var tilesToPlay = _tiles.Where((_, i) => bestNode.IsTileUsed[i] && _isPlayerTile[i]).ToArray();
        var jokerToPlay = _jokers - bestNode.Jokers - _boardJokers;

        return SolverResult.FromSolution("SequentialGraphSolver", bestSolution, tilesToPlay, jokerToPlay,
            bestNode.Score);
    }

    public static SequentialGraphSolver Create(Set boardSet, Set playerSet)
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

        return new SequentialGraphSolver(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers,
            boardTile
        );
    }
}