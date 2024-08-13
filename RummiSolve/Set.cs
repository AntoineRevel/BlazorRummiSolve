namespace RummiSolve;

public class Set
{
    protected List<Tile> Tiles { get; private init; } = [];

    private void AddTile(Tile tile)
    {
        Tiles.Add(tile);
    }

    public void AddTilesFromInput(string input, Color color)
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

        SortTiles();
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

    public List<Run> GetRuns()
    {
        var runs = new List<Run>();

        if (Tiles.Count == 0) return runs;

        var firstTile = Tiles[0];
        var currentRun = new Run();
        currentRun.AddTile(firstTile);

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

    public List<Group> GetGroups()
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
}