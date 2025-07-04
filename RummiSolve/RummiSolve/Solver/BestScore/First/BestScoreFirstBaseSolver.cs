using RummiSolve.Results;
using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.BestScore.First;

public class BestScoreFirstBaseSolver : BaseSolver, ISolver
{
    private readonly int _availableJokers;
    private int _bestSolutionScore;

    private BestScoreFirstBaseSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
    }

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        var scoreSolver = new ScoreFirstBaseSolver(Tiles, Jokers);

        var canPlay = scoreSolver.SearchBestScore(cancellationToken);

        if (!canPlay) return new SolverResult(GetType().Name);
        ;

        _bestSolutionScore = scoreSolver.BestScore;
        var bestSolution = FindSolution(new Solution(), 0, 0, cancellationToken);
        var tilesToPlay = Tiles.Where((_, i) => UsedTiles[i]);
        var jokerToPlay = _availableJokers - Jokers;
        var won = UsedTiles.All(b => b);

        return new SolverResult(GetType().Name, bestSolution, tilesToPlay, jokerToPlay, won);
    }


    public static BestScoreFirstBaseSolver Create(Set playerSet)
    {
        var tiles = new List<Tile>(playerSet.Tiles);

        tiles.Sort();

        if (playerSet.Jokers > 0) tiles.RemoveRange(tiles.Count - playerSet.Jokers, playerSet.Jokers);


        return new BestScoreFirstBaseSolver(
            tiles.ToArray(),
            playerSet.Jokers
        );
    }


    private bool ValidateCondition(int solutionScore)
    {
        return solutionScore == _bestSolutionScore;
    }


    private Solution FindSolution(Solution solution, int solutionScore, int startIndex, CancellationToken cancellationToken = default)
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
        var firstTileScore = Tiles[firstUnusedTileIndex].Value;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

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