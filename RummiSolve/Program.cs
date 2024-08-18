using System.Xml;
using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<RummiBench>();
        //RummiBench.TestBench();
        //RummiBench.TestGroup();
    }



    private static void Play()
    {
        var game = new Game();
        game.Start();
    }
}