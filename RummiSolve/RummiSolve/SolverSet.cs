using System.Collections.Immutable;

namespace RummiSolve;

internal class SolverSet(Tile[] tiles, int jokers, bool[] usedTiles, bool[] isPlayerTile, int playerJokers)
{
    private readonly Tile[] _tiles = tiles;
    private readonly bool[] _usedTiles = usedTiles;
    private readonly bool[] _isPlayerTile = isPlayerTile;
    private readonly int _playerJokers = playerJokers;
    private int _jokers = jokers;

    
    
    public IEnumerable<Tile> GetTilesToPlay() => _tiles.Where((_, i) => _isPlayerTile[i] && _usedTiles[i]);

    public int JokerToPlay()
    {
        return 0;
    }
    
    
    public static SolverSet Initialize(Set boardSet, Set playerSet)
    {
        var combined = new List<(Tile tile, bool isPlayerTile)>(boardSet.Tiles.Count + playerSet.Tiles.Count);
        
        combined.AddRange(boardSet.Tiles.Select(tile => (tile, false)));

        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));
        
        var totalJokers = boardSet.Jokers + playerSet.Jokers;
        
        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });
        
        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);
        
        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();
        
        return new SolverSet(
            finalTiles,
            totalJokers,
            new bool[finalTiles.Length], 
            isPlayerTile,
            playerSet.Jokers
        );
    }
}