using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public sealed class IncrementalFirstSolver : SolverBase, IIncrementalSolver
{
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _bestUsedTiles[i]);
    public bool Won { get; private set; }
    public int JokerToPlay => _availableJokers - _remainingJoker;

    private IncrementalFirstSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
        _bestUsedTiles = UsedTiles;
        _bestSolutionScore = MinScore;
    }

    public static IncrementalFirstSolver Create(in Set playerSet)
    {
        var tiles = new List<Tile>(playerSet.Tiles);

        tiles.Sort();

        if (playerSet.Jokers > 0) tiles.RemoveRange(tiles.Count - playerSet.Jokers, playerSet.Jokers);

        return new IncrementalFirstSolver(
            tiles.ToArray(),
            playerSet.Jokers
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

        _bestSolutionScore = solutionScore;
        return true;
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
            MarkTilesAsUsed(set, firstUnusedTileIndex);

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;

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