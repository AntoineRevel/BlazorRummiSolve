namespace RummiSolve.Solver.Graph;

public class DebugGraph
{
    public static void Test()
    {
        var tiles = new[]
        {
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(4, TileColor.Red),
            new Tile(4, TileColor.Mango)
        };

        var gs = new GraphSolver(tiles, 0);
        gs.Test();
    }
}