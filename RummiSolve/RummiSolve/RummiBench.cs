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
        Game game = new(Guid.Parse("1f0a8141-2f10-4b05-bf82-f3725c713eff"));
        //Guid.Parse("f194c3b0-8088-452c-b180-66614ba2f6d7")

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
            new Tile(8,TileColor.Black),
            new Tile(9,TileColor.Black),
            new Tile(10,TileColor.Black),
            
            new Tile(4,TileColor.Black),
            new Tile(5,TileColor.Black),
            new Tile(6,TileColor.Black),
            
            new Tile(9,TileColor.Mango),
            new Tile(10,TileColor.Mango),
            new Tile(11,TileColor.Mango),
            
            new Tile(5,TileColor.Red),
            new Tile(6,TileColor.Red),
            new Tile(7,TileColor.Red),
            new Tile(true),
            new Tile(9,TileColor.Red),
            
            new Tile(10),
            new Tile(10,TileColor.Mango),
            new Tile(10,TileColor.Red),
            
            new Tile(3),
            new Tile(3,TileColor.Black),
            new Tile(3,TileColor.Red),
            
            new Tile(6,TileColor.Black),
            new Tile(13,TileColor.Black),
            new Tile(7,TileColor.Black),
            new Tile(4,TileColor.Red),
            new Tile(9),
            new Tile(2,TileColor.Red),
            new Tile(4,TileColor.Red),
            new Tile(12,TileColor.Black),
            new Tile(3,TileColor.Mango),
            new Tile(4,TileColor.Black),
            new Tile(5,TileColor.Black),
            new Tile(5,TileColor.Red),
            new Tile(12,TileColor.Black),
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