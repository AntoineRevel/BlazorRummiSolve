namespace RummiSolve;

public static class Program
{
    private static void Main()
    {
        //BenchmarkRunner.Run<RummiBench>();

        var tilesToHighlight = new List<Tile>();

        var matchingTile = tilesToHighlight.FirstOrDefault(t => t.Equals(new Tile(3, TileColor.Red)));

        matchingTile.PrintTile();
    }
}