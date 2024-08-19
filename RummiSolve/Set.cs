namespace RummiSolve;

public class Set
{
    public required Tile[] Tiles { get; init; }

    public void PrintAllTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.PrintTile();
        }
    }

    public Solution GetSolution()
    {
        Array.Sort(Tiles);
        var length = Tiles.Length;
        var usedTiles = new bool[length];
        return GetSolution(new Solution(), usedTiles, length, 0);
    }

    private Solution GetSolution(Solution solution, bool[] usedTiles, int unusedTileCount, int firstUnusedTileIndex)
    {
        switch (unusedTileCount)
        {
            case 0:
                return solution;
            case 1 or 2:
                return Solution.GetInvalidSolution();
        }

        while (firstUnusedTileIndex < Tiles.Length && usedTiles[firstUnusedTileIndex])
        {
            firstUnusedTileIndex++;
        }

        var runs = GetRuns(firstUnusedTileIndex, usedTiles);
        var groups = GetGroups(firstUnusedTileIndex, usedTiles);

        for (var i = runs.Count - 1; i >= 0; i--)
        {
            var run = runs[i];
            MarkTilesAsUsed(run, true, usedTiles, ref unusedTileCount);
            var newSolution = GetSolution(solution.GetSolutionWithAddedRun(run), usedTiles, unusedTileCount,
                firstUnusedTileIndex);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsUsed(run, false, usedTiles, ref unusedTileCount);
        }

        foreach (var group in groups)
        {
            MarkTilesAsUsed(group, true, usedTiles, ref unusedTileCount);
            var newSolution = GetSolution(solution.GetSolutionWithAddedGroup(group), usedTiles,
                unusedTileCount, firstUnusedTileIndex);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsUsed(group, false, usedTiles, ref unusedTileCount);
        }

        return Solution.GetInvalidSolution();
    }

    private List<Run> GetRuns(int firstTileIndex, bool[] usedTiles)
    {
        var runs = new List<Run>();

        if (Tiles.Length == 0) return runs;

        var firstTile = Tiles[firstTileIndex];

        var currentRun = new List<Tile> { firstTile };

        var lastNumber = firstTile.Number;

        for (var j = firstTileIndex + 1; j < Tiles.Length; j++)
        {
            if (usedTiles[j]) continue;

            var currentTile = Tiles[j];

            if (currentTile.TileColor == firstTile.TileColor)
            {
                if (currentTile.Number == lastNumber + 1)
                {
                    currentRun.Add(currentTile);
                    lastNumber = currentTile.Number;

                    if (currentRun.Count >= 3)
                    {
                        runs.Add(new Run { Tiles = currentRun.ToArray() });
                    }
                }
                else if (currentTile.Number != lastNumber) break;
            }
            else break;
        }
        
        return runs;
    }

    private Group[] GetGroups(int firstTileIndex, bool[] usedTiles)
    {
        if (Tiles.Length == 0) return [];

        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Number;
        var color = firstTile.TileColor;

        var sameNumberTiles = Tiles
            .Where((tile, index) => !usedTiles[index] && tile.Number == number && tile.TileColor != color)
            .Distinct()
            .ToArray();

        var size = sameNumberTiles.Length;

        return size switch
        {
            < 2 => [],
            2 => [new Group { Tiles = [firstTile, ..sameNumberTiles] }],
            3 =>
            [
                new Group { Tiles = [firstTile, ..sameNumberTiles] },
                new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[1]] },
                new Group { Tiles = [firstTile, sameNumberTiles[1], sameNumberTiles[2]] },
                new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[2]] }
            ],
            _ => []
        };
    }

    private void MarkTilesAsUsed(Set runOrGroup, bool isUsed, bool[] usedTiles, ref int unusedTile)
    {
        foreach (var tile in runOrGroup.Tiles)
        {
            for (var i = 0; i < Tiles.Length; i++)
            {
                if (usedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                usedTiles[i] = isUsed;
                unusedTile += isUsed ? -1 : 1;
                break;
            }
        }
    }
    
    public Set ShuffleTiles()
    {
        var random = new Random();
        var n = Tiles.Length;

        for (var i = n - 1; i > 0; i--)
        {
            var j = random.Next(0, i + 1);
            (Tiles[i], Tiles[j]) = (Tiles[j], Tiles[i]);
        }

        return this;
    }

    public static List<Tile> GetTilesFromInput(string input, Tile.Color color)
    {
        var tiles = new List<Tile>();
        var numbers = input.Split(' ');
        foreach (var numberStr in numbers)
        {
            if (int.TryParse(numberStr, out var number) && number is >= 1 and <= 13)
            {
                var tile = new Tile(number, color);
                tiles.Add(tile);
            }
            else
            {
                Console.WriteLine($"Invalid number: {numberStr}. Skipping.");
            }
        }

        return tiles;
    }

    public static Set ConcatTiles(Tile[] tiles1, Tile[] tiles2)
    {
        var combinedTiles = new Tile[tiles1.Length + tiles2.Length];

        Array.Copy(tiles1, 0, combinedTiles, 0, tiles1.Length);

        Array.Copy(tiles2, 0, combinedTiles, tiles1.Length, tiles2.Length);

        return new Set { Tiles = combinedTiles };
    }

    public static IEnumerable<Tile[]> GetBestSets(List<Tile> tiles, int n)
    {
        var combinations = GetCombinations(tiles, n);
        return combinations
            .Select(combination => combination.ToArray())
            .OrderByDescending(t => t.Sum(tile => tile.Number));
    }

    private static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length)
    {
        if (length == 0) yield return [];
        else
        {
            for (var i = 0; i < list.Count; i++)
            {
                var element = list[i];
                var remainingList = list.Skip(i + 1).ToList();
                foreach (var combination in GetCombinations(remainingList, length - 1))
                {
                    combination.Insert(0, element);
                    yield return combination;
                }
            }
        }
    }
}