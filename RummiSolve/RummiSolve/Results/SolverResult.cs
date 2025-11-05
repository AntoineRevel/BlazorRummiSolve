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
        Score = 0;
    }

    public string Source { get; private init; }
    public bool Found { get; private init; }
    public Solution BestSolution { get; private init; }
    public IEnumerable<Tile> TilesToPlay { get; private init; }
    public int JokerToPlay { get; private init; }
    public int Score { get; private init; }

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

    // Factory method pour créer un résultat à partir d'une solution
    public static SolverResult FromSolution(string source, Solution solution, IEnumerable<Tile> tilesToPlay,
        int jokerToPlay = 0, int score = 0)
    {
        if (solution.IsValid)
            return new SolverResult
            {
                Source = source,
                Found = solution.IsValid,
                BestSolution = solution,
                TilesToPlay = tilesToPlay,
                JokerToPlay = jokerToPlay,
                Score = score
            };
        return Invalid(source);
    }
}