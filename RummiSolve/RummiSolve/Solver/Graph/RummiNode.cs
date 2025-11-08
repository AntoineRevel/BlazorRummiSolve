using RummiSolve.Solver.Abstract;

// ReSharper disable MemberCanBePrivate.Global

namespace RummiSolve.Solver.Graph;

public class RummiNode(string nodeId, ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers)
    : BaseSolver(tiles, jokers)
{
    public readonly List<RummiNode> Children = [];
    public readonly bool[] IsTileUsed = (bool[])isTileUsed.Clone();
    public readonly string NodeId = nodeId;
    public readonly ValidSet Set = set;

    public static RummiNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiNode("n", new ValidSet([]), tiles, new bool[tiles.Length], jokers);
    }

    public void GetChildren()
    {
        var cratedNode = 0;
        var startIndex = Array.FindIndex(UsedTiles, 0, used => !used); //TODO add startIndex
        UsedTiles[startIndex] = true;

        for (var i = startIndex; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;

            foreach (var set in GetRuns(i).Concat(GetGroups(i)))
            {
                cratedNode++;
                MarkTilesAsUsed(set, i);
                var child = new RummiNode(nodeId + cratedNode, set, Tiles, UsedTiles, Jokers);
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