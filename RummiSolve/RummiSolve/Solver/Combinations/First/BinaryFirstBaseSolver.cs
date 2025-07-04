using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public sealed class BinaryFirstBaseSolver(Tile[] tiles, int jokers) : BaseSolver(tiles, jokers), IBinarySolver
{
    public IEnumerable<Tile> TilesToPlay => Tiles;
    public int JokerToPlay { get; } = jokers;
    public Solution BinarySolution { get; private set; } = new();

    public bool SearchSolution(CancellationToken cancellationToken = default)
    {
        BinarySolution = FindSolution(new Solution(), 0, 0, cancellationToken);
        return BinarySolution.IsValid;
    }

    private bool ValidateCondition(int solutionScore)
    {
        if (UsedTiles.Any(b => !b)) return false;

        return solutionScore > MinScore && Jokers == 0;
    }

    private Solution FindSolution(Solution solution, int solutionScore, int startIndex, CancellationToken cancellationToken = default)
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

        return solGroup;
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
            var newSolution = solution;

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;
            else newSolution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex, cancellationToken);

            if (newSolution.IsValid)
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