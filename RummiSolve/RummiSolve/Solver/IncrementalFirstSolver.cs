namespace RummiSolve.Solver;

public class IncrementalFirstSolver : FirstSolver
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
        _bestSolutionScore = MinScore;
    }

    public static IncrementalFirstSolver Create(in Set playerSet)
    {
        var capacity = playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = playerSet.Jokers;

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();

        return new IncrementalFirstSolver(
            finalTiles,
            totalJokers
        );
    }

    public override bool SearchSolution()
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

    protected override bool ValidateCondition(int solutionScore)
    {
        if (solutionScore <= _bestSolutionScore) return false;

        _bestSolutionScore = solutionScore;
        return true;
    }

    protected override Solution FindSolution(Solution solution, int solutionScore, int startIndex)
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

            startIndex++;
        }

        return solution;
    }
}