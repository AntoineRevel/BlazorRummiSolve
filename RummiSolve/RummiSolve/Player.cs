using RummiSolve.Solver;
using RummiSolve.Solver.BestScore;
using RummiSolve.Solver.BestScore.First;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;
using RummiSolve.Solver.Incremental;
using RummiSolve.Solver.Interfaces;
using static System.Console;

namespace RummiSolve;

public class Player
{
    public readonly string Name;

    private readonly Set _rackTilesSet;

    private Tile _lastDrewTile;
    private Set _lastRackTilesSet;
    private bool _played;
    public bool PlayedToShow;
    public Set RackTileToShow;
    public List<Tile> TilesToPlay = [];

    public Player(string name, List<Tile> tiles)
    {
        Name = name;
        _rackTilesSet = new Set(tiles); //pas de copie de la liste TODO
        _lastRackTilesSet = new Set(tiles);
        RackTileToShow = _lastRackTilesSet;
        PlayedToShow = false;
    }

    public bool Won { get; private set; }


    public Solution SolveCombi(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();

        ISolver incrementalSolver = _played
            ? CombinationsSolver.Create(boardSet, _rackTilesSet)
            : CombinationsFirstSolver.Create(_rackTilesSet);

        incrementalSolver.SearchSolution();
        return Solve(boardSolution, incrementalSolver);
    }

    public Solution SolveIncr(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();

        ISolver incrementalSolver = _played
            ? IncrementalComplexSolver.Create(boardSet, _rackTilesSet)
            : IncrementalFirstBaseSolver.Create(_rackTilesSet);

        incrementalSolver.SearchSolution();
        return Solve(boardSolution, incrementalSolver);
    }

    public Solution SolveIncrTileAndSc(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();

        ISolver incrementalSolver = _played
            ? IncrementalComplexSolverTileAndSc.Create(boardSet, _rackTilesSet)
            : IncrementalFirstBaseSolver.Create(_rackTilesSet);

        incrementalSolver.SearchSolution();
        return Solve(boardSolution, incrementalSolver);
    }

    public Solution SolveBs(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();

        ISolver bestScoreSolver = _played
            ? BestScoreComplexSolver.Create(boardSet, _rackTilesSet)
            : BestScoreFirstBaseSolver.Create(_rackTilesSet);

        bestScoreSolver.SearchSolution();
        return Solve(boardSolution, bestScoreSolver);
    }

    public async Task<Solution> Solve(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();

        ISolver combiSolver = _played
            ? CombinationsSolver.Create(boardSet, _rackTilesSet)
            : CombinationsFirstSolver.Create(_rackTilesSet);

        ISolver incrementalSolver = _played
            ? IncrementalComplexSolver.Create(boardSet, _rackTilesSet)
            : IncrementalFirstBaseSolver.Create(_rackTilesSet);

        using var cts = new CancellationTokenSource();

        var tasks = new[]
        {
            Task.Run(() => incrementalSolver.SearchSolution(), cts.Token),
            Task.Run(() => combiSolver.SearchSolution(), cts.Token)
        };

        try 
        {
            var completedTask = await Task.WhenAny(tasks);
        
            // Annuler les autres tâches
            await cts.CancelAsync();

            var winner = completedTask == tasks[0] 
                ? (solver: incrementalSolver, name: "Incremental") 
                : (solver: combiSolver, name: "Combi");

            WriteLine($"{winner.name} First");
            return Solve(boardSolution, winner.solver);
        }
        finally
        {
            // S'assurer que toutes les tâches sont annulées
            await cts.CancelAsync();
        }
    }

    private Solution Solve(Solution boardSolution, ISolver solver)
    {
        if (!solver.Found) return Solution.GetInvalidSolution();
        Won = solver.Won;

        var solution = solver.BestSolution;

        TilesToPlay = solver.TilesToPlay.ToList();

        WriteLine("Play :");
        foreach (var tile in TilesToPlay) tile.PrintTile();
        WriteLine();

        for (var i = 0; i < solver.JokerToPlay; i++) TilesToPlay.Add(new Tile(true));

        if (_played) return solution;

        solution.AddSolution(boardSolution);

        _played = true;

        return solution;
    }


    public void SaveRack()
    {
        _lastRackTilesSet = new Set(_rackTilesSet);
        RackTileToShow = _lastRackTilesSet;
    }

    public void RemoveTilePlayed()
    {
        foreach (var tile in TilesToPlay) _rackTilesSet.Remove(tile);
    }

    public void ShowRemovedTile()
    {
        RackTileToShow = _rackTilesSet;
        PlayedToShow = _played;
    }

    public void ShowLastTile()
    {
        RackTileToShow = _lastRackTilesSet;
    }

    public void AddTileToRack(Tile tile)
    {
        _rackTilesSet.AddTile(tile);
    }

    public void SetLastDrewTile(Tile tile)
    {
        _lastDrewTile = tile;
    }

    public void PrintRackTiles()
    {
        if (!Won)
        {
            WriteLine(Name + " tiles : ");
            _rackTilesSet.Tiles.ForEach(t => t.PrintTile());
        }
        else
        {
            WriteLine(Name + " Win !!!");
        }

        WriteLine();
    }
}