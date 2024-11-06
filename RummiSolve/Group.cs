namespace RummiSolve;

public class Group : ValidSet
{
    public bool IsValidGroup()
    {
        if (Tiles.Length is < 3 or > 4)
        {
            return false;
        }

        var number = Tiles[0].Value;
        var colors = new HashSet<TileColor>();

        return Tiles.All(tile => tile.Value == number && colors.Add(tile.Color));
    }
}