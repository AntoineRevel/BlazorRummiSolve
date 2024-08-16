using System.Reflection;

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

    private List<Run> GetRuns()
    {
        var runs = new List<Run>();

        if (Tiles.Count == 0) return runs;

        var firstTile = Tiles[0];
        var currentRun = new Run { Tiles = [firstTile] };

        var lastNumber = firstTile.Number;

        for (var i = 1; i < Tiles.Count; i++)
        {
            var currentTile = Tiles[i];

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
                else if (currentTile.Number == lastNumber) continue;
                else break;
            }
            else break;
        }

        return runs;
    }

    private List<Group> GetGroups()
    {
        var groups = new List<Group>();

        if (Tiles.Count == 0) return groups;

        var firstTile = Tiles[0];
        var number = firstTile.Number;
        var color = firstTile.TileColor;

        var sameNumberTiles = Tiles.Where(tile => tile.Number == number && tile.TileColor != color).Distinct().ToList();

        if (sameNumberTiles.Count < 2) return groups;

        groups.Add(new Group { Tiles = [firstTile, ..sameNumberTiles] });
        if (sameNumberTiles.Count != 3) return groups;
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[1]] });
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[1], sameNumberTiles[2]] });
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[2]] });

        return groups;
    }


    public Solution GetSolution()
    {
        return GetSolution(new Solution());
    }

    private Solution GetSolution(Solution solution)
    {
        switch (Tiles.Count)
        {
            case 0:
                return solution;
            case 1 or 2:
                return Solution.GetInvalidSolution();
        }

        SortTiles();

        var runs = GetRuns();
        var groups = GetGroups();

        if (runs.Count == 0 && groups.Count == 0) return Solution.GetInvalidSolution();

        foreach (var run in runs)
        {
            run.Tiles.ForEach(tile => Tiles.Remove(tile));
            var newSolution = GetSolution(solution.GetSolutionWithAddedRun(run));
            if (newSolution.IsValid) return newSolution;
            Tiles.AddRange(run.Tiles);
        }

        foreach (var group in groups)
        {
            group.Tiles.ForEach(tile => Tiles.Remove(tile));
            var newSolution = GetSolution(solution.GetSolutionWithAddedGroup(group));
            if (newSolution.IsValid) return newSolution;
            Tiles.AddRange(group.Tiles);
        }

        return Solution.GetInvalidSolution();
    }

    public Solution GetSolutionArray()
    {
        SortTiles();
        var usedTiles = new bool[Tiles.Count];
        return GetSolution(new Solution(), ref usedTiles);
    }

    private Solution GetSolution(Solution solution, ref bool[] usedTiles)
    {
        switch (usedTiles.Count(b => !b))
        {
            case 0:
                return solution;
            case 1 or 2:
                return Solution.GetInvalidSolution();
        }

        var firstTileIndex = 0;
        for (var i = 0; i < Tiles.Count; i++)
        {
            if (usedTiles[i]) continue;
            firstTileIndex = i;

            break;
        }

        var runs = GetRuns(firstTileIndex, ref usedTiles);
        var groups = GetGroups(firstTileIndex, usedTiles);

        if (runs.Count == 0 && groups.Count == 0) return Solution.GetInvalidSolution();

        foreach (var run in runs)
        {
            MarkTilesAsUsed(run, ref usedTiles);
            var newSolution = GetSolution(solution.GetSolutionWithAddedRun(run), ref usedTiles);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsNotUsed(run, ref usedTiles);
        }

        foreach (var group in groups)
        {
            MarkTilesAsUsed(group, ref usedTiles);
            var newSolution = GetSolution(solution.GetSolutionWithAddedGroup(group), ref usedTiles);
            if (newSolution.IsValid) return newSolution;
            MarkTilesAsNotUsed(group, ref usedTiles);
        }

        return Solution.GetInvalidSolution();
    }

    private List<Run> GetRuns(int firstTileIndex, ref bool[] usedTiles)
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
                else if (currentTile.Number == lastNumber) continue;
                else break;
            }
            else break;
        }

        return runs;
    }

    private List<Group> GetGroups(int firstTileIndex, bool[] usedTiles)
    {
        var groups = new List<Group>();

        if (Tiles.Count == 0) return groups;

        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Number;
        var color = firstTile.TileColor;

        var sameNumberTiles = Tiles
            .Where((tile, index) => !usedTiles[index] && tile.Number == number && tile.TileColor != color)
            .Distinct()
            .ToList();

        if (sameNumberTiles.Count < 2) return groups;

        groups.Add(new Group { Tiles = [firstTile, ..sameNumberTiles] });
        if (sameNumberTiles.Count != 3) return groups;
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[1]] });
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[1], sameNumberTiles[2]] });
        groups.Add(new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[2]] });

        return groups;
    }

    private void MarkTilesAsUsed(Set runOrGroup, ref bool[] usedTiles)
    {
        foreach (var tile in runOrGroup.Tiles)
        {
            for (var i = 0; i < Tiles.Count; i++)
            {
                if (usedTiles[i] || !Tiles[i].Equals(tile)) continue;
                usedTiles[i] = true;
                break;
            }
        }
    }

    private void MarkTilesAsNotUsed(Set runOrGroup, ref bool[] usedTiles)
    {
        foreach (var tile in runOrGroup.Tiles)
        {
            for (var i = Tiles.Count - 1; i > 0; i--)
            {
                if (!usedTiles[i] || !Tiles[i].Equals(tile)) continue;
                usedTiles[i] = false;
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