using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

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

    public bool SearchBestScore()
    {
        if (Tiles.Length + Jokers <= 2) return false;

        FindBestScore(new Solution(), 0, 0);

        return BestScore != 0;
    }

    private bool ValidateCondition(int solutionScore)
    {
        return solutionScore >= 30;
    }


    private void FindBestScore(Solution solution, int solutionScore, int startIndex)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return;

            TrySet(GetRuns(startIndex), solution, solutionScore, startIndex);

            TrySet(GetGroups(startIndex), solution, solutionScore, startIndex);

            startIndex++;
        }
    }

    private void TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = Tiles[firstUnusedTileIndex].Value;
        foreach (var set in sets)
        {
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition(newSolutionScore) && newSolutionScore > BestScore) BestScore = newSolutionScore;

            FindBestScore(solution, newSolutionScore, firstUnusedTileIndex);

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;
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