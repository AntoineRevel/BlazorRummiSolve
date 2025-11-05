namespace RummiSolve;

/// <summary>
/// Represents a validated set (Run or Group) of tiles, contain jokers 
/// </summary>
public abstract class ValidSet
{
    /// <summary>
    ///     contain the joker tiles.
    /// </summary>
    public required Tile[] Tiles { get; init; }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }
}