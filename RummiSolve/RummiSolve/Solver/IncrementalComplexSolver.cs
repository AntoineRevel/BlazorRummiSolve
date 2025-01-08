using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public sealed class IncrementalComplexSolver : ComplexSolver, ISolver
{
    private readonly int _boardJokers;
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore = 1;

    public bool Found => BestSolution.IsValid;

    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => IsPlayerTile[i] && _bestUsedTiles[i]);

    public bool Won { get; private set; }
    public int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    private IncrementalComplexSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(tiles,
        jokers, isPlayerTile)
    {
        _availableJokers = jokers;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
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

    public void SearchSolution()
    {
        if (Tiles.Length + Jokers <= 2) return;

        while (true)
        {
            var newSolution = FindSolution(new Solution(), 0, 0);

            if (!newSolution.IsValid) return;
            BestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;
            if (UsedTiles.All(b => b))
            {
                Won = true;
                return;
            }

            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }


    private bool ValidateCondition(int solutionScore)
    {
        if (solutionScore <= _bestSolutionScore) return false;
        
        var allBoardTilesUsed = true;
        
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (var i = 0; i < UsedTiles.Length; i++)
        {
            if (IsPlayerTile[i] || UsedTiles[i]) continue;
            allBoardTilesUsed = false;
            break;
        }

        return allBoardTilesUsed;
    }


    private Solution FindSolution(Solution solution, int solutionScore, int startIndex)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TrySet(GetRuns(startIndex), solution, solutionScore, startIndex,
                (sol, run) => sol.AddRun(run));

            if (solRun.IsValid) return solRun;

            var solGroup = TrySet(GetGroups(startIndex), solution, solutionScore, startIndex,
                (sol, group) => sol.AddGroup(group));

            if (solGroup.IsValid) return solGroup;

            if (IsPlayerTile[startIndex]) startIndex++;
            else return solution;
        }

        return solution;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition(newSolutionScore))
            {
                _bestSolutionScore = newSolutionScore;
                solution.IsValid = true;
            }

            else solution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex);

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