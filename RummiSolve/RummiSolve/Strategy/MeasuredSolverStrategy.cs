using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.BestScore;
using RummiSolve.Solver.BestScore.First;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class MeasuredSolverStrategy : ISolverStrategy
{
    public async Task<SolverResult> GetSolverResult(Set board, Set rack, bool hasPlayed,
        CancellationToken externalToken)
    {
        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(new Set(board), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver bestScoreSolver = hasPlayed
            ? BestScoreComplexSolver.Create(new Set(board), rack)
            : BestScoreFirstBaseSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(new Set(board), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        ISolver incrementalScoreFSolver = hasPlayed
            ? IncrementalScoreFieldComplexSolver.Create(new Set(board), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        ISolver incrementalScoreTileSolver = hasPlayed
            ? IncrementalComplexSolverTileAndSc.Create(new Set(board), rack)
            : IncrementalFirstBaseSolver.Create(rack);

        var results = new List<TimedResult>
        {
            await RunSolverAsync("Combinations", combiSolver, externalToken),
            await RunSolverAsync("BestScore", bestScoreSolver, externalToken),
            await RunSolverAsync("Incremental", incrementalSolver, externalToken),
            await RunSolverAsync("IncrementalScoreField", incrementalScoreFSolver, externalToken),
            await RunSolverAsync("IncrementalScoreTile", incrementalScoreTileSolver, externalToken)
        };

        Console.WriteLine("\n====== Résultats des stratégies ======\n");

        foreach (var res in results)
        {
            Console.WriteLine($"🧠 {res.Name} - {res.TimeMs} ms - Found: {res.Result.Found}");
            if (res.Result.Found)
            {
                Console.Write("    ➤ Tiles: ");
                foreach (var tile in res.Result.TilesToPlay)
                    tile.PrintTile();
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("    ➤ Aucun coup trouvé.");
            }

            Console.WriteLine();
        }

        Console.WriteLine("======================================\n");


        var best = results.OrderBy(r => r.TimeMs).FirstOrDefault();

        return best?.Result ?? throw new InvalidOperationException();
    }

    private static async Task<TimedResult> RunSolverAsync(string name, ISolver solver, CancellationToken token)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await Task.Run(() => solver.SearchSolution(token), token);
        stopwatch.Stop();
        return new TimedResult(name, result, stopwatch.ElapsedMilliseconds);
    }

    private record TimedResult(string Name, SolverResult Result, long TimeMs);
}