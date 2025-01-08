using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public class CombinationsFirstSolver(List<Tile> tiles) : ISolver
{
    public bool Found { get; private set; }
    public Solution BestSolution { get; private set; }
    public IEnumerable<Tile> TilesToPlay { get; private set; }
    public int JokerToPlay { get; private set; }
    public bool Won { get; private set; }

    public static CombinationsFirstSolver Create(Set playerSet)
    {
        return new CombinationsFirstSolver(playerSet.Tiles);
    }
    
    public void SearchSolution()
    {
        tiles.Sort();
        
        var tilesFirstTry = new List<Tile>(tiles);
        
        var playerJokers = tiles.Count(tile => tile.IsJoker);
        
        if (playerJokers > 0) tilesFirstTry.RemoveRange(tilesFirstTry.Count - playerJokers, playerJokers);
        
        var firstBinarySolver = new BinaryFirstBaseSolver(tilesFirstTry.ToArray(), playerJokers);

        var isValid = firstBinarySolver.SearchSolution();

        if (isValid)
        {
            Found = true;
            BestSolution = firstBinarySolver.BinarySolution;
            TilesToPlay = tilesFirstTry;
            JokerToPlay = playerJokers;
            Won = true;
            return ;
        }
        
    }
    
}