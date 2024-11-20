namespace RummiSolve;

public abstract class ValidSet : ISet
{
    public required Tile[] Tiles { get; set; }
    public int Jokers { get; init; }
    
    public void ResetNewTiles()
    {
        Tiles = Tiles.Select(tile =>
        {
            tile.IsNew = false;
            return tile;
        }).ToArray();
    }
}