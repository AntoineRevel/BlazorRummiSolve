using System.Xml;
using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<RummiBench>();
        //TestBench();
    }

    private static void TestBench()
    {
        var bench = new RummiBench();
        bench.GetSolutions();

        Console.WriteLine(" array :");
        bench.GetSolutionsArray();
    }

    private static void Play()
    {
        var game = new Game();
        game.Start();
    }
}