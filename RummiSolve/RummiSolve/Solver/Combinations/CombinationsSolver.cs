using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public class CombinationsSolver : ISolver
{
    private readonly List<Tile> _boardTiles;
    private readonly int _boardJokers;
    private readonly List<Tile> _playerTilesJ;
    public bool Found { get; private set; }
    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay { get; private set; } = [];
    public int JokerToPlay { get; private set; }
    public bool Won { get; private set; }

    private CombinationsSolver(List<Tile> boardTiles, int boardJokers, List<Tile> playerTiles)
    {
        _boardTiles = boardTiles;
        _boardJokers = boardJokers;
        _playerTilesJ = playerTiles;
    }

    public static CombinationsSolver Create(Set boardSet, Set playerSet)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new CombinationsSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles);
    }

    public void SearchSolution()
    {
        var playerJokers = _playerTilesJ.Count(tile => tile.IsJoker);
        var playerTiles = new List<Tile>(_playerTilesJ);

        if (playerJokers > 0) playerTiles.RemoveRange(playerTiles.Count - playerJokers, playerJokers);

        var winnableTiles = playerTiles.Concat(_boardTiles).ToList();
        winnableTiles.Sort();
        
        var firstBinarySolver = new BinaryBaseSolver(winnableTiles.ToArray(), playerJokers + _boardJokers)
        {
            TilesToPlay = playerTiles,
            JokerToPlay = playerJokers
        };

        Found = firstBinarySolver.SearchSolution();
        
        if (Found)
        {
            BestSolution = firstBinarySolver.BinarySolution;
            TilesToPlay = firstBinarySolver.TilesToPlay;
            JokerToPlay = firstBinarySolver.JokerToPlay;
            Won = true;
            return;
        }
    }
}