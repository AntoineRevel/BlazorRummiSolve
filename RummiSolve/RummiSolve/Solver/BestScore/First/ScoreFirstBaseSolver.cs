using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.BestScore.First;

public class ScoreFirstBaseSolver : BaseSolver, IScoreSolver
{
    public int BestScore { get; private set; }

    internal ScoreFirstBaseSolver(Tile[] tiles, int jokers) : base(tiles, jokers)
    {
        BestScore = 0;
    }

    public static ScoreFirstBaseSolver Create(Set playerSet)
    {
        var tiles = new List<Tile>(playerSet.Tiles);

        tiles.Sort();

        if (playerSet.Jokers > 0) tiles.RemoveRange(tiles.Count - playerSet.Jokers, playerSet.Jokers);

        return new ScoreFirstBaseSolver(tiles.ToArray(), playerSet.Jokers);
    }

    public bool SearchBestScore(CancellationToken cancellationToken = default)
    {
        if (Tiles.Length + Jokers <= 2) return false;

        FindBestScore(new Solution(), 0, 0, cancellationToken);

        return BestScore != 0;
    }

    private static bool ValidateCondition(int solutionScore)
    {
        return solutionScore >= 30;
    }

    private void FindBestScore(Solution solution, int solutionScore, int startIndex, CancellationToken cancellationToken = default)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
                
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return;

            TrySet(GetRuns(startIndex, cancellationToken), solution, solutionScore, startIndex, cancellationToken);

            TrySet(GetGroups(startIndex, cancellationToken), solution, solutionScore, startIndex, cancellationToken);

            startIndex++;
        }
    }

    private void TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex, CancellationToken cancellationToken = default)
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

            if (ValidateCondition(newSolutionScore) && newSolutionScore > BestScore) BestScore = newSolutionScore;

            FindBestScore(solution, newSolutionScore, firstUnusedTileIndex, cancellationToken);

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;
    }
}