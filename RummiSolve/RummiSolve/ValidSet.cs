namespace RummiSolve;

public abstract class ValidSet
{
    public required Tile[] Tiles { get; init; }
    public int Jokers { get; init; }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }
}