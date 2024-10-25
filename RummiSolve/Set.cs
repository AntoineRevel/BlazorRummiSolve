namespace RummiSolve;

public class Set
{
    public Tile[] Tiles;
    private bool _isSorted;

    public Set()
    {
        Tiles = [];
        _isSorted = false;
    }

    public void PrintAllTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.PrintTile();
        }
    }

    private void Sort()
    {
        if (_isSorted) return;
        Array.Sort(Tiles);
        _isSorted = true;
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Number);
    }

    public Solution GetSolution()
    {
        Sort();
        var length = Tiles.Length;
        var usedTiles = new bool[length];
        if (length <= 2)
        {
            return length == 0 ? new Solution() : Solution.GetInvalidSolution();
        }

        var solution = FindSolution(new Solution(), usedTiles, length, 0);
        return solution;
    }

    private Solution FindSolution(Solution solution, bool[] usedTiles, int unusedTileCount, int firstUnusedTileIndex)
    {
        firstUnusedTileIndex = GetNextUnusedTileIndex(usedTiles, firstUnusedTileIndex);

        // var availableJokerIndices = Tiles
        //     .Select((tile, index) => new { Tile = tile, Index = index })
        //     .Where(t => !usedTiles[t.Index] && t.Tile.IsJoker)
        //     .Select(t => t.Index)
        //     .ToList();

        var runs = GetRuns(firstUnusedTileIndex, usedTiles);
        var groups = GetGroups(firstUnusedTileIndex, usedTiles);

        if (TryAddSets(solution, runs, usedTiles, ref unusedTileCount, firstUnusedTileIndex,
                (sol, set) => sol.AddRun(set))
            ||
            TryAddSets(solution, groups, usedTiles, ref unusedTileCount, firstUnusedTileIndex,
                (sol, set) => sol.AddGroup(set)))
        {
            return solution;
        }

        return Solution.GetInvalidSolution();
    }

    private int GetNextUnusedTileIndex(bool[] usedTiles, int startIndex)
    {
        while (startIndex < Tiles.Length && usedTiles[startIndex])
        {
            startIndex++;
        }

        return startIndex;
    }

    private bool TryAddSets<T>(Solution solution, IEnumerable<T> sets, bool[] usedTiles, ref int unusedTileCount,
        int firstUnusedTileIndex, Action<Solution, T> addSetAction)
        where T : Set
    {
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, usedTiles, ref unusedTileCount);
            Solution newSolution;
            if (unusedTileCount <= 2)
            {
                newSolution = unusedTileCount == 0 ? solution : Solution.GetInvalidSolution();
            }
            else
            {
                newSolution = FindSolution(solution, usedTiles, unusedTileCount, firstUnusedTileIndex);
            }

            if (newSolution.IsValid)
            {
                addSetAction(newSolution, set);
                return true;
            }

            MarkTilesAsUsed(set, false, usedTiles, ref unusedTileCount);
        }

        return false;
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
            if (currentTile.TileColor == firstTile.TileColor && currentTile.Number == lastNumber + 1)
            {
                currentRun.Add(currentTile);
                lastNumber = currentTile.Number;

                if (currentRun.Count >= 3)
                {
                    runs.Add(new Run { Tiles = currentRun.ToArray() });
                }
            }

            if (currentTile.Number != lastNumber) break;
        }

        //TODO runs.Reverse();
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

    private void MarkTilesAsUsed(Set set, bool isUsed, bool[] usedTiles, ref int unusedTile)
    {
        foreach (var tile in set.Tiles)
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

    public static IEnumerable<Set> GetBestSets(List<Tile> tiles, int n)
    {
        var combinations = GetCombinations(tiles, n);
        return combinations
            .Select(combination => new Set { Tiles = combination.ToArray() })
            .OrderByDescending(t => t.GetScore());
    }

    private static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length)
    {
        if (length == 0) yield return [];
        else
        {
            HashSet<string> seenCombinations = [];
            for (var i = 0; i < list.Count; i++)
            {
                var element = list[i];
                var remainingList = list.Skip(i + 1).ToList();
                foreach (var combination in GetCombinations(remainingList, length - 1))
                {
                    combination.Insert(0, element);
                    combination.Sort();
                    var combinationKey = GetKey(combination);
                    if (seenCombinations.Add(combinationKey)) yield return combination;
                }
            }
        }
    }
    
    private static string GetKey(IEnumerable<Tile> sortedTiles)
    {
        return string.Join("-", sortedTiles);
    }
}