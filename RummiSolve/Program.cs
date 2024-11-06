using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        var listT = new List<Tile>
        {
            new(11, TileColor.Blue),
            new(13, TileColor.Black),
            new(10, TileColor.Blue),
            new(9, TileColor.Blue),
            new(6, TileColor.Blue),
            new(12, TileColor.Blue),
            new(8, TileColor.Blue),
            new(5, TileColor.Blue),
            new(13, TileColor.Mango),
            new(7, TileColor.Blue),
            new(13, TileColor.Blue),
        };

        var set = new Set()
        {
            Tiles = listT
        };

        set.GetSolution().PrintSolution();
        
    }
    
    
    
}