using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<RummiBench>();
    }
    

    public static void TestRand()
    {
        RummiBench.TestRandomValidSet();
    }
    
}