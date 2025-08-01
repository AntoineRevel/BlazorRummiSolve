using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Incremental;

public sealed class IncrementalScoreFieldComplexSolver : ComplexSolver, ISolver
{
    private readonly int _availableJokers;
    private readonly int _boardJokers;
    private int _bestSolutionScore;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _solutionScore;

    private IncrementalScoreFieldComplexSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(
        tiles,
        jokers, isPlayerTile)
    {
        _availableJokers = jokers;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
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
                
            var newSolution = FindSolution(new Solution(), 0, cancellationToken);

            if (!newSolution.IsValid) return new SolverResult(GetType().Name, bestSolution, TilesToPlay, JokerToPlay);

            bestSolution = newSolution;
            _bestSolutionScore = _solutionScore;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;

            if (UsedTiles.All(b => b))
                return new SolverResult(GetType().Name, bestSolution, TilesToPlay, JokerToPlay, true);

            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
            _solutionScore = 0;
        }
    }

    public static IncrementalScoreFieldComplexSolver Create(Set boardSet, Set playerSet)
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

        return new IncrementalScoreFieldComplexSolver(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers
        );
    }


    private bool ValidateCondition()
    {
        if (_solutionScore <= _bestSolutionScore) return false;

        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < UsedTiles.Length; i++)
        {
            if (!IsPlayerTile[i] && !UsedTiles[i]) return false;
        }

        return Jokers == 0;
    }


    private Solution FindSolution(Solution solution, int startIndex, CancellationToken cancellationToken = default)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            if (cancellationToken.IsCancellationRequested)
                return solution;
                
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TrySet(GetRuns(startIndex, cancellationToken), solution, startIndex,
                (sol, run) => sol.AddRun(run), cancellationToken);

            if (solRun.IsValid) return solRun;

            var solGroup = TrySet(GetGroups(startIndex, cancellationToken), solution, startIndex,
                (sol, group) => sol.AddGroup(group), cancellationToken);

            if (solGroup.IsValid) return solGroup;

            if (IsPlayerTile[startIndex]) startIndex++;
            else return solution;
        }

        return solution;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution, CancellationToken cancellationToken = default)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        _solutionScore += IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            MarkTilesAsUsed(set, firstUnusedTileIndex);

            // var normal = Tiles.Where((_, i) => _isPlayerTile[i] && UsedTiles[i]).Sum(t => t.Value);
            // if (_solutionScore != normal) throw new Exception();

            if (ValidateCondition()) solution.IsValid = true;

            else solution = FindSolution(solution, firstUnusedTileIndex, cancellationToken);

            if (solution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;
        _solutionScore -= IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;

        return solution;
    }

    private new void MarkTilesAsUsed(ValidSet set, int unusedIndex)
    {
        unusedIndex++;
        for (var tIndex = 1; tIndex < set.Tiles.Length; tIndex++)
        {
            var tile = set.Tiles[tIndex];
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (; unusedIndex < Tiles.Length; unusedIndex++)
            {
                if (UsedTiles[unusedIndex] || !Tiles[unusedIndex].Equals(tile)) continue;

                UsedTiles[unusedIndex] = true;

                if (IsPlayerTile[unusedIndex])
                {
                    _solutionScore += tile.Value;
                }

                break;
            }
        }
    }

    private new void MarkTilesAsUnused(ValidSet set, int firstUnusedIndex)
    {
        var lastIndex = Tiles.Length - 1;

        for (var tIndex = set.Tiles.Length - 1; tIndex > 0; tIndex--)
        {
            var tile = set.Tiles[tIndex];
            if (tile.IsJoker)
            {
                Jokers += 1;
                continue;
            }

            for (; lastIndex > firstUnusedIndex; lastIndex--)
            {
                if (!UsedTiles[lastIndex] || !Tiles[lastIndex].Equals(tile)) continue;

                UsedTiles[lastIndex] = false;

                if (IsPlayerTile[lastIndex])
                {
                    _solutionScore -= tile.Value;
                }

                break;
            }
        }
    }
}