using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<RummiBench>();
    }

    private static void Play()
    {
        var game = new Game();
        game.Start();
    }
    
}