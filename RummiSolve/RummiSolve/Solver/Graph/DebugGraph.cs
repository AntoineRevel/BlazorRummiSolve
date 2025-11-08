namespace RummiSolve.Solver.Graph;

public class DebugGraph
{
    public static void Test()
    {
        var tiles1 = new[]
        {
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(4, TileColor.Red),
            new Tile(4, TileColor.Mango)
        };

        var tiles2 = new[]
        {
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
            new Tile(4),
            new Tile(6),
            new Tile(11),
            new Tile(11, TileColor.Red),
            new Tile(13),
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Black)
        };

        var gs = new GraphSolver(tiles1, 1);
        gs.Test();
    }
}