using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public abstract class SolverBase(Tile[] tiles, int jokers)
{
    protected readonly Tile[] Tiles = tiles;
    protected readonly bool[] UsedTiles = new bool[tiles.Length];
    protected int Jokers = jokers;
    public Solution BestSolution { get; protected set; } = new();

    public abstract bool SearchSolution();

    protected void MarkTilesAsUsed(ValidSet set, bool isUsed, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles[0].IsJoker ? set.Tiles.Skip(2) : set.Tiles.Skip(1))
        {
            for (var i = firstUnusedIndex + 1; i < Tiles.Length; i++)
            {
                if (UsedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = isUsed;
                break;
            }
        }

        Jokers += isUsed ? -set.Jokers : set.Jokers;
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

            if (currentRun[^1].Value != 13) currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));

            else if (currentRun[0].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));

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
        var sameNumberTiles = Tiles
            .Where((tile, index) => !UsedTiles[index] &&
                                    tile.Value == Tiles[firstTileIndex].Value &&
                                    tile.Color != Tiles[firstTileIndex].Color)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

        if (Jokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { Tiles[firstTileIndex] };

            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
            {
                yield return new Group
                {
                    Tiles = [Tiles[firstTileIndex], sameNumberTiles[i], sameNumberTiles[j]]
                };
            }
        }
        else
        {
            for (var jokersUsed = 0; jokersUsed <= Jokers; jokersUsed++)
            for (var tilesUsed = 2; tilesUsed <= size + 1; tilesUsed++)
            {
                var totalTiles = tilesUsed + jokersUsed;
                if (totalTiles is < 3 or > 4) continue;

                var combinations = GetCombinations(sameNumberTiles, tilesUsed - 1);

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