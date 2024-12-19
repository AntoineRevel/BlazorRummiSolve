using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace RummiSolve;

[MemoryDiagnoser]
public class RummiBench
{
    
    public void TestMultiPlayerGameNoStatic()
    {
        var game = new Game();
        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }

    public static void TestMultiPlayerGame()
    {
        Game game = new(Guid.Parse("93ce558e-6da6-4d27-8c50-691d469e7f86"));
        //Guid.Parse("4e8b9e81-216b-4de6-8984-bcd7b7bbd3ac")

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    
    public static void TestSupMark2()
    {
        var set = new Set([
            new Tile(5, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(true), // Joker
            new Tile(8, TileColor.Black),
            new Tile(9, TileColor.Black),
            
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango),
            new Tile(13, TileColor.Mango),
            
            new Tile(9, TileColor.Mango),
            new Tile(10, TileColor.Mango),
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango),
            
            new Tile(8,TileColor.Mango),
            
        ]);


        Console.WriteLine(set.Tiles.Count);
        var gameStopwatch = Stopwatch.StartNew();
        var sol = set.GetSolution();
        gameStopwatch.Stop();

        sol.PrintSolution();
        Console.WriteLine(sol.GetSet().Tiles.Count);
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    
}