namespace RummiSolve;

/// <summary>
///     Represents a set of tiles with explicit joker handling. Tiles list NEVER contains jokers
/// </summary>
public class Set
{
    public Set()
    {
        Tiles = [];
        Jokers = 0;
    }

    public Set(List<Tile> tiles)
    {
        Tiles = [..tiles];
        NormalizeAfterMutation();
    }

    public Set(Set set)
    {
        Tiles = [..set.Tiles];
        Jokers = set.Jokers;
    }

    /// <summary>
    ///     never contains jokers
    /// </summary>
    public List<Tile> Tiles { get; }

    public int Jokers { get; private set; }

    public IEnumerable<Tile> GetAllTilesIncludingJokers()
    {
        foreach (var tile in Tiles)
            yield return tile;

        for (var i = 0; i < Jokers; i++)
            yield return new Tile(0, isJoker: true);
    }

    public void AddTile(Tile tile)
    {
        if (tile.IsJoker)
            Jokers++;
        else
            Tiles.Add(tile);
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        // ValidSet.Tiles may contain jokers, so we filter them
        foreach (var tile in set.Tiles)
            if (tile.IsJoker)
                Jokers++;
            else
                Tiles.Add(tile);
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
            Tiles.Remove(tile);
        }
    }

    public void PrintAllTiles()
    {
        foreach (var tile in GetAllTilesIncludingJokers())
            tile.PrintTile();
        Console.WriteLine();
    }

    private void NormalizeAfterMutation()
    {
        var jokerCount = Tiles.Count(t => t.IsJoker);
        if (jokerCount <= 0) return;
        {
            Tiles.RemoveAll(t => t.IsJoker);
            Jokers += jokerCount;
        }
    }
}