namespace RummiSolve;

/// <summary>
/// Represents a validated set (Run or Group) of tiles.
///
/// DESIGN RULES:
/// - Tiles array DOES contain wildcards (IsJoker == true) because they represent specific positions
/// - WildcardCount tracks how many wildcards are in this set for quick reference
/// - ValidSets are immutable (init-only properties)
/// </summary>
public abstract class ValidSet
{
    public required Tile[] Tiles { get; init; }

    /// <summary>
    /// Gets the number of wildcards in this ValidSet.
    /// Note: Unlike Set, ValidSet.Tiles DOES contain the wildcard tiles.
    /// </summary>
    public int WildcardCount { get; init; }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }
}