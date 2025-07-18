using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Incremental;

public sealed class IncrementalComplexSolverTileAndSc : ComplexSolver, ISolver
{
    private readonly int _availableJokers;
    private readonly int _boardJokers;
    private int _bestPlayerUsedTiles;
    private int _bestSolutionScore;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;

    private IncrementalComplexSolverTileAndSc(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(
        tiles,
        jokers, isPlayerTile)
    {
        _availableJokers = jokers;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
        _bestPlayerUsedTiles = 0;
    }

    private IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => IsPlayerTile[i] && _bestUsedTiles[i]);
    private int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        if (Tiles.Length + Jokers <= 2) return new SolverResult(GetType().Name);
        ;

        Solution bestSolution = new();

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                return new SolverResult(GetType().Name, bestSolution, TilesToPlay, JokerToPlay);
                
            var newSolution = FindSolution(new Solution(), 0, 0, cancellationToken);

            if (!newSolution.IsValid) return new SolverResult(GetType().Name, bestSolution, TilesToPlay, JokerToPlay);
            bestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;

            if (UsedTiles.All(b => b))
                return new SolverResult(GetType().Name, bestSolution, TilesToPlay, JokerToPlay, true);

            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }

    public static IncrementalComplexSolverTileAndSc Create(Set boardSet, Set playerSet)
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

        return new IncrementalComplexSolverTileAndSc(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers
        );
    }


    private bool ValidateCondition(int solutionScore, out int usedTiles)
    {
        usedTiles = 0;

        //if (solutionScore < _bestSolutionScore) return false;

        for (var i = 0; i < UsedTiles.Length; i++)
        {
            if (!IsPlayerTile[i] && !UsedTiles[i]) return false;
            if (IsPlayerTile[i] && UsedTiles[i]) usedTiles++;
        }

        if (usedTiles < _bestPlayerUsedTiles) return false;

        if (Jokers != 0) return false;

        //if (solutionScore == _bestSolutionScore) return usedTiles > _bestPlayerUsedTiles;
        if (usedTiles == _bestPlayerUsedTiles) return solutionScore > _bestSolutionScore;

        return true;
    }


    private Solution FindSolution(Solution solution, int solutionScore, int startIndex, CancellationToken cancellationToken = default)
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
        Action<Solution, TS> addSetToSolution, CancellationToken cancellationToken = default)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition(newSolutionScore, out var usedTiles))
            {
                _bestSolutionScore = newSolutionScore;
                _bestPlayerUsedTiles = usedTiles;
                solution.IsValid = true;
            }

            else solution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex, cancellationToken);

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