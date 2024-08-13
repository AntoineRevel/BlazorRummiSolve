namespace RummiSolve;

public class Run : Set
{
    public bool IsValidRun()
    {
        if (Tiles.Count is < 3 or > 13)
        {
            return false;
        }

        Tiles.Sort((x, y) => x.Number.CompareTo(y.Number));

        for (var i = 1; i < Tiles.Count; i++)
        {
            if (Tiles[i].TileColor != Tiles[0].TileColor || Tiles[i].Number != Tiles[i - 1].Number + 1)
            {
                return false;
            }
        }

        return true;
    }
}