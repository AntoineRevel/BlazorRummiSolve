namespace RummiSolve.Solver.Abstract;

public abstract class ComplexSolver(Tile[] tiles, int jokers, bool[] isPlayerTile) : BaseSolver(tiles, jokers)
{
    protected readonly bool[] IsPlayerTile = isPlayerTile;
    protected void MarkTilesAsUsedOut(ValidSet set, int unusedIndex, out int playerSetScore)
    {
        playerSetScore = 0;
        unusedIndex++;
        for (var tIndex = 1; tIndex < set.Tiles.Length; tIndex++)
        {
            var tile = set.Tiles[tIndex];
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (; unusedIndex < Tiles.Length; unusedIndex++)
            {
                if (UsedTiles[unusedIndex] || !Tiles[unusedIndex].Equals(tile)) continue;

                UsedTiles[unusedIndex] = true;

                if (IsPlayerTile[unusedIndex])
                {
                    playerSetScore += tile.Value;
                }

                break;
            }
        }
    }
}