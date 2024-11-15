namespace RummiSolve;

public class TileListComparer : IEqualityComparer<List<Tile>>
{
    public bool Equals(List<Tile>? x, List<Tile>? y)
         {
             if (x == null && y == null) return true;
             if (x == null || y == null || x.Count != y.Count) return false;
             return x.SequenceEqual(y);
         }

    public int GetHashCode(List<Tile> obj)
    {
        unchecked
        {
            return obj.Aggregate(17, (current, tile) => current * 31 + tile.GetHashCode());
        }
    }
}