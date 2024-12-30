namespace RummiSolve.Solver.Interfaces;

public interface ISolver
{
    bool Found { get; }
    Solution BestSolution { get; }
    IEnumerable<Tile> TilesToPlay { get; }
    int JokerToPlay { get; }
    bool Won { get; }
    void SearchSolution();
}