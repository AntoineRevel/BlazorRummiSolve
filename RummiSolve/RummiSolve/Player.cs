using RummiSolve.Results;
using RummiSolve.Strategies;
using static System.Console;

namespace RummiSolve;

public class Player(string name, List<Tile> tiles, ISolverStrategy solverStrategy)
{
    public bool Played { get; private set; }
    public string Name { get; } = name;
    public Set Rack { get; } = new(tiles);
    public List<Tile> TilesToPlay { get; private set; } = [];
    public bool Won { get; private set; }

    public async Task<Solution> SolveAsync(Solution boardSolution,
        CancellationToken externalCancellationToken = default)
    {
        TilesToPlay.Clear();

        var solver = await solverStrategy.GetSolverResult(boardSolution.GetSet(), new Set(Rack), Played,
            externalCancellationToken);

        WriteLine($"{solver.Source} Selected");

        return ProcessSolution(boardSolution, solver);
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