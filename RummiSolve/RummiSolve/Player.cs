using RummiSolve.Solver;
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
    public int JokersToPlay;

    public Player(string name, List<Tile> tiles)
    {
        Name = name;
        JokersToPlay = 0;
        _rackTilesSet = new Set(tiles); //pas de copie de la liste TODO
        _lastRackTilesSet = new Set(tiles);
        RackTileToShow = _lastRackTilesSet;
        PlayedToShow = false;
    }

    public bool Won { get; private set; }


    public Solution Solve(Solution boardSolution)
    {
        TilesToPlay.Clear();
        var boardSet = boardSolution.GetSet();
        
        ISolver solver = _played
            ? new FullSolutionWithScoreSolver(boardSet, _rackTilesSet)
            : IncrementalFirstSolver.Create(_rackTilesSet);

        return Solve(boardSolution, solver);
    }
    
    private Solution Solve(Solution boardSolution, ISolver solver)
    {
        solver.SearchSolution();
        
        if (!solver.Found) return Solution.GetInvalidSolution();
        Won = solver.Won;
        
        var solution = solver.BestSolution;
        TilesToPlay = solver.TilesToPlay.ToList();
        JokersToPlay = solver.JokerToPlay;

        
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
        for (var i = 0; i < JokersToPlay; i++) _rackTilesSet.Remove(new Tile(true));
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