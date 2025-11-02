using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Incremental;

public sealed class IncrementalComplexSolver : ComplexSolver, ISolver
{
    private readonly int _availableJokers;
    private readonly int _boardJokers;
    private int _bestSolutionScore;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;

    private IncrementalComplexSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(tiles,
        jokers, isPlayerTile)
    {
        _availableJokers = jokers;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
    }

    private Solution BestSolution { get; set; } = new();
    private IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => IsPlayerTile[i] && _bestUsedTiles[i]);
    private int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        if (Tiles.Length + Jokers <= 2) return SolverResult.Invalid(GetType().Name);

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                return SolverResult.FromSolution(GetType().Name, BestSolution, TilesToPlay, JokerToPlay);

            var newSolution = FindSolution(new Solution(), 0, 0, cancellationToken);

            if (!newSolution.IsValid)
                return SolverResult.FromSolution(GetType().Name, BestSolution, TilesToPlay, JokerToPlay);

            BestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;
            if (UsedTiles.All(b => b))
                return SolverResult.FromSolution(GetType().Name, BestSolution, TilesToPlay, JokerToPlay);

            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }

    public static IncrementalComplexSolver Create(Set boardSet, Set playerSet)
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

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new IncrementalComplexSolver(
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