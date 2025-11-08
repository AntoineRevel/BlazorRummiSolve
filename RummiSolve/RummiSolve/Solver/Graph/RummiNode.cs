using RummiSolve.Solver.Abstract;

// ReSharper disable MemberCanBePrivate.Global

namespace RummiSolve.Solver.Graph;

public class RummiNode(int nodeNumber, ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers)
    : BaseSolver(tiles, jokers)
{
    public readonly bool[] IsTileUsed = (bool[])isTileUsed.Clone();
    public readonly int NodeNumber = nodeNumber;
    public readonly ValidSet Set = set;
    public List<RummiNode> Children = [];

    public static RummiNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiNode(0, new ValidSet([]), tiles, new bool[tiles.Length], jokers);
    }

    public void GetChildren()
    {
        var cratedNode = 0;
        for (var i = 0; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;

            foreach (var set in GetRuns(i).Concat(GetGroups(i)))
            {
                cratedNode++;
                var child = new RummiNode(NodeNumber + cratedNode, set, Tiles, IsTileUsed, Jokers);
                Children.Add(child);
                Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
            }
        }
    }

    private void Print()
    {
        Console.Write($"Node {NodeNumber}: ");
        Set.Print();
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