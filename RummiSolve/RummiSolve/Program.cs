using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<AllSolversBenchmark>();
    }

    public static void TestBenchmark()
    {
        var gsb = new AllSolversBenchmark();

        gsb.Setup();

        var stopwatch = Stopwatch.StartNew();
        gsb.CombinationsSolver_Test();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");

        stopwatch = Stopwatch.StartNew();
        gsb.IncrementalComplexSolverTileAndSc_Test();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");

        stopwatch = Stopwatch.StartNew();
        gsb.OptimizedIncrementalComplexSolver_Test();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");
    }
}