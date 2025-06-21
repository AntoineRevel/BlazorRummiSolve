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

    public SolverResult SearchSolution()
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

        if (firstBinarySolver.SearchSolution())
            return new SolverResult(
                GetType().Name,
                firstBinarySolver.BinarySolution,
                firstBinarySolver.TilesToPlay,
                firstBinarySolver.JokerToPlay, true);


        for (var tileTry = _playerTilesJ.Count - 1; tileTry > 0; tileTry--)
        {
            foreach (
                var combi in
                BaseSolver.GetCombinations(_playerTilesJ, tileTry)
                    .OrderByDescending(l => l.Sum(t => t.Value)))
            {
                var joker = combi.Count(tile => tile.IsJoker);
                if (joker > 0) combi.RemoveRange(tileTry - joker, joker);


                var solver = new BinaryBaseSolver(_boardTiles.Concat(combi).Order().ToArray(), joker + _boardJokers)
                {
                    TilesToPlay = combi,
                    JokerToPlay = joker
                };

                if (!solver.SearchSolution()) continue;

                return new SolverResult(GetType().Name, solver.BinarySolution, solver.TilesToPlay, solver.JokerToPlay);
            }
        }

        return new SolverResult(GetType().Name);
        ;
    }

    public static CombinationsSolver Create(Set boardSet, Set playerSet)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new CombinationsSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles);
    }
}