namespace RummiSolve;

public static class Program
{
    private static async Task Main()
    {
        //BenchmarkRunner.Run<RummiBench>();

        await RummiBench.RunGamesUntilErrorAsync();
    }
}