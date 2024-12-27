namespace RummiSolve.Solver.Interfaces;

public interface ISolverBase
{
    protected internal Solution BestSolution { get; }
    IEnumerable<Tile> TilesToPlay { get; }
    bool SearchSolution();
}