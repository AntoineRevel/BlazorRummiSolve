namespace RummiSolve.Solver;

public abstract class SolverBase(Tile[] tiles, int jokers)
{
    protected readonly Tile[] Tiles = tiles;
    protected readonly bool[] UsedTiles = new bool[tiles.Length];
    protected int Jokers = jokers;
    
    public Solution BestSolution { get; protected set; } = new();

    protected abstract bool ValidateCondition();
    
    
}