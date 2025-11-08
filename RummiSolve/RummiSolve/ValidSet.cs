namespace RummiSolve;

/// <summary>
///     Represents a validated set (Run or Group) of tiles, contain jokers
/// </summary>
public class ValidSet(Tile[] tiles)
{
    public readonly Tile[] Tiles = tiles;

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }

    public void Print()
    {
        Console.Write("[ ");
        foreach (var tile in Tiles) tile.PrintTile();

        Console.WriteLine("]");
    }
}