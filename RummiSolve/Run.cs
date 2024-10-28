namespace RummiSolve;

public class Run : Set
{
    public bool IsValidRun()
    {
        if (Tiles.Count is < 3 or > 13)
        {
            return false;
        }

        for (var i = 1; i < Tiles.Count; i++)
        {
            if (Tiles[i].TileColor != Tiles[0].TileColor || Tiles[i].Value != Tiles[i - 1].Value + 1)
            {
                return false;
            }
        }

        return true;
    }
}