using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public class FullSolutionWithScoreSolver(Set boardSet, Set rackTilesSet) : ISolver
{
    public bool Found  { get;  private set;}
    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay { get; private set; } = [];
    public int JokerToPlay { get;  private set;}
    public bool Won { get; private set; }
    public void SearchSolution()
    {
        IScoreSolver scoreSolver =  ScoreSolver.Create(boardSet, rackTilesSet);

        var canPlay = scoreSolver.SearchSolution();
        
        if (!canPlay) return;
        Found = true;

        var solver = SolutionWithScoreSolver.Create(boardSet, rackTilesSet, scoreSolver.BestScore);
        solver.SearchSolution();
        Won = solver.Won;
        BestSolution = solver.BestSolution;
        TilesToPlay = solver.TilesToPlay;
        JokerToPlay = solver.JokerToPlay;

        
    }
    
    
}