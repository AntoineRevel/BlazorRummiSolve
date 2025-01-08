using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public sealed class BinaryFirstBaseSolver : BaseSolver, IBinarySolver
{
    public Solution BinarySolution { get; private set; } = new();
    public required IEnumerable<Tile> TilesToPlay { get; init; }

    internal BinaryFirstBaseSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
    }

    public static BinaryFirstBaseSolver Create(List<Tile> playerTiles)
    {
        var tiles = new List<Tile>(playerTiles);

        tiles.Sort();

        var playerJokers = playerTiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tiles.RemoveRange(tiles.Count - playerJokers, playerJokers);

        return new BinaryFirstBaseSolver(
            tiles.ToArray(),
            playerJokers
        )
        {
            TilesToPlay = playerTiles,
        };
    }


    public void SearchSolution()
    {
        BinarySolution = FindSolution(new Solution(), 0, 0);
    }

    private bool ValidateCondition(int solutionScore)
    {
        return solutionScore > MinScore;
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
            MarkTilesAsUsed(set, firstUnusedTileIndex);
            var newSolution = solution;

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;
            else newSolution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex);

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