using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<AllSolversBenchmark>();
    }


    public static void AllSolversBenchmark()
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

    public static void GraphSolverBenchmark()
    {
        var gsb = new GraphSolverBenchmark();

        gsb.Setup();

        var stopwatch = Stopwatch.StartNew();
        gsb.GraphSolver_Turn8Config();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");

        stopwatch = Stopwatch.StartNew();
        gsb.SequentialGraphSolver_Turn8Config();
        stopwatch.Stop();

        Console.WriteLine(
            $"Temps d'exécution: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.Elapsed.TotalSeconds:F2} s)");

        stopwatch = Stopwatch.StartNew();
        gsb.SequentialGraphSolver_Turn8Config();
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