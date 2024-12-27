namespace RummiSolve.Solver.Interfaces;

public interface IIncrementalSolver : ISolverBase
{
    protected internal int JokerToPlay { get; }
}