using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("test");
    }


    public static void TestRand()
    {
        RummiBench.TestRandomValidSet();
    }
}