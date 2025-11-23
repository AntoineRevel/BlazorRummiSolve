using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Incremental;

/// <summary>
///     Lightly optimized version of IncrementalComplexSolver with safe micro-optimizations:
///     - Cached TilesToPlay list (avoids LINQ on every call)
///     - Direct array construction in Create (avoids LINQ ToArray)
///     - Direct loop for allUsed check (avoids LINQ All)
///     These optimizations don't change the algorithm logic, just reduce allocations.
/// </summary>
public sealed class OptimizedIncrementalComplexSolver : ComplexSolver, ISolver
{
    private readonly int _availableJokers;
    private readonly int _boardJokers;
    private readonly List<Tile> _tilesToPlay; // Cached to avoid LINQ
    private int _bestSolutionScore;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;

    private OptimizedIncrementalComplexSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers)
        : base(tiles, jokers, isPlayerTile)
    {
        _availableJokers = jokers;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
        _tilesToPlay = new List<Tile>(tiles.Length);
    }

    private Solution BestSolution { get; set; } = new();
    private int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        if (Tiles.Length + Jokers <= 2)
            return SolverResult.Invalid(GetType().Name);

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                return SolverResult.FromSolution(GetType().Name, BestSolution, _tilesToPlay, JokerToPlay);

            var newSolution = FindSolution(new Solution(), 0, 0, cancellationToken);

            if (!newSolution.IsValid)
                return SolverResult.FromSolution(GetType().Name, BestSolution, _tilesToPlay, JokerToPlay);

            BestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;

            // Update cached tiles to play
            _tilesToPlay.Clear();
            for (var i = 0; i < Tiles.Length; i++)
                if (IsPlayerTile[i] && _bestUsedTiles[i])
                    _tilesToPlay.Add(Tiles[i]);

            // Direct loop instead of LINQ All()
            var allUsed = true;
            foreach (var t in UsedTiles)
                if (!t)
                {
                    allUsed = false;
                    break;
                }

            if (allUsed)
                return SolverResult.FromSolution(GetType().Name, BestSolution, _tilesToPlay, JokerToPlay);

            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }

    public static OptimizedIncrementalComplexSolver Create(Set boardSet, Set playerSet)
    {
        var capacity = boardSet.Tiles.Count + playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(boardSet.Tiles.Select(tile => (tile, false)));
        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = boardSet.Jokers + playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        // Direct array construction instead of LINQ
        var finalTiles = new Tile[combined.Count];
        var isPlayerTile = new bool[combined.Count];
        for (var i = 0; i < combined.Count; i++)
        {
            finalTiles[i] = combined[i].tile;
            isPlayerTile[i] = combined[i].isPlayerTile;
        }

        return new OptimizedIncrementalComplexSolver(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers
        );
    }

    private bool ValidateCondition(int solutionScore)
    {
        if (solutionScore <= _bestSolutionScore) return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < UsedTiles.Length; i++)
            if (!IsPlayerTile[i] && !UsedTiles[i])
                return false;

        return Jokers == 0;
    }

    private Solution FindSolution(Solution solution, int solutionScore, int startIndex,
        CancellationToken cancellationToken)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            if (cancellationToken.IsCancellationRequested)
                return solution;

            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TrySet(GetRuns(startIndex, cancellationToken), solution, solutionScore, startIndex,
                (sol, run) => sol.AddRun(run), cancellationToken);

            if (solRun.IsValid) return solRun;

            var solGroup = TrySet(GetGroups(startIndex, cancellationToken), solution, solutionScore, startIndex,
                (sol, group) => sol.AddGroup(group), cancellationToken);

            if (solGroup.IsValid) return solGroup;

            if (IsPlayerTile[startIndex]) startIndex++;
            else return solution;
        }

        return solution;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution, CancellationToken cancellationToken)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            MarkTilesAsUsedOut(set, firstUnusedTileIndex, _boardJokers, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition(newSolutionScore))
            {
                _bestSolutionScore = newSolutionScore;
                solution.IsValid = true;
            }

            else
            {
                solution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex, cancellationToken);
            }

            if (solution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;

        return solution;
    }
}