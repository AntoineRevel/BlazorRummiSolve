namespace RummiSolve.Solver;

public abstract class SolverBase(Tile[] tiles, int jokers)
{
    protected readonly Tile[] Tiles = tiles;
    protected readonly bool[] UsedTiles = new bool[tiles.Length];
    protected int Jokers = jokers;
    public Solution BestSolution { get; protected set; } = new();

    protected const int MinScore = 29;

    protected void MarkTilesAsUsed(ValidSet set, int firstUnusedIndex)
    {
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
                break;
            }
        }
    }

    protected void MarkTilesAsUnused(ValidSet set, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles.Skip(1))
        {
            if (tile.IsJoker)
            {
                Jokers += 1;
                continue;
            }

            for (var i = Tiles.Length - 1; i > firstUnusedIndex; i--)
            {
                if (!UsedTiles[i] || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = false;
                break;
            }
        }
    }

    protected IEnumerable<Run> GetRuns(int tileIndex)
    {
        var availableJokers = Jokers;
        var color = Tiles[tileIndex].Color;
        var currentRun = new List<Tile> { Tiles[tileIndex] };
        var i = tileIndex + 1;

        while (true)
        {
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
                    Tiles = [..currentRun],
                    Jokers = jokersUsed
                };
            }

            if (availableJokers <= 0) yield break;

            currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));

            availableJokers--;

            if (currentRun.Count < 3) continue;

            var jokersUsedJ = Jokers - availableJokers;

            yield return new Run
            {
                Tiles = [..currentRun],
                Jokers = jokersUsedJ
            };
        }
    }

    protected IEnumerable<Group> GetGroups(int firstTileIndex)
    {
        var sameNumberTiles = new HashSet<Tile>();

        for (var i = firstTileIndex; i < Tiles.Length; i++)
        {
            if (!UsedTiles[i] &&
                Tiles[i].Value == Tiles[firstTileIndex].Value &&
                Tiles[i].Color != Tiles[firstTileIndex].Color)
            {
                sameNumberTiles.Add(Tiles[i]);
            }
        }

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
                var totalTiles = tilesUsed + jokersUsed;
                if (totalTiles is < 3 or > 4) continue;

                var combinations = GetCombinations(uniqueTilesList, tilesUsed - 1);

                foreach (var tiles in combinations)
                {
                    var groupTiles = new Tile[totalTiles];

                    groupTiles[0] = Tiles[firstTileIndex];

                    for (var i = 0; i < tilesUsed - 1; i++) groupTiles[i + 1] = tiles[i];

                    for (var k = 0; k < jokersUsed; k++)
                        groupTiles[tilesUsed + k] = new Tile(Tiles[firstTileIndex].Value, isJoker: true);

                    yield return new Group
                    {
                        Tiles = groupTiles,
                        Jokers = jokersUsed
                    };
                }
            }
        }
    }

    private static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length)
    {
        if (length == 0) yield return [];

        for (var i = 0; i < list.Count; i++)
        {
            var element = list[i];
            foreach (var combination in GetCombinations(list.Skip(i + 1).ToList(), length - 1))
            {
                combination.Add(element);
                yield return combination;
            }
        }
    }
}