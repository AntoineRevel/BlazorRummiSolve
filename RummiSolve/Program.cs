namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        var tileCollection = new TileCollection();

        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            Console.WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                tileCollection.AddTilesFromInput(input, color);
            }
        }

        Console.WriteLine("All tiles in the collection:");
        tileCollection.PrintAllTiles();
        
        Console.WriteLine("Run:");
        foreach (var tiles in tileCollection.GetRuns())
        {
            tiles.PrintAllTiles();
        }
    }
}