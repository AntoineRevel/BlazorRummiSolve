using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public class BinaryFirstSolver : FirstSolver, IBinarySolver
{
    public   IEnumerable<Tile> TilesToPlay { get; init; }
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

    protected override bool ValidateCondition(int solutionScore)
    {
        return solutionScore > MinScore;
    }

    protected override Solution FindSolution(Solution solution, int solutionScore, int startIndex)
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
}