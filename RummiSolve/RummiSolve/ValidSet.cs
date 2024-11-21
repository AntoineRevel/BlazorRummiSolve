namespace RummiSolve;

public abstract class ValidSet : ISet
{
    public required Tile[] Tiles { get; init; }
    public int Jokers { get; init; }
}