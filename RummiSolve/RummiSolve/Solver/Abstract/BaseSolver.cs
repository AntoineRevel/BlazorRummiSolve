namespace RummiSolve.Solver.Abstract;

public abstract class BaseSolver(Tile[] tiles, int jokers)
{
    protected const int MinScore = 29;
    protected readonly Tile[] Tiles = tiles;
    protected readonly bool[] UsedTiles = new bool[tiles.Length];
    protected int Jokers = jokers;

    protected void MarkTilesAsUsed(ValidSet set, int unusedIndex)
    {
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
                break;
            }
        }
    }

    protected void MarkTilesAsUnused(ValidSet set, int unusedIndex)
    {
        var lastIndex = Tiles.Length - 1;

        for (var tIndex = set.Tiles.Length - 1; tIndex > 0; tIndex--)
        {
            var tile = set.Tiles[tIndex];

            if (tile.IsJoker)
            {
                Jokers += 1;
                continue;
            }

            for (; lastIndex > unusedIndex; lastIndex--)
            {
                if (!UsedTiles[lastIndex] || !Tiles[lastIndex].Equals(tile)) continue;

                UsedTiles[lastIndex] = false;
                break;
            }
        }
    }

    protected void MarkTilesAsUsedOut(ValidSet set, int unusedIndex, out int playerSetScore)
    {
        playerSetScore = 0;
        unusedIndex++;
        for (var tIndex = 1; tIndex < set.Tiles.Length; tIndex++)
        {
            var tile = set.Tiles[tIndex];
            playerSetScore += tile.Value;

            if (tile.IsJoker)
            {
                Jokers -= 1;

                continue;
            }

            for (; unusedIndex < Tiles.Length; unusedIndex++)
            {
                if (UsedTiles[unusedIndex] || !Tiles[unusedIndex].Equals(tile)) continue;

                UsedTiles[unusedIndex] = true;

                break;
            }
        }
    }

    protected IEnumerable<Run> GetRuns(int tileIndex, CancellationToken cancellationToken = default)
    {
        var availableJokers = Jokers;
        var color = Tiles[tileIndex].Color;
        var currentRun = new List<Tile> { Tiles[tileIndex] };
        var i = tileIndex + 1;

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            for (; i < Tiles.Length; i++)
            {
                if (Tiles[i].Color != color)
                {
                    i = Tiles.Length;
                    break;
                }

                if (UsedTiles[i] || Tiles[i].Value == currentRun[^1].Value) continue;

                if (Tiles[i].Value != currentRun[^1].Value + 1) break;

                currentRun.Add(Tiles[i]);

                if (currentRun.Count < 3) continue;

                var jokersUsed = Jokers - availableJokers;

                yield return new Run
                {
                    Tiles = [..currentRun]
                };
            }

            if (availableJokers <= 0) yield break;

            currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));

            availableJokers--;

            if (currentRun.Count < 3) continue;

            var jokersUsedJ = Jokers - availableJokers;

            yield return new Run
            {
                Tiles = [..currentRun]
            };
        }
    }

    protected IEnumerable<Group> GetGroups(int firstTileIndex, CancellationToken cancellationToken = default)
    {
        var sameNumberTiles = new HashSet<Tile>();

        for (var i = firstTileIndex; i < Tiles.Length; i++)
            if (!UsedTiles[i] &&
                Tiles[i].Value == Tiles[firstTileIndex].Value &&
                Tiles[i].Color != Tiles[firstTileIndex].Color)
                sameNumberTiles.Add(Tiles[i]);

        var size = sameNumberTiles.Count;

        if (Jokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { Tiles[firstTileIndex] };

            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;
            var uniqueTilesList = sameNumberTiles.ToList();

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                yield return new Group
                {
                    Tiles = [Tiles[firstTileIndex], uniqueTilesList[i], uniqueTilesList[j]]
                };
            }
        }
        else
        {
            var uniqueTilesList = sameNumberTiles.ToList();
            for (var jokersUsed = 0; jokersUsed <= Jokers; jokersUsed++)
            for (var tilesUsed = 2; tilesUsed <= size + 1; tilesUsed++)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                var totalTiles = tilesUsed + jokersUsed;
                if (totalTiles is < 3 or > 4) continue;

                var combinations = GetCombinations(uniqueTilesList, tilesUsed - 1, cancellationToken);

                foreach (var tiles in combinations)
                {
                    var groupTiles = new Tile[totalTiles];

                    groupTiles[0] = Tiles[firstTileIndex];

                    for (int i = tilesUsed - 2, j = 1; i >= 0; i--, j++) groupTiles[j] = tiles[i];

                    for (var k = 0; k < jokersUsed; k++)
                        groupTiles[tilesUsed + k] = new Tile(Tiles[firstTileIndex].Value, isJoker: true);

                    yield return new Group
                    {
                        Tiles = groupTiles
                    };
                }
            }
        }
    }

    internal static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            yield break;

        if (length == 0) yield return [];

        for (var i = 0; i < list.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var element = list[i];
            foreach (var combination in GetCombinations(list.Skip(i + 1).ToList(), length - 1, cancellationToken))
            {
                combination.Add(element);
                yield return combination;
            }
        }
    }
}