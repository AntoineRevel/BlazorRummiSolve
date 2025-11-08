using RummiSolve.Solver.Abstract;

// ReSharper disable MemberCanBePrivate.Global

namespace RummiSolve.Solver.Graph;

public class RummiNode : BaseSolver
{
    private readonly string _nodeId;
    private readonly int _startindex;
    public readonly List<RummiNode> Children = [];
    public readonly bool[] IsTileUsed;
    public readonly string NodeId;
    public readonly ValidSet Set;

    private RummiNode(string nodeId, ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startindex) :
        base(tiles, jokers)
    {
        _nodeId = nodeId;
        IsTileUsed = (bool[])isTileUsed.Clone();
        Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
        NodeId = nodeId;
        Set = set;
        _startindex = startindex;
    }

    public static RummiNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiNode("n", new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0);
    }

    public void GetChildren()
    {
        var cratedNode = 0;

        var firstUnusedTileIndex =
            Array.FindIndex(IsTileUsed, _startindex, used => !used);
        UsedTiles[firstUnusedTileIndex] = true;

        for (var i = firstUnusedTileIndex; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;

            foreach (var set in GetRuns(i).Concat(GetGroups(i)))
            {
                cratedNode++;
                MarkTilesAsUsed(set, i);
                var child = new RummiNode(_nodeId + cratedNode, set, Tiles, UsedTiles, Jokers, firstUnusedTileIndex);
                MarkTilesAsUnused(set, i);
                Children.Add(child);
            }
        }
    }

    private void Print()
    {
        Console.Write($"{NodeId}: ");
        Set.Print();

        Console.Write("  Unused: [ ");
        for (var i = 0; i < Tiles.Length; i++)
            if (!IsTileUsed[i])
                Tiles[i].PrintTile();

        Console.WriteLine("]");
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