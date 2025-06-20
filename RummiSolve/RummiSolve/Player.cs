using RummiSolve.Solver;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;
using static System.Console;

namespace RummiSolve;

public class Player(string name, List<Tile> tiles) //, ISolverStrategy solverStrategy)
{
    public bool Played { get; private set; }
    public string Name { get; } = name;

    public Set Rack { get; } = new(tiles);
    public List<Tile> TilesToPlay { get; private set; } = [];
    public bool Won { get; private set; }

    //TODO private readonly ISolverStrategy _solverStrategy =solverStrategy;

    public Solution Solve(Solution boardSolution)
    {
        TilesToPlay.Clear();
        using var cts = new CancellationTokenSource();

        ISolver combiSolver = Played
            ? CombinationsSolver.Create(boardSolution.GetSet(), Rack)
            : CombinationsFirstSolver.Create(Rack);

        ISolver incrementalSolver = Played
            ? IncrementalComplexSolver.Create(boardSolution.GetSet(), Rack)
            : IncrementalFirstBaseSolver.Create(Rack);

        var incrementalTask = Task.Run(() => incrementalSolver.SearchSolution(), cts.Token);

        var combiTask = Task.Run(() => combiSolver.SearchSolution(), cts.Token);

        var completedTask = Task.WaitAny(incrementalTask, combiTask);

        cts.Cancel();

        var winner = completedTask == 0
            ? (solverTask: incrementalTask, name: "Incremental")
            : (solverTask: combiTask, name: "Combi");

        WriteLine($"{winner.name} First");
        return ProcessSolution(boardSolution, winner.solverTask.Result);
    }

    private Solution ProcessSolution(Solution boardSolution, SolverResult result)
    {
        if (!result.Found) return Solution.InvalidSolution;

        Won = result.Won;

        TilesToPlay = result.TilesToPlay.ToList();

        for (var i = 0; i < result.JokerToPlay; i++) TilesToPlay.Add(new Tile(true));

        if (Played) return result.BestSolution;

        Played = true;
        return result.BestSolution.AddSolution(boardSolution);
    }

    public void Play()
    {
        WriteLine("Play : ");
        foreach (var tile in TilesToPlay)
        {
            Rack.Remove(tile);
            tile.PrintTile();
        }

        WriteLine();
    }

    public void Drew(Tile tile)
    {
        Rack.AddTile(tile);

        Write("Drew : ");
        tile.PrintTile();
        WriteLine();
    }
}