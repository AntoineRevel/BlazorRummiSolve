using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public sealed class BinaryBaseSolver(Tile[] tiles, int jokers) : BaseSolver(tiles, jokers), IBinarySolver
{
    public required int JokerToPlay { get; init; }
    public Solution BinarySolution { get; private set; } = new();
    public required IEnumerable<Tile> TilesToPlay { get; init; }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        BinarySolution = FindSolution(new Solution(), 0, cancellationToken);
        return new SolverResult(GetType().Name, BinarySolution, TilesToPlay, JokerToPlay, BinarySolution.IsValid);
    }

    private bool ValidateCondition()
    {
        if (UsedTiles.Any(b => !b)) return false;

        return Jokers == 0;
    }

    private Solution FindSolution(Solution solution, int startIndex, CancellationToken cancellationToken)
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

        return solGroup;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution, CancellationToken cancellationToken)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            MarkTilesAsUsed(set, firstUnusedTileIndex);

            var newSolution = solution;

            if (ValidateCondition()) solution.IsValid = true;
            else newSolution = FindSolution(solution, firstUnusedTileIndex, cancellationToken);

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