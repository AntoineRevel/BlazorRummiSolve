namespace RummiSolve;

public class Group: Set
{
    public bool IsValidGroup()
    {
        if (Tiles.Count is < 3 or > 4)
        {
            return false;
        }

        var number = Tiles[0].Number;
        var colors = new HashSet<Color>();

        return Tiles.All(tile => tile.Number == number && colors.Add(tile.TileColor));
    }
}