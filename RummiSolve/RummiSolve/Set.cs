namespace RummiSolve;

public class Set
{
    public int Jokers;
    public List<Tile> Tiles;
    
    public Set()
    {
        Tiles = [];
    }

    public Set(List<Tile> tiles)
    {
        Tiles = [..tiles];
        Jokers = Tiles.Count(tile => tile.IsJoker);
    }

    public Set(Set set)
    {
        Tiles = [..set.Tiles];
        Jokers = set.Jokers;
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
        if (tile.IsJoker) Jokers++;
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        Tiles.AddRange(set.Tiles);
        Jokers += set.Jokers;
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
        Tiles.Remove(tile);

        if (tile.IsJoker) Jokers--;
    }


    public void PrintAllTiles()
    {
        foreach (var tile in Tiles) tile.PrintTile();
    }
}