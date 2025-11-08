using RummiSolve.Solver.Abstract;

namespace RummiSolve.Solver.Graph;

public class RummiNode(int nodeNumber, ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers)
    : BaseSolver(tiles, jokers)
{
    public readonly bool[] IsTileUsed = (bool[])isTileUsed.Clone();
    public List<RummiNode> Children = [];
    public int NodeNumber = nodeNumber;
    public ValidSet Set = set;

    private void GetChildren()
    {
        var cratedNode = 0;
        for (var i = 0; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;

            foreach (var set in GetRuns(i).Concat<ValidSet>(GetGroups(i)))
            {
                cratedNode++;
                Children.Add(new RummiNode(cratedNode, set, Tiles, IsTileUsed, Jokers));
                Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
            }
        }
    }
}