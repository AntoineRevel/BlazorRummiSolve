namespace RummiSolve.Solver;

public class SolverResult
{
    public SolverResult(Solution bestSolution, IEnumerable<Tile> tilesToPlay, int jokerToPlay, bool won = false)
    {
        Found = bestSolution.IsValid;
        BestSolution = bestSolution;
        TilesToPlay = tilesToPlay;
        JokerToPlay = jokerToPlay;
        Won = won;
    }

    private SolverResult()
    {
        Found = false;
        BestSolution = Solution.InvalidSolution;
        TilesToPlay = [];
    }

    public bool Found { get; }
    public Solution BestSolution { get; }
    public IEnumerable<Tile> TilesToPlay { get; }
    public int JokerToPlay { get; }
    public bool Won { get; }

    public static SolverResult Invalid { get; } = new();
}