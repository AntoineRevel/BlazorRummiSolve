using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public class BinaryFirstSolver : SolverBase, IBinarySolver
{
    public required IEnumerable<Tile> TilesToPlay { get; init; }

    private BinaryFirstSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
    }

    public static BinaryFirstSolver Create(List<Tile> playerTiles)
    {
        var tiles = new List<Tile>(playerTiles);

        tiles.Sort();

        var playerJokers = playerTiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tiles.RemoveRange(tiles.Count - playerJokers, playerJokers);

        return new BinaryFirstSolver(
            tiles.ToArray(),
            playerJokers
        )
        {
            TilesToPlay = playerTiles,
        };
    }


    public override bool SearchSolution()
    {
        BestSolution = FindSolution(new Solution(), 0, 0);

        return BestSolution.IsValid;
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