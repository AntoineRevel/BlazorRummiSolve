using System.Collections.Concurrent;
using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public class ParallelCombinationsSolver : ISolver
{
    private readonly int _boardJokers;
    private readonly List<Tile> _boardTiles;
    private readonly List<Tile> _playerTilesJ;

    private ParallelCombinationsSolver(List<Tile> boardTiles, int boardJokers, List<Tile> playerTiles)
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
        };

        var results = new ConcurrentBag<(long index, SolverResult result)>();
        var foundSolutionIndex = long.MaxValue;
        var lockObject = new Lock();
        var cancellationTokenSources = new ConcurrentDictionary<long, CancellationTokenSource>();
        try
        {
            Parallel.ForEach(combinations, parallelOptions, (combi, loopState, index) =>
            {
                if (cancellationToken.IsCancellationRequested) loopState.Stop();

                if (index >= foundSolutionIndex) return;

                // Create a combined cancellation token for this specific task
                var taskCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cancellationTokenSources.TryAdd(index, taskCts);

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

                var result = solver.SearchSolution(taskCts.Token);

                if (!result.Found) return;

                results.Add((index, result));

                lock (lockObject)
                {
                    if (index >= foundSolutionIndex) return;

                    foundSolutionIndex = index;

                    // Cancel all tasks with higher indices
                    foreach (var kvp in cancellationTokenSources.Where(kvp => kvp.Key > index))
                    {
                        kvp.Value.Cancel();
                    }
                }
            });
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            // Dispose all cancellation token sources
            foreach (var kvp in cancellationTokenSources)
                kvp.Value.Dispose();
        }

        return GetBestResult(results);
    }


    private SolverResult GetBestResult(ConcurrentBag<(long Index, SolverResult Result)> results)
    {
        if (results.IsEmpty) return SolverResult.Invalid(GetType().Name);

        var bestResult = results.MinBy(r => r.Index);

        if (bestResult.Index == 0) bestResult.Result.Won = true;

        return bestResult.Result;
    }


    public static ParallelCombinationsSolver Create(Set boardSet, Set playerSet)
    {
        boardSet.Tiles.Sort();

        if (boardSet.Jokers > 0) boardSet.Tiles.RemoveRange(boardSet.Tiles.Count - boardSet.Jokers, boardSet.Jokers);

        return new ParallelCombinationsSolver(boardSet.Tiles, boardSet.Jokers, playerSet.Tiles);
    }
}