using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        //BenchmarkRunner.Run<RummiBench>();
        TestBench();
    }

    public static void TestBench()
    {
        var rummiBench = new RummiBench();
        rummiBench.OldHand().PrintSolution();
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