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
        Game game = new(Guid.Parse("d67b3157-f0c1-42cd-8b8b-a3b8c2435304"));
        //Guid.Parse("9e1bf68d-78eb-436d-8ed9-c148e98a991b")

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    
    
}