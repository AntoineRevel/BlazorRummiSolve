namespace RummiSolve;

/// <summary>
/// Represents a validated set (Run or Group) of tiles.
///
/// DESIGN RULES:
/// - Tiles array DOES contain jokers (IsJoker == true) because they represent specific positions
/// - Jokers tracks how many jokers are in this set for quick reference
/// - ValidSets are immutable (init-only properties)
/// </summary>
public abstract class ValidSet
{
    public required Tile[] Tiles { get; init; }

    /// <summary>
    /// Gets the number of jokers in this ValidSet.
    /// Note: Unlike Set, ValidSet.Tiles DOES contain the joker tiles.
    /// </summary>
    public int Jokers { get; init; }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }
}