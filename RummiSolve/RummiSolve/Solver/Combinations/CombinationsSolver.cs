using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations;

public class CombinationsSolver(List<Tile> tiles) : ISolver
{
    private List<Tile> _tiles = tiles;
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