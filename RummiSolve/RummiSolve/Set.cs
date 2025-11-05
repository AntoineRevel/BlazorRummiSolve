namespace RummiSolve;

/// <summary>
/// Represents a set of tiles with explicit wildcard (joker) handling.
///
/// DESIGN RULES:
/// - Tiles list NEVER contains wildcards (IsJoker == true tiles are excluded)
/// - WildcardCount tracks the number of wildcards separately
/// - Use GetAllTilesIncludingWildcards() to get all tiles with wildcards for display/validation
/// - All mutations automatically enforce the "no wildcards in Tiles" invariant
/// </summary>
public class Set
{
    private List<Tile> _tiles = [];

    /// <summary>
    /// Gets the tiles in this set. INVARIANT: This list never contains wildcards (IsJoker == true).
    /// Wildcards are tracked separately in WildcardCount.
    /// </summary>
    public List<Tile> Tiles
    {
        get => _tiles;
        set
        {
            _tiles = value;
            NormalizeAfterMutation();
        }
    }

    /// <summary>
    /// Gets or sets the number of wildcards (jokers) in this set.
    /// Wildcards are not stored in the Tiles list.
    /// </summary>
    public int WildcardCount { get; set; }

    public Set()
    {
        _tiles = [];
        WildcardCount = 0;
    }

    public Set(List<Tile> tiles)
    {
        _tiles = [..tiles];
        NormalizeAfterMutation();
    }

    public Set(Set set)
    {
        _tiles = [..set.Tiles];
        WildcardCount = set.WildcardCount;
    }

    /// <summary>
    /// Returns all tiles including wildcards as a unified collection.
    /// Wildcards are appended at the end. Use this for display or validation.
    /// </summary>
    public IEnumerable<Tile> GetAllTilesIncludingWildcards()
    {
        foreach (var tile in Tiles)
            yield return tile;

        for (int i = 0; i < WildcardCount; i++)
            yield return new Tile(0, isJoker: true);
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }

    public void AddTile(Tile tile)
    {
        if (tile.IsJoker)
        {
            WildcardCount++;
        }
        else
        {
            _tiles.Add(tile);
        }
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        // ValidSet.Tiles may contain wildcards, so we filter them
        foreach (var tile in set.Tiles)
        {
            if (tile.IsJoker)
                WildcardCount++;
            else
                _tiles.Add(tile);
        }
    }

    public Set ConcatNew(Set set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var newSet = new Set
        {
            Tiles = [..Tiles],
            WildcardCount = WildcardCount
        };

        newSet.Tiles.AddRange(set.Tiles);
        newSet.WildcardCount += set.WildcardCount;
        return newSet;
    }

    public void Remove(Tile tile)
    {
        if (tile.IsJoker)
        {
            if (WildcardCount > 0)
                WildcardCount--;
        }
        else
        {
            _tiles.Remove(tile);
        }
    }

    public void PrintAllTiles()
    {
        foreach (var tile in GetAllTilesIncludingWildcards())
            tile.PrintTile();
        Console.WriteLine();
    }

    /// <summary>
    /// Ensures the invariant: Tiles list never contains wildcards.
    /// Extracts any wildcards from Tiles and updates WildcardCount.
    /// </summary>
    private void NormalizeAfterMutation()
    {
        var wildcardCount = _tiles.Count(t => t.IsJoker);
        if (wildcardCount > 0)
        {
            _tiles.RemoveAll(t => t.IsJoker);
            WildcardCount += wildcardCount;
        }
    }
}