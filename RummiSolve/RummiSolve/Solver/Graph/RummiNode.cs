using System.Collections.Concurrent;
using RummiSolve.Solver.Abstract;

namespace RummiSolve.Solver.Graph;

public class RummiNode : BaseSolver
{
    private readonly int _gen;
    private readonly bool[] _isTileUsed;
    private readonly ValidSet _set;
    private readonly int _startIndex;
    public readonly List<RummiNode> Children = [];

    public readonly ConcurrentBag<RummiNode> LeafNodes;
    public readonly int Score;

    private RummiNode(ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startIndex, int gen,
        ConcurrentBag<RummiNode> leafNodes, int score) :
        base(tiles, jokers)
    {
        _isTileUsed = (bool[])isTileUsed.Clone();
        Array.Copy(_isTileUsed, UsedTiles, Tiles.Length);
        _set = set;
        _startIndex = startIndex;
        _gen = gen;
        LeafNodes = leafNodes;
        Score = score;
    }

    public static RummiNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiNode(new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0, 0, [], 0);
    }

    public void GetChildren()
    {
        var createdNode = 0;

        for (var i = _startIndex; i < Tiles.Length; i++)
        {
            if (_isTileUsed[i]) continue;
            UsedTiles[i] = true;
            foreach (var set in GetRuns(i).Concat(GetGroups(i)))
            {
                createdNode++;
                MarkTilesAsUsed(set, i);
                var child = new RummiNode(set, Tiles, UsedTiles, Jokers, i + 1, createdNode, LeafNodes,
                    Score + set.GetScore());
                MarkTilesAsUnused(set, i);
                Children.Add(child);
            }

            UsedTiles[i] = false;
        }

        if (createdNode == 0) LeafNodes.Add(this);
    }

    private void Print()
    {
        Console.Write($"{_gen}: ");
        _set.Print();

        Console.Write("  Unused: [ ");
        for (var i = 0; i < Tiles.Length; i++)
            if (!_isTileUsed[i])
                Tiles[i].PrintTile();
        for (var i = 0; i < Jokers; i++) Tile.PrintJoker();
        Console.Write("]");

        Console.WriteLine($" {Score}");
    }

    public void PrintTree()
    {
        PrintTreeRecursive(0);
    }

    private void PrintTreeRecursive(int depth)
    {
        Console.Write(new string(' ', depth * 2));
        Print();
        foreach (var child in Children) child.PrintTreeRecursive(depth + 1);
    }
}