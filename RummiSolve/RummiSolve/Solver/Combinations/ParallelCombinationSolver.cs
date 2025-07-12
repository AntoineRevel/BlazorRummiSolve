using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public class ParallelCombinationSolver : ISolver
{
    private readonly int _boardJokers;
    private readonly List<Tile> _boardTiles;
    private readonly List<Tile> _playerTilesJ;

    private ParallelCombinationSolver(List<Tile> boardTiles, int boardJokers, List<Tile> playerTiles)
    {
        _boardTiles = boardTiles;
        _boardJokers = boardJokers;
        _playerTilesJ = playerTiles;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        var combinations = new List<List<Tile>>();

        for (var tileTry = _playerTilesJ.Count; tileTry > 0; tileTry--)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            combinations.AddRange(BaseSolver.GetCombinations(_playerTilesJ, tileTry, cancellationToken)
                .OrderByDescending(l => l.Sum(t => t.Value)));
        }

        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        var result = new SolverResult[combinations.Count];

        Parallel.ForEach(combinations, parallelOptions, (combi, loopState, index) =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                loopState.Stop();
                return;
            }

            var joker = 0;
            var nonJokerCombi = new List<Tile>();
            foreach (var tile in combi)
                if (tile.IsJoker)
                    joker++;
                else
                    nonJokerCombi.Add(tile);

            var solver = new BinaryBaseSolver(_boardTiles.Concat(nonJokerCombi).Order().ToArray(),
                joker + _boardJokers)
            {
                TilesToPlay = nonJokerCombi,
                JokerToPlay = joker
            };

            result[index] = solver.SearchSolution(cancellationToken);
        });

        var firstResult = result[0];
        if (firstResult.Found)
        {
            firstResult.Won = true;
            return firstResult;
        }

        foreach (var solverResult in result)
            if (solverResult.Found)
                return solverResult;

        return new SolverResult(GetType().Name);
    }

    public static ParallelCombinationSolver Create(Set boardSet, Set playerSet)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new ParallelCombinationSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles);
    }
}