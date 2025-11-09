using System.Collections.Concurrent;
using RummiSolve.Solver.Abstract;

namespace RummiSolve.Solver.Graph;

public class RummiNode : BaseSolver
{
    private readonly int _gen;
    private readonly bool[] _isPlayerTile;
    private readonly bool _isRun;
    private readonly RummiNode? _parentNode;
    private readonly bool _played;
    private readonly ValidSet _set;
    private readonly int _startIndex;

    public readonly List<RummiNode> Children = [];
    public readonly bool[] IsTileUsed;
    public readonly ConcurrentBag<RummiNode> LeafNodes;
    public readonly int Score;


    private RummiNode(ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startIndex, int gen,
        ConcurrentBag<RummiNode> leafNodes, int score, RummiNode? parentNode, bool isRun, bool[] isPlayerTile,
        bool played) :
        base(tiles, jokers)
    {
        IsTileUsed = (bool[])isTileUsed.Clone();
        Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
        _set = set;
        _startIndex = startIndex;
        _gen = gen;
        LeafNodes = leafNodes;
        Score = score;
        _parentNode = parentNode;
        _isRun = isRun;
        _isPlayerTile = isPlayerTile;
        _played = played;
    }

    public static RummiNode CreateRoot(Tile[] tiles, int jokers, bool[] isPlayerTile)
    {
        return new RummiNode(new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0, 0, [], 0, null, false,
            isPlayerTile, false);
    }

    public void GetChildren()
    {
        var createdNode = 0;

        for (var i = _startIndex; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;

            UsedTiles[i] = true;
            foreach (var set in GetRuns(i)) CreateChildNode(ref createdNode, i, set, true);
            foreach (var set in GetGroups(i)) CreateChildNode(ref createdNode, i, set, false);
            UsedTiles[i] = false;
        }

        if (createdNode == 0 && _played) LeafNodes.Add(this);
    }

    private void CreateChildNode(ref int createdNode, int index, ValidSet set, bool isRun)
    {
        createdNode++;
        var played = _played;
        MarkTilesAsUsed(set, index, ref played);
        var child = new RummiNode(set, Tiles, UsedTiles, Jokers, index + 1, createdNode, LeafNodes,
            Score + set.GetScore(), this, isRun, _isPlayerTile, played);
        MarkTilesAsUnused(set, index);
        Children.Add(child);
    }

    private void MarkTilesAsUsed(ValidSet set, int unusedIndex, ref bool played)
    {
        unusedIndex++;
        for (var tIndex = 1; tIndex < set.Tiles.Length; tIndex++)
        {
            var tile = set.Tiles[tIndex];
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (; unusedIndex < Tiles.Length; unusedIndex++)
            {
                if (UsedTiles[unusedIndex] || !Tiles[unusedIndex].Equals(tile)) continue;

                if (_isPlayerTile[unusedIndex]) played = true;

                UsedTiles[unusedIndex] = true;
                break;
            }
        }
    }

    private void Print()
    {
        Console.Write($"{_gen}: ");
        _set.Print();

        Console.Write("  Unused: [ ");
        for (var i = 0; i < Tiles.Length; i++)
            if (!IsTileUsed[i])
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

    public Solution GetSolution()
    {
        var solution = new Solution { IsValid = true };
        var currentNode = this;

        while (currentNode._parentNode is not null)
        {
            if (currentNode._isRun)
                solution.AddRun(currentNode._set);
            else
                solution.AddGroup(currentNode._set);

            currentNode = currentNode._parentNode;
        }

        return solution;
    }
}