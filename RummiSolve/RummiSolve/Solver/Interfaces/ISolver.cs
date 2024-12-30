namespace RummiSolve.Solver.Interfaces;

public interface ISolver
{
    protected internal Solution BestSolution { get; }
    IEnumerable<Tile> TilesToPlay { get; }
    
    bool Won { get; }
    void SearchSolution();
}