using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public class CombinationsFirstSolver(List<Tile> tiles) : ISolver
{
    private List<Tile> _playerTiles = tiles;
    public bool Found { get; }
    public Solution BestSolution { get; }
    public IEnumerable<Tile> TilesToPlay { get; }
    public int JokerToPlay { get; }
    public bool Won { get; }

    public static CombinationsSolver Create(Set playerSet)
    {
        return new CombinationsSolver(playerSet.Tiles);
    }
    public void SearchSolution()
    {
        
    }
    
}