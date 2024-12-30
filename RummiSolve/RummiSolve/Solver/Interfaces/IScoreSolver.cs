namespace RummiSolve.Solver.Interfaces;

public interface IScoreSolver
{
    protected internal int BestScore { get; }
    bool SearchSolution();
}