namespace RummiSolve.Solver;

public class IncrementalFirstSolver : SolverBase
{
    private readonly bool[] _isPlayerTile;
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _isPlayerTile[i] && _bestUsedTiles[i]);
    public int JokerToPlay => _availableJokers - _remainingJoker;

    private IncrementalFirstSolver(Tile[] tiles, int jokers, bool[] isPlayerTile) : base(tiles, jokers)
    {
        _availableJokers = jokers;
        _isPlayerTile = isPlayerTile;
        _bestUsedTiles = UsedTiles;
        _bestSolutionScore = 29;
    }

    public static IncrementalFirstSolver Create(Set playerSet)
    {
        var capacity = playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new IncrementalFirstSolver(
            finalTiles,
            totalJokers,
            isPlayerTile
        );
    }

    private bool ValidateCondition(int solutionScore)
    {
        var allBoardTilesUsed =
            !UsedTiles.Where((use, i) => !use && !_isPlayerTile[i]).Any(); //check pas de joker restant ?

        if (!allBoardTilesUsed || solutionScore <= _bestSolutionScore) return false;

        _bestSolutionScore = solutionScore;
        return true;
    }


    public bool SearchSolution()
    {
        if (Tiles.Length + Jokers <= 2) return false;


        while (true)
        {
            var newSolution = FindSolution(new Solution(), 0, 0);

            if (!newSolution.IsValid) return false;
            BestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;
            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }


    private Solution FindSolution(Solution solution, int solutionScore, int startIndex)
    {
        startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

        if (startIndex == -1) return solution;

        var solRun = TrySet(GetRuns(startIndex), solution, solutionScore, startIndex,
            (sol, run) => sol.AddRun(run));

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(startIndex), solution, solutionScore, startIndex,
            (sol, group) => sol.AddGroup(group));

        return solGroup;
    }


    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, firstUnusedTileIndex);
            var newSolution = solution;

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;
            else newSolution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex);

            if (newSolution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUsed(set, false, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;

        return solution;
    }
}