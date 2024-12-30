using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public class BestScoreFirstSolver : SolverBase, ISolver
{
    private readonly int _availableJokers;
    private int _bestSolutionScore;

    public bool Found { get; private set; }
    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay { get; private set; } = [];
    public int JokerToPlay { get; private set; }
    public bool Won { get; private set; }

    private BestScoreFirstSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
    }


    public static BestScoreFirstSolver Create(Set playerSet)
    {
        var tiles = new List<Tile>(playerSet.Tiles);

        tiles.Sort();

        if (playerSet.Jokers > 0) tiles.RemoveRange(tiles.Count - playerSet.Jokers, playerSet.Jokers);
        
        
        return new BestScoreFirstSolver(
            tiles.ToArray(),
            playerSet.Jokers
        );
    }

    public void SearchSolution()
    {
        var scoreSolver = new ScoreFirstSolver(Tiles, Jokers);

        var canPlay = scoreSolver.SearchBestScore();

        if (!canPlay) return;

        Found = true;

        _bestSolutionScore = scoreSolver.BestScore;
        BestSolution = FindSolution(new Solution(), 0, 0);
        Won = UsedTiles.All(b => b);
        TilesToPlay = Tiles.Where((_, i) => UsedTiles[i]);
        JokerToPlay = _availableJokers - Jokers;
    }


    private bool ValidateCondition(int solutionScore)
    {
        return solutionScore == _bestSolutionScore;
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
        var firstTileScore = Tiles[firstUnusedTileIndex].Value;
        foreach (var set in sets)
        {
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

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

    private void MarkTilesAsUsedOut(ValidSet set, int firstUnusedIndex, out int playerSetScore)
    {
        playerSetScore = 0;
        foreach (var tile in set.Tiles.Skip(1))
        {
            playerSetScore += tile.Value;
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (var i = firstUnusedIndex + 1; i < Tiles.Length; i++)
            {
                if (UsedTiles[i] || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = true;
                
                break;
            }
        }
    }
}