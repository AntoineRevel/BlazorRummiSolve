namespace RummiSolve;

public class Group: Set
{
    public Group()
    {
    }

    public Group(string key) : base(key)
    {
    }

    public bool IsValidGroup()
    {
        if (Tiles.Length is < 3 or > 4)
        {
            return false;
        }

        var number = Tiles[0].Number;
        var colors = new HashSet<Tile.Color>();

        return Tiles.All(tile => tile.Number == number && colors.Add(tile.TileColor));
    }
}