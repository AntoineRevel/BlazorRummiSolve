namespace RummiSolve.Solver.Interfaces;

public interface IBinarySolver : ISolverBase
{
    new IEnumerable<Tile> TilesToPlay { get; init; }
}