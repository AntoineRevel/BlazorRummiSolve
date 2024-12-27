namespace RummiSolve.Solver;

public class IncrementalFirstSolver : FirstSolverBase
{
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _bestUsedTiles[i]);
    public int JokerToPlay => _availableJokers - _remainingJoker;

    private IncrementalFirstSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
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

        return new IncrementalFirstSolver(
            finalTiles,
            totalJokers
        );
    }

    private bool ValidateCondition(int solutionScore)
    {
        if (solutionScore <= _bestSolutionScore) return false;

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
        while (startIndex < UsedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TrySet(GetRuns(startIndex), solution, solutionScore, startIndex,
                (sol, run) => sol.AddRun(run));

            if (solRun.IsValid) return solRun;

            var solGroup = TrySet(GetGroups(startIndex), solution, solutionScore,startIndex,
                (sol, group) => sol.AddGroup(group));

            if (solGroup.IsValid) return solGroup;

            startIndex++;
        }

        return solution;
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