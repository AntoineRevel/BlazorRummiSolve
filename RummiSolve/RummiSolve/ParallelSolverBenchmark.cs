using BenchmarkDotNet.Attributes;
using RummiSolve.Results;
using RummiSolve.Strategy;

namespace RummiSolve;

[MemoryDiagnoser]
[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net90, iterationCount: 3, warmupCount: 2)]
public class ParallelSolverBenchmark
{
    private readonly List<string> _playerNames = ["Antoine", "Matthieu", "Maguy"];
    private readonly Guid _gameId = Guid.Parse("8f53d490-db85-4962-8886-8a49c0e2afb8");

    [Benchmark]
    public async Task<SolverResult> ParallelSolver_FirstTurn()
    {
        // Premier tour - rack initial avec ID fixe
        var game = new Game(_gameId);
        game.InitializeGame(_playerNames);
        
        var initialPlayer = game.Players[game.PlayerIndex];
        var strategy = new ParallelSolverStrategy();
        
        return await strategy.GetSolverResult(new Solution(), initialPlayer.Rack, false, CancellationToken.None);
    }

    [Benchmark]
    public async Task<SolverResult> ParallelSolver_MidGame()
    {
        // Milieu de partie - plateau avec quelques sets
        var midGameBoard = new Solution();
        midGameBoard.AddRun(new Run { Tiles = [new Tile(1), new Tile(2), new Tile(3)] });
        midGameBoard.AddGroup(new Group { Tiles = [new Tile(7), new Tile(7, TileColor.Red), new Tile(7, TileColor.Black)] });
        
        var midGameRack = new Set([
            new Tile(4), new Tile(8), new Tile(11), new Tile(13),
            new Tile(13, TileColor.Red), new Tile(2), new Tile(5),
            new Tile(9), new Tile(12)
        ]);
        
        var strategy = new ParallelSolverStrategy();
        return await strategy.GetSolverResult(midGameBoard, midGameRack, true, CancellationToken.None);
    }

    [Benchmark]
    public async Task<SolverResult> ParallelSolver_EndGame()
    {
        // Fin de partie - plateau complexe
        var endGameBoard = new Solution();
        endGameBoard.AddRun(new Run { Tiles = [new Tile(1), new Tile(2), new Tile(3)] });
        endGameBoard.AddRun(new Run { Tiles = [new Tile(10, TileColor.Red), new Tile(11, TileColor.Red), new Tile(12, TileColor.Red)] });
        endGameBoard.AddGroup(new Group { Tiles = [new Tile(7), new Tile(7, TileColor.Red), new Tile(7, TileColor.Black)] });
        endGameBoard.AddGroup(new Group { Tiles = [new Tile(13), new Tile(13, TileColor.Black), new Tile(13, TileColor.Red)] });
        endGameBoard.AddGroup(new Group { Tiles = [new Tile(4), new Tile(4, TileColor.Red), new Tile(4, TileColor.Black)] });
        
        var endGameRack = new Set([
            new Tile(4, TileColor.Black), new Tile(8), new Tile(3, TileColor.Mango),
            new Tile(8), new Tile(2, TileColor.Black)
        ]);
        
        var strategy = new ParallelSolverStrategy();
        return await strategy.GetSolverResult(endGameBoard, endGameRack, true, CancellationToken.None);
    }
}