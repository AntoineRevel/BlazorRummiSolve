using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<RummiBench>();
    }
    
    public static void PlaySoloGame()
    {
        var game = new Game();
        game.PlaySoloGame();
    }

    public static void TestRand()
    {
        RummiBench.TestRandomValidSet();
    }

    public static void Play()
    {
        var game = new Game();
        game.StartConsole();
    }
    
}