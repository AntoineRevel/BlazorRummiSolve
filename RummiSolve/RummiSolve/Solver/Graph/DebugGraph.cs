namespace RummiSolve.Solver.Graph;

public class DebugGraph
{
    public static void Test()
    {
        var set1 = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(4, TileColor.Red),
            new Tile(4, TileColor.Mango),
            new Tile(true)
        ]);

        var set2 = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
            new Tile(4),
            new Tile(8),
            new Tile(11),
            new Tile(11),
            new Tile(13),
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Black),
            new Tile(true)
        ]);

        var gs = GraphFirstSolver.Create(set2);
        gs.SearchSolution();
    }
}