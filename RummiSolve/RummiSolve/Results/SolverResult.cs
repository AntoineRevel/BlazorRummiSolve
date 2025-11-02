namespace RummiSolve.Results;

public class SolverResult
{
    // Constructeur privé pour forcer l'utilisation des factory methods
    private SolverResult()
    {
        Source = string.Empty;
        Found = false;
        BestSolution = new Solution();
        TilesToPlay = [];
        JokerToPlay = 0;
        Won = false;
        Score = 0;
    }

    public string Source { get; private init; }
    public bool Found { get; private init; }
    public Solution BestSolution { get; private init; }
    public IEnumerable<Tile> TilesToPlay { get; private init; }
    public int JokerToPlay { get; private init; }
    public bool Won { get; set; }
    public int Score { get; set; }

    // Factory method pour les échecs
    public static SolverResult Invalid(string source)
    {
        return new SolverResult
        {
            Source = source,
            Found = false,
            BestSolution = new Solution(),
            TilesToPlay = []
        };
    }

    // Factory method pour les succès
    public static SolverResult Valid(string source, Solution solution, IEnumerable<Tile> tilesToPlay,
        int jokerToPlay = 0, bool won = false, int score = 0)
    {
        return new SolverResult
        {
            Source = source,
            Found = solution.IsValid,
            BestSolution = solution,
            TilesToPlay = tilesToPlay,
            JokerToPlay = jokerToPlay,
            Won = won,
            Score = score
        };
    }
}