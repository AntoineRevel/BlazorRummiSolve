namespace RummiSolve.Results;

public class SolverResult
{
    public SolverResult(string source, Solution bestSolution, IEnumerable<Tile> tilesToPlay, int jokerToPlay,
        bool won = false)
    {
        Source = source ?? "Unknown";
        Found = bestSolution.IsValid;
        BestSolution = bestSolution;
        TilesToPlay = tilesToPlay;
        JokerToPlay = jokerToPlay;
        Won = won;
    }

    public SolverResult(string source)
    {
        Source = source;
        Found = false;
        BestSolution = Solution.InvalidSolution;
        TilesToPlay = [];
    }

    public string Source { get; }
    public bool Found { get; }
    public Solution BestSolution { get; }
    public IEnumerable<Tile> TilesToPlay { get; }
    public int JokerToPlay { get; }
    public bool Won { get; set; }
}