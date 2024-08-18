namespace RummiSolve;

public class Set
{
    public required List<Tile> Tiles { get; init; } //TODO made private

    public Set Copy()
    {
        return new Set { Tiles = [..Tiles] };
    }

    private void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public void AddTiles(Set set)
    {
        Tiles.AddRange(set.Tiles);
    }

    public void AddTilesFromInput(string input, Tile.Color color)
    {
        var numbers = input.Split(' ');
        foreach (var numberStr in numbers)
        {
            if (int.TryParse(numberStr, out var number) && number is >= 1 and <= 13)
            {
                var tile = new Tile(number, color);
                AddTile(tile);
            }
            else
            {
                Console.WriteLine($"Invalid number: {numberStr}. Skipping.");
            }
        }
    }

    public void RemoveAll(Set set)
    {
        foreach (var tile in set.Tiles)
        {
            Tiles.Remove(tile);
        }
    }

    public void PrintAllTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.PrintTile();
        }

        Console.WriteLine();
    }

    private void SortTiles()
    {
        Tiles.Sort((x, y) =>
        {
            var colorComparison = x.TileColor.CompareTo(y.TileColor);
            return colorComparison != 0 ? colorComparison : x.Number.CompareTo(y.Number);
        });
    }
    
    public Solution GetSolution()
    {
        SortTiles();
        var lenght = Tiles.Count;
        var usedTiles = new bool[lenght];
        const int firstUnusedTileIndex = 0;
        return GetSolution(new Solution(), usedTiles, lenght, firstUnusedTileIndex);
    }

    private Solution GetSolution(Solution solution, bool[] usedTiles, int unusedTile,
        int firstUnusedTileIndex)
    {
        switch (unusedTile)
        {
            case 0:
                return solution;
            case 1 or 2:
                return Solution.GetInvalidSolution();
        }
        
        while (firstUnusedTileIndex < Tiles.Count && usedTiles[firstUnusedTileIndex])
        {
            firstUnusedTileIndex++;
        }

        var runs = GetRuns(firstUnusedTileIndex, usedTiles);
        var groups = GetGroups(firstUnusedTileIndex, usedTiles);

        if (runs.Count == 0 && groups.Length == 0) return Solution.GetInvalidSolution();

        foreach (var run in runs)
        {
            MarkTilesAsUsedRef(run, usedTiles, ref unusedTile);
            var newSolution = GetSolution(solution.GetSolutionWithAddedRun(run), usedTiles, unusedTile,
                firstUnusedTileIndex);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsNotUsedRef(run, usedTiles, ref unusedTile);
        }

        foreach (var group in groups)
        {
            MarkTilesAsUsedRef(group, usedTiles, ref unusedTile);
            var newSolution = GetSolution(solution.GetSolutionWithAddedGroup(group), usedTiles,
                unusedTile, firstUnusedTileIndex);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsNotUsedRef(group, usedTiles, ref unusedTile);
        }

        return Solution.GetInvalidSolution();
    }

    private List<Run> GetRuns(int firstTileIndex, bool[] usedTiles)
    {
        var runs = new List<Run>();

        if (Tiles.Count == 0) return runs;

        var firstTile = Tiles[firstTileIndex];

        var currentRun = new Run { Tiles = [firstTile] };

        var lastNumber = firstTile.Number;

        for (var j = firstTileIndex + 1; j < Tiles.Count; j++)
        {
            if (usedTiles[j]) continue;

            var currentTile = Tiles[j];

            if (currentTile.TileColor == firstTile.TileColor)
            {
                if (currentTile.Number == lastNumber + 1)
                {
                    currentRun.AddTile(currentTile);
                    lastNumber = currentTile.Number;

                    if (currentRun.Tiles.Count >= 3)
                    {
                        runs.Add(new Run { Tiles = [..currentRun.Tiles] });
                    }
                }
                else if (currentTile.Number != lastNumber) break;
            }
            else break;
        }

        return runs.OrderByDescending(run => run.Tiles.Count).ToList();
    }

    private Group[] GetGroups(int firstTileIndex, bool[] usedTiles)
    {
        if (Tiles.Count == 0) return [];

        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Number;
        var color = firstTile.TileColor;

        var sameNumberTiles = Tiles
            .Where((tile, index) => !usedTiles[index] && tile.Number == number && tile.TileColor != color)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

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

    private void MarkTilesAsUsedRef(Set runOrGroup, bool[] usedTiles, ref int unusedTile)
    {
        foreach (var tile in runOrGroup.Tiles)
        {
            for (var i = 0; i < Tiles.Count; i++)
            {
                if (usedTiles[i] || !Tiles[i].Equals(tile)) continue;
                usedTiles[i] = true;
                unusedTile--;
                break;
            }
        }
    }

    private void MarkTilesAsNotUsedRef(Set runOrGroup, bool[] usedTiles, ref int unusedTile)
    {
        foreach (var tile in runOrGroup.Tiles)
        {
            for (var i = Tiles.Count - 1; i > -1; i--)
            {
                if (!usedTiles[i] || !Tiles[i].Equals(tile)) continue;
                usedTiles[i] = false;
                unusedTile++;
                break;
            }
        }
    }

    public List<Set> GetBestSets(int n)
    {
        var combinations = GetCombinations(Tiles, n);
        return combinations
            .Select(combination => new Set { Tiles = combination })
            .OrderByDescending(tiles => tiles.GetScore())
            .ToList();
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

    public int GetScore()
    {
        return Tiles.Sum(tile => tile.Number);
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
}