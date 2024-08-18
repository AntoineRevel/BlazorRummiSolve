namespace RummiSolve;

public class Run: Set
{
    public bool IsValidRun()
    {
        if (Tiles.Length is < 3 or > 13)
        {
            return false;
        }

        for (var i = 1; i < Tiles.Length; i++)
        {
            if (Tiles[i].TileColor != Tiles[0].TileColor || Tiles[i].Number != Tiles[i - 1].Number + 1)
            {
                return false;
            }
        }

        return true;
    }
}