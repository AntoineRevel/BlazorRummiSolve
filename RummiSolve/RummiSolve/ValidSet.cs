namespace RummiSolve;

/// <summary>
///     Represents a validated set (Run or Group) of tiles, contain jokers
/// </summary>
public abstract class ValidSet
{
    public required Tile[] Tiles { get; init; }

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