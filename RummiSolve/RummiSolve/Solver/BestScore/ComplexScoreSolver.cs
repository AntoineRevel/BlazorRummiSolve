using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.BestScore;

public class ComplexScoreSolver(Tile[] tiles, int jokers, bool[] isPlayerTile)
    : ComplexSolver(tiles, jokers, isPlayerTile), IScoreSolver
{
    public int BestScore { get; private set; }

    public static ComplexScoreSolver Create(Set boardSet, Set playerSet)
    {
        var capacity = boardSet.Tiles.Count + playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(boardSet.Tiles.Select(tile => (tile, false)));
        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = boardSet.Jokers + playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new ComplexScoreSolver(
            finalTiles,
            totalJokers,
            isPlayerTile
        );
    }

    public bool SearchBestScore(CancellationToken cancellationToken = default)
    {
        if (Tiles.Length + Jokers <= 2) return false;

        FindBestScore(new Solution(), 0, 0, cancellationToken);

        return BestScore != 0;
    }

    private bool ValidateCondition()
    {
        var allBoardTilesUsed =
            !UsedTiles.Where((use, i) => !use && !IsPlayerTile[i]).Any(); //check pas de joker restant ?

        return allBoardTilesUsed;
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

            if (IsPlayerTile[startIndex]) startIndex++;
            else return;
        }
    }

    private void TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex, CancellationToken cancellationToken = default)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = IsPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition() && newSolutionScore > BestScore) BestScore = newSolutionScore;

            FindBestScore(solution, newSolutionScore, firstUnusedTileIndex, cancellationToken);

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;
    }
}