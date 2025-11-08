using RummiSolve.Solver.Abstract;

// ReSharper disable MemberCanBePrivate.Global

namespace RummiSolve.Solver.Graph;

public class RummiNode : BaseSolver
{
    private readonly int _gen;
    private readonly int _startIndex;
    public readonly List<RummiNode> Children = [];
    public readonly bool[] IsTileUsed;
    public readonly ValidSet Set;

    private RummiNode(ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startIndex, int gen) :
        base(tiles, jokers)
    {
        IsTileUsed = (bool[])isTileUsed.Clone();
        Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
        Set = set;
        _startIndex = startIndex;
        _gen = gen;
    }

    public static RummiNode CreateRoot(Tile[] tiles, int jokers)
    {
        return new RummiNode(new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0, 0);
    }

    public void GetChildren()
    {
        var createdNode = 0;

        for (var i = _startIndex; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;
            UsedTiles[i] = true;
            foreach (var set in GetRuns(i).Concat(GetGroups(i)))
            {
                createdNode++;
                MarkTilesAsUsed(set, i);
                var child = new RummiNode(set, Tiles, UsedTiles, Jokers, i + 1, createdNode);
                MarkTilesAsUnused(set, i);
                Children.Add(child);
            }

            UsedTiles[i] = false;
        }
    }

    private void Print()
    {
        Console.Write($"{_gen}: ");
        Set.Print();

        Console.Write("  Unused: [ ");
        for (var i = 0; i < Tiles.Length; i++)
            if (!IsTileUsed[i])
                Tiles[i].PrintTile();
        for (var i = 0; i < Jokers; i++) Tile.PrintJoker();
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