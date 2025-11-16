using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<GraphSolverBenchmark>();
    }

    public static void TestBenchmark()
    {
        var gsb = new GraphSolverBenchmark();

        gsb.Setup();

        var stopwatch = Stopwatch.StartNew();
        gsb.GraphSolver_Turn8Config();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");
    }
}