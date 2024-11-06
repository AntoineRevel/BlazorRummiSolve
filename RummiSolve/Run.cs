namespace RummiSolve;

public class Run : ValidSet
{
    public bool IsValidRun()
    {
        if (Tiles.Length is < 3 or > 13)
        {
            return false;
        }

        for (var i = 1; i < Tiles.Length; i++)
        {
            if (Tiles[i].Color != Tiles[0].Color || Tiles[i].Value != Tiles[i - 1].Value + 1)
            {
                return false;
            }
        }

        return true;
    }
}