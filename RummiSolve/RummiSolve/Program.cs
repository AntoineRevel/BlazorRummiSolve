namespace RummiSolve;

public static class Program
{
    private static async Task Main()
    {
        //BenchmarkRunner.Run<ParallelSolverBenchmark>();

        await RummiBench.TestSimpleGame2();
    }
}