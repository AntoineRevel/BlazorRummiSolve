using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Incremental;

public sealed class IncrementalFirstBaseSolver : BaseSolver, ISolver
{
    private readonly int _availableJokers;
    private int _bestSolutionScore;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;

    private IncrementalFirstBaseSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
        _bestUsedTiles = UsedTiles;
        _bestSolutionScore = MinScore;
    }

    private Solution BestSolution { get; set; } = new();
    private IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _bestUsedTiles[i]);
    private int JokerToPlay => _availableJokers - _remainingJoker;

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

    public static IncrementalFirstBaseSolver Create(in Set playerSet)
    {
        var tiles = new List<Tile>(playerSet.Tiles);

        tiles.Sort();

        if (playerSet.Jokers > 0) tiles.RemoveRange(tiles.Count - playerSet.Jokers, playerSet.Jokers);

        return new IncrementalFirstBaseSolver(
            tiles.ToArray(),
            playerSet.Jokers
        );
    }

    private bool ValidateCondition(int solutionScore)
    {
        if (solutionScore <= _bestSolutionScore) return false;

        _bestSolutionScore = solutionScore;
        return true;
    }

    private Solution FindSolution(Solution solution, int solutionScore, int startIndex,
        CancellationToken cancellationToken = default)
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

            startIndex++;
        }

        return solution;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution, CancellationToken cancellationToken = default)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            MarkTilesAsUsed(set, firstUnusedTileIndex);

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;

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