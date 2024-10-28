namespace RummiSolve;

public class Set
{
    public List<Tile> Tiles; //TODO check if List is the best
    private bool _isSorted;
    private int _jokers;

    public Set(int jokers = 0)
    {
        _jokers = jokers;
        Tiles = [];
        _isSorted = false;
    }

    public Set(string input, Tile.Color color)
    {
        _jokers = 0;
        _isSorted = false;

        Tiles = [];
        var numbers = input.Split(' ');

        foreach (var numberStr in numbers)
        {
            if (int.TryParse(numberStr, out var number) && number is >= 1 and <= 13)
            {
                var tile = new Tile(number, color);
                Tiles.Add(tile);
            }
            else if (numberStr.Equals("J", StringComparison.OrdinalIgnoreCase))
            {
                var jokerTile = new Tile(true);
                _jokers++;
                Tiles.Add(jokerTile);
            }
            else
            {
                Console.WriteLine($"Invalid number or character: {numberStr}. Skipping.");
            }
        }
    }

    public void AddTile(Tile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        Tiles.Add(tile);
        if (tile.IsJoker) _jokers++;
        _isSorted = false;
    }

    public void AddTiles(IEnumerable<Tile> tiles)
    {
        ArgumentNullException.ThrowIfNull(tiles);

        foreach (var tile in tiles)
        {
            AddTile(tile);
        }
    }

    public Set Concat(Set set)
    {
        ArgumentNullException.ThrowIfNull(set);

        Tiles.AddRange(set.Tiles);
        _jokers += set._jokers;
        _isSorted = false;

        return this;
    }

    public Set ConcatNew(Set set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var newSet = new Set(_jokers)
        {
            Tiles = [..Tiles],
            _isSorted = _isSorted
        };

        newSet.Tiles.AddRange(set.Tiles);
        newSet._jokers += set._jokers;
        newSet._isSorted = false;

        return newSet;
    }


    public bool Remove(Tile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        var removed = Tiles.Remove(tile);

        if (!removed) return removed;
        if (tile.IsJoker) _jokers--;

        return removed;
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
        Tiles.Sort();
        _isSorted = true;
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Number);
    }

    public Solution GetSolution()
    {
        Sort();
        var length = Tiles.Count;
        var usedTiles = new bool[length];
        if (length <= 2)
        {
            return length == 0 ? new Solution() : Solution.GetInvalidSolution();
        }

        var solution = FindSolution(new Solution(), usedTiles, length, 0, _jokers);
        return solution;
    }

    private Solution FindSolution(Solution solution, bool[] usedTiles, int unusedTileCount, int firstUnusedTileIndex,
        int availableJokers)
    {
        firstUnusedTileIndex = GetNextUnusedTileIndex(usedTiles, firstUnusedTileIndex);

        var runs = GetRuns(firstUnusedTileIndex, usedTiles, availableJokers);
        var groups = GetGroups(firstUnusedTileIndex, usedTiles, availableJokers);

        //TODO concat joker at the end

        if (TryAddSets(solution, runs, usedTiles, ref unusedTileCount, firstUnusedTileIndex
                , ref availableJokers, (sol, set) => sol.AddRun(set))
            ||
            TryAddSets(solution, groups, usedTiles, ref unusedTileCount, firstUnusedTileIndex
                , ref availableJokers, (sol, set) => sol.AddGroup(set)))
        {
            return solution;
        }

        return Solution.GetInvalidSolution();
    }

    private static int GetNextUnusedTileIndex(bool[] usedTiles, int startIndex)
    {
        return Array.FindIndex(usedTiles, startIndex, used => !used);
    }

    private bool TryAddSets<T>(Solution solution, IEnumerable<T> sets, bool[] usedTiles, ref int unusedTileCount,
        int firstUnusedTileIndex, ref int availableJokers, Action<Solution, T> addSetAction)
        where T : Set
    {
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, usedTiles, ref unusedTileCount, ref availableJokers);
            Solution newSolution;
            if (unusedTileCount <= 2)
            {
                newSolution = unusedTileCount == 0 ? solution : Solution.GetInvalidSolution();
            }
            else
            {
                newSolution = FindSolution(solution, usedTiles, unusedTileCount, firstUnusedTileIndex, availableJokers);
            }

            if (newSolution.IsValid)
            {
                addSetAction(newSolution, set);
                return true;
            }

            MarkTilesAsUsed(set, false, usedTiles, ref unusedTileCount, ref availableJokers);
        }

        return false;
    }

    private List<Run> GetRuns(int firstTileIndex, bool[] usedTiles, int availableJokers)
    {
        var runs = new List<Run>();

        if (Tiles.Count == 0) return runs;

        var firstTile = Tiles[firstTileIndex];

        var currentRun = new List<Tile> { firstTile };

        var lastNumber = firstTile.Number;

        for (var j = firstTileIndex + 1; j < Tiles.Count; j++)
        {
            if (usedTiles[j]) continue;

            var currentTile = Tiles[j];
            if (currentTile.TileColor == firstTile.TileColor && currentTile.Number == lastNumber + 1)
            {
                currentRun.Add(currentTile);
                lastNumber = currentTile.Number;

                if (currentRun.Count >= 3)
                {
                    runs.Add(new Run { Tiles = currentRun });
                }
            }

            if (currentTile.Number != lastNumber) break;
        }

        //TODO runs.Reverse();
        return runs;
    }

    private Group[] GetGroups(int firstTileIndex, bool[] usedTiles, int availableJokers)
    {
        if (Tiles.Count == 0) return [];

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

    private void MarkTilesAsUsed(Set set, bool isUsed, bool[] usedTiles, ref int unusedTile, ref int availableJokers)
    {
        foreach (var tile in set.Tiles)
        {
            for (var i = 0; i < Tiles.Count; i++)
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
        var n = Tiles.Count;

        for (var i = n - 1; i > 0; i--)
        {
            var j = random.Next(0, i + 1);
            (Tiles[i], Tiles[j]) = (Tiles[j], Tiles[i]);
        }

        return this;
    }

    public static IEnumerable<Set> GetBestSets(List<Tile> tiles, int n)
    {
        var combinations = GetCombinations(tiles, n);
        return combinations
            .Select(combination => new Set { Tiles = combination })
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