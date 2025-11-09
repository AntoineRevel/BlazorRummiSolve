using System.Collections.Concurrent;
using RummiSolve.Solver.Abstract;

namespace RummiSolve.Solver.Graph;

public class RummiFirstNode : BaseSolver
{
    private readonly int _gen;
    private readonly bool _isRun;
    private readonly RummiFirstNode? _parentNode;
    private readonly ValidSet _set;
    private readonly int _startIndex;
    public readonly List<RummiFirstNode> Children = [];

    public readonly bool[] IsTileUsed;
    public readonly ConcurrentBag<RummiFirstNode> LeafNodes;
    public readonly int Score;

    private RummiFirstNode(ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startIndex, int gen,
        ConcurrentBag<RummiFirstNode> leafNodes, int score, RummiFirstNode? parentNode, bool isRun) :
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
    }

    public static RummiFirstNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiFirstNode(new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0, 0, [], 0, null, false);
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

        if (createdNode == 0) LeafNodes.Add(this);
    }

    private void CreateChildNode(ref int createdNode, int index, ValidSet set, bool isRun)
    {
        createdNode++;
        MarkTilesAsUsed(set, index);
        var child = new RummiFirstNode(set, Tiles, UsedTiles, Jokers, index + 1, createdNode, LeafNodes,
            Score + set.GetScore(), this, isRun);
        MarkTilesAsUnused(set, index);
        Children.Add(child);
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