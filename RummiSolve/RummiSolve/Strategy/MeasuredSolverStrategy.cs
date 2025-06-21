using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategy;

public class MeasuredSolverStrategy : ISolverStrategy
{
    public async Task<SolverResult> GetSolverResult(Solution boardSolution, Set rack, bool hasPlayed,
        CancellationToken externalToken)
    {
        ISolver combiSolver = hasPlayed
            ? CombinationsSolver.Create(boardSolution.GetSet(), rack)
            : CombinationsFirstSolver.Create(rack);

        ISolver incrementalSolver = hasPlayed
            ? IncrementalComplexSolver.Create(boardSolution.GetSet(), rack)
            : IncrementalFirstBaseSolver.Create(rack);


        var combiTimer = Stopwatch.StartNew();
        var combiTask = Task.Run(() =>
        {
            var result = combiSolver.SearchSolution();
            combiTimer.Stop();
            return result;
        }, externalToken);

        var incrementalTimer = Stopwatch.StartNew();
        var incrementalTask = Task.Run(() =>
        {
            var result = incrementalSolver.SearchSolution();
            incrementalTimer.Stop();
            return result;
        }, externalToken);

        // Attendre les deux
        await Task.WhenAll(combiTask, incrementalTask);

        var combiResult = combiTask.Result;
        var incrementalResult = incrementalTask.Result;

        Console.WriteLine(
            $"🧠 CombiSolver: {combiResult.Source} - {combiTimer.ElapsedMilliseconds} ms - Found = {combiResult.Found}");

        foreach (var tile in combiResult.TilesToPlay) tile.PrintTile();

        Console.WriteLine();
        Console.WriteLine();
        combiResult.BestSolution.PrintSolution();

        Console.WriteLine(
            $"🧠 IncrementalSolver: {incrementalResult.Source} - {incrementalTimer.ElapsedMilliseconds} ms - Found = {incrementalResult.Found}");
        foreach (var tile in incrementalResult.TilesToPlay) tile.PrintTile();

        Console.WriteLine();
        Console.WriteLine();
        incrementalResult.BestSolution.PrintSolution();


        return incrementalResult;
    }
}