namespace RummiSolve.Solver.Interfaces;

public interface IBinarySolver
{
    IEnumerable<Tile> TilesToPlay { get; }
    int JokerToPlay { get; }
    Solution BinarySolution { get; }
}