using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combi;

public sealed class BinaryBaseSolver : BaseSolver, IBinarySolver
{
    public Solution BinarySolution { get; private set; } = new();
    public required IEnumerable<Tile> TilesToPlay { get; init; }

    private BinaryBaseSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
    }

    public static BinaryBaseSolver Create(Set boardSet, List<Tile> playerTiles)
    {
        var capacity = boardSet.Tiles.Count + playerTiles.Count;

        var tiles = new List<Tile>(capacity);

        tiles.AddRange(boardSet.Tiles);

        tiles.AddRange(playerTiles);

        tiles.Sort();

        var playerJokers = playerTiles.Count(tile => tile.IsJoker);

        var totalJokers = boardSet.Jokers + playerJokers;

        if (totalJokers > 0) tiles.RemoveRange(tiles.Count - totalJokers, totalJokers);

        return new BinaryBaseSolver(
            tiles.ToArray(),
            totalJokers
        )
        {
            TilesToPlay = playerTiles,
        };
    }

    public void SearchSolution()
    {
        BinarySolution = FindSolution(new Solution(), 0);
    }

    private bool ValidateCondition()
    {
        if (UsedTiles.Any(b => !b)) return false;

        return Jokers == 0;
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
            MarkTilesAsUsed(set, firstUnusedTileIndex);

            var newSolution = solution;

            if (ValidateCondition()) solution.IsValid = true;
            else newSolution = FindSolution(solution, firstUnusedTileIndex);

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