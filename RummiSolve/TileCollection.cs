namespace RummiSolve;

public class TileCollection
{
    private readonly List<Tile> _tiles = [];

    private void AddTile(Tile tile)
    {
        _tiles.Add(tile);
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
        foreach (var tile in _tiles)
        {
            tile.PrintTile();
        }
    }

    private void SortTiles()
    {
        _tiles.Sort((x, y) =>
        {
            var colorComparison = x.TileColor.CompareTo(y.TileColor);
            return colorComparison != 0 ? colorComparison : x.Number.CompareTo(y.Number);
        });
    }
}