using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public class ScoreSolver : SolverBase, IScoreSolver
{
    private readonly bool[] _isPlayerTile;

    public int BestScore { get; private set; }

    private ScoreSolver(Tile[] tiles, int jokers, bool[] isPlayerTile) : base(tiles, jokers)
    {
        _isPlayerTile = isPlayerTile;
        BestScore = 0;
    }

    public static ScoreSolver Create(Set boardSet, Set playerSet)
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

        return new ScoreSolver(
            finalTiles,
            totalJokers,
            isPlayerTile
        );
    }

    public bool SearchSolution()
    {
        if (Tiles.Length + Jokers <= 2) return false;

        FindBestScore(new Solution(), 0, 0);

        return BestScore != 0;
    }
    
    private bool ValidateCondition()
    {
        var allBoardTilesUsed =
            !UsedTiles.Where((use, i) => !use && !_isPlayerTile[i]).Any(); //check pas de joker restant ?

        return allBoardTilesUsed;
    }


    private void FindBestScore(Solution solution, int solutionScore, int startIndex)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return;

            TrySet(GetRuns(startIndex), solution, solutionScore, startIndex);

            TrySet(GetGroups(startIndex), solution, solutionScore, startIndex);

            if (_isPlayerTile[startIndex]) startIndex++;
            else return;
        }
    }

    private void TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        var firstTileScore = _isPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            MarkTilesAsUsedOut(set, firstUnusedTileIndex, out var playerSetScore);

            var newSolutionScore = solutionScore + firstTileScore + playerSetScore;

            if (ValidateCondition() && newSolutionScore > BestScore) BestScore = newSolutionScore;

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
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (var i = firstUnusedIndex + 1; i < Tiles.Length; i++)
            {
                if (UsedTiles[i] || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = true;

                if (_isPlayerTile[i])
                {
                    playerSetScore += tile.Value;
                }

                break;
            }
        }
    }
}