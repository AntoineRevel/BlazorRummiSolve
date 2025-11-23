using BenchmarkDotNet.Attributes;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Incremental;

namespace RummiSolve;

[MemoryDiagnoser]
[SimpleJob(iterationCount: 3, warmupCount: 2)]
[RankColumn]
public class AllSolversBenchmark
{
    private Set _boardSet = null!;
    private Set _playerSet = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Board configuration from the game image (Turn 8)
        _boardSet = new Set();

        // Run 1: 8, 9, 10 (Black)
        _boardSet.AddTile(new Tile(8, TileColor.Black));
        _boardSet.AddTile(new Tile(9, TileColor.Black));
        _boardSet.AddTile(new Tile(10, TileColor.Black));

        // Run 2: 6, 7, 8, 9, 10 (Mango/Orange)
        _boardSet.AddTile(new Tile(6, TileColor.Mango));
        _boardSet.AddTile(new Tile(7, TileColor.Mango));
        _boardSet.AddTile(new Tile(8, TileColor.Mango));
        _boardSet.AddTile(new Tile(9, TileColor.Mango));
        _boardSet.AddTile(new Tile(10, TileColor.Mango));

        // Run 3: 11, 12, 13 (Red)
        _boardSet.AddTile(new Tile(11, TileColor.Red));
        _boardSet.AddTile(new Tile(12, TileColor.Red));
        _boardSet.AddTile(new Tile(13, TileColor.Red));

        // Run 4: 8, 9, 10 (Red)
        _boardSet.AddTile(new Tile(8, TileColor.Red));
        _boardSet.AddTile(new Tile(9, TileColor.Red));
        _boardSet.AddTile(new Tile(10, TileColor.Red));

        // Run 5: 5, 6, 7, 8, 9 (Red)
        _boardSet.AddTile(new Tile(5, TileColor.Red));
        _boardSet.AddTile(new Tile(6, TileColor.Red));
        _boardSet.AddTile(new Tile(7, TileColor.Red));
        _boardSet.AddTile(new Tile(8, TileColor.Red));
        _boardSet.AddTile(new Tile(9, TileColor.Red));

        // Run 6: 10, 11, 12 (Blue)
        _boardSet.AddTile(new Tile(10));
        _boardSet.AddTile(new Tile(11));
        _boardSet.AddTile(new Tile(12));

        // Run 7: 5, 6, 7 (Blue)
        _boardSet.AddTile(new Tile(5));
        _boardSet.AddTile(new Tile(6));
        _boardSet.AddTile(new Tile(7));

        // Run 8: 1, 2, 3, 4 (Blue)
        _boardSet.AddTile(new Tile(1));
        _boardSet.AddTile(new Tile(2));
        _boardSet.AddTile(new Tile(3));
        _boardSet.AddTile(new Tile(4));

        // Group 1: 2, 2, 2 (Red, Mango, Black)
        _boardSet.AddTile(new Tile(2, TileColor.Red));
        _boardSet.AddTile(new Tile(2, TileColor.Mango));
        _boardSet.AddTile(new Tile(2, TileColor.Black));

        // Group 2: 6, 6, 6 (Blue, Mango, Black)
        _boardSet.AddTile(new Tile(6));
        _boardSet.AddTile(new Tile(6, TileColor.Mango));
        _boardSet.AddTile(new Tile(6, TileColor.Black));

        // Group 3: 5, 5, 5 (Blue, Red, Mango)
        _boardSet.AddTile(new Tile(5));
        _boardSet.AddTile(new Tile(5, TileColor.Red));
        _boardSet.AddTile(new Tile(5, TileColor.Mango));

        // Group 4: 3, 3, 3 (Blue, Red, Mango)
        _boardSet.AddTile(new Tile(3));
        _boardSet.AddTile(new Tile(3, TileColor.Red));
        _boardSet.AddTile(new Tile(3, TileColor.Mango));

        // Bob's rack (player tiles)
        _playerSet = new Set();
        _playerSet.AddTile(new Tile(13, TileColor.Black));
        _playerSet.AddTile(new Tile(11));
        _playerSet.AddTile(new Tile(12, TileColor.Mango));
        _playerSet.AddTile(new Tile(4, TileColor.Mango));
        _playerSet.AddTile(new Tile(7, TileColor.Black));
        _playerSet.AddTile(new Tile(7, TileColor.Black));
        _playerSet.AddTile(new Tile(1, TileColor.Red));
        _playerSet.AddTile(new Tile(4, TileColor.Mango));
    }

    // Combinations Solvers
    [Benchmark]
    public void CombinationsSolver_Test()
    {
        var solver = CombinationsSolver.Create(_boardSet, _playerSet);
        solver.SearchSolution();
    }

    [Benchmark]
    public void IncrementalComplexSolverTileAndSc_Test()
    {
        var solver = IncrementalComplexSolverTileAndSc.Create(_boardSet, _playerSet);
        solver.SearchSolution();
    }

    [Benchmark]
    public void OptimizedIncrementalComplexSolver_Test()
    {
        var solver = OptimizedIncrementalComplexSolver.Create(_boardSet, _playerSet);
        solver.SearchSolution();
    }
}