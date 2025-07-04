using BenchmarkDotNet.Attributes;
using RummiSolve.Results;
using RummiSolve.Strategy;

namespace RummiSolve;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90, iterationCount: 3, warmupCount: 2)]
public class ParallelSolverBenchmark
{
    private readonly List<string> _playerNames = ["Antoine", "Matthieu", "Maguy"];
    private readonly Guid _gameId = Guid.Parse("32ba6c8d-3bb2-41f6-8292-655f9459d53c");

    [Benchmark]
    public async Task<bool> ParallelSolver_CompleteGame()
    {

        var game = new Game(_gameId);
        game.InitializeGame(_playerNames);
        while (!game.IsGameOver) await game.PlayAsync();
        return game.IsGameOver;
    }
}