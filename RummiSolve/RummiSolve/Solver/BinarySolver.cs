namespace RummiSolve.Solver;

public class BinarySolver : SolverBase
{
    private BinarySolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
    }

    public required IEnumerable<Tile> TilesToPlay { get; init; }


    public static BinarySolver Create(Set boardSet, List<Tile> playerTiles)
    {
        var capacity = boardSet.Tiles.Count + playerTiles.Count;

        var tiles = new List<Tile>(capacity);

        tiles.AddRange(boardSet.Tiles);

        tiles.AddRange(playerTiles);

        tiles.Sort();

        var playerJokers = playerTiles.Count(tile => tile.IsJoker);

        var totalJokers = boardSet.Jokers + playerJokers;

        if (totalJokers > 0) tiles.RemoveRange(tiles.Count - totalJokers, totalJokers);

        return new BinarySolver(
            tiles.ToArray(),
            totalJokers
        )
        {
            TilesToPlay = playerTiles,
        };
    }

    private bool ValidateCondition()
    {
        if (UsedTiles.Any(b => !b)) return false;

        return Jokers == 0;
    }


    public bool SearchSolution()
    {
        BestSolution = FindSolution(new Solution(), 0);

        return BestSolution.IsValid;
    }


    private Solution FindSolution(Solution solution, int startIndex)
    {
        startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

        if (startIndex == -1) return solution;

        var solRun = TrySet(GetRuns(startIndex), solution, startIndex,
            (sol, run) => sol.AddRun(run));

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(startIndex), solution, startIndex,
            (sol, group) => sol.AddGroup(group));

        return solGroup;
    }


    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, firstUnusedTileIndex);

            var newSolution = solution;

            if (ValidateCondition()) solution.IsValid = true;
            else newSolution = FindSolution(solution, firstUnusedTileIndex);

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