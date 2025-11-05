namespace RummiSolve;

/// <summary>
/// Represents a set of tiles with explicit joker handling.
///
/// DESIGN RULES:
/// - Tiles list NEVER contains jokers (IsJoker == true tiles are excluded)
/// - Jokers tracks the number of jokers separately
/// - Use GetAllTilesIncludingJokers() to get all tiles with jokers for display/validation
/// - All mutations automatically enforce the "no jokers in Tiles" invariant
/// </summary>
public class Set
{
    private List<Tile> _tiles = [];

    /// <summary>
    /// Gets the tiles in this set. INVARIANT: This list never contains jokers (IsJoker == true).
    /// Jokers are tracked separately in Jokers property.
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
    /// Gets or sets the number of jokers in this set.
    /// Jokers are not stored in the Tiles list.
    /// </summary>
    public int Jokers { get; set; }

    public Set()
    {
        _tiles = [];
        Jokers = 0;
    }

    public Set(List<Tile> tiles)
    {
        _tiles = [..tiles];
        NormalizeAfterMutation();
    }

    public Set(Set set)
    {
        _tiles = [..set.Tiles];
        Jokers = set.Jokers;
    }

    /// <summary>
    /// Returns all tiles including jokers as a unified collection.
    /// Jokers are appended at the end. Use this for display or validation.
    /// </summary>
    public IEnumerable<Tile> GetAllTilesIncludingJokers()
    {
        foreach (var tile in Tiles)
            yield return tile;

        for (int i = 0; i < Jokers; i++)
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
            Jokers++;
        }
        else
        {
            _tiles.Add(tile);
        }
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        // ValidSet.Tiles may contain jokers, so we filter them
        foreach (var tile in set.Tiles)
        {
            if (tile.IsJoker)
                Jokers++;
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
            Jokers = Jokers
        };

        newSet.Tiles.AddRange(set.Tiles);
        newSet.Jokers += set.Jokers;
        return newSet;
    }

    public void Remove(Tile tile)
    {
        if (tile.IsJoker)
        {
            if (Jokers > 0)
                Jokers--;
        }
        else
        {
            _tiles.Remove(tile);
        }
    }

    public void PrintAllTiles()
    {
        foreach (var tile in GetAllTilesIncludingJokers())
            tile.PrintTile();
        Console.WriteLine();
    }

    /// <summary>
    /// Ensures the invariant: Tiles list never contains jokers.
    /// Extracts any jokers from Tiles and updates Jokers count.
    /// </summary>
    private void NormalizeAfterMutation()
    {
        var jokerCount = _tiles.Count(t => t.IsJoker);
        if (jokerCount > 0)
        {
            _tiles.RemoveAll(t => t.IsJoker);
            Jokers += jokerCount;
        }
    }
}