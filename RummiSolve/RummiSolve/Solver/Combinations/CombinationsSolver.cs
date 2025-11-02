using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public class CombinationsSolver : ISolver
{
    private readonly int _boardJokers;
    private readonly List<Tile> _boardTiles;
    private readonly List<Tile> _playerTilesJ;

    private CombinationsSolver(List<Tile> boardTiles, int boardJokers, List<Tile> playerTiles)
    {
        _boardTiles = boardTiles;
        _boardJokers = boardJokers;
        _playerTilesJ = playerTiles;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
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

        var result = firstBinarySolver.SearchSolution(cancellationToken);
        if (result.Found)
            return SolverResult.Valid(GetType().Name, result.BestSolution, result.TilesToPlay, result.JokerToPlay,
                true);


        for (var tileTry = _playerTilesJ.Count - 1; tileTry > 0; tileTry--)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            foreach (
                var combi in
                BaseSolver.GetCombinations(_playerTilesJ, tileTry, cancellationToken)
                    .OrderByDescending(l => l.Sum(t => t.Value)))
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var joker = 0;
                var nonJokerCombi = new List<Tile>(tileTry);
                foreach (var tile in combi)
                {
                    if (tile.IsJoker)
                        joker++;
                    else
                        nonJokerCombi.Add(tile);
                }

                var solver = new BinaryBaseSolver(_boardTiles.Concat(nonJokerCombi).Order().ToArray(),
                    joker + _boardJokers)
                {
                    TilesToPlay = nonJokerCombi,
                    JokerToPlay = joker
                };

                result = solver.SearchSolution(cancellationToken);
                if (!result.Found) continue;

                return result;
            }
        }

        return SolverResult.Invalid(GetType().Name);
    }

    public static CombinationsSolver Create(Set boardSet, Set playerSet)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new CombinationsSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles);
    }
}