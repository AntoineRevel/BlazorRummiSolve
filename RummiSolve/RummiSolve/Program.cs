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

        gsb.GraphSolver_Turn8Config();
    }
}