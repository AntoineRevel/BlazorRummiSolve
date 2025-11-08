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

        // set4: Combinaisons valides mais < 30 points
        // 1-2-3 bleue (6 points) + 4-4-4 (12 points) + quelques tuiles isolées
        var set4 = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(4, TileColor.Red),
            new Tile(4, TileColor.Mango),
            new Tile(7, TileColor.Black),
            new Tile(9, TileColor.Red),
            new Tile(11, TileColor.Mango),
            new Tile(13),
            new Tile(5, TileColor.Red),
            new Tile(8, TileColor.Black),
            new Tile(12, TileColor.Mango),
            new Tile(6)
        ]);

        // set5: Combinaisons qui font exactement 30 points
        // 10-10-10 (30 points exact) + quelques tuiles pour compléter
        var set5 = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Mango),
            new Tile(11),
            new Tile(12),
            new Tile(13),
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Red),
            new Tile(7, TileColor.Red),
            new Tile(3, TileColor.Black),
            new Tile(4, TileColor.Black),
            new Tile(8, TileColor.Mango),
            new Tile(9, TileColor.Mango),
            new Tile(1),
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(4, TileColor.Red),
            new Tile(4, TileColor.Mango),
            new Tile(7, TileColor.Black),
            new Tile(9, TileColor.Red),
            new Tile(11, TileColor.Mango),
            new Tile(13),
            new Tile(5, TileColor.Red),
            new Tile(8, TileColor.Black),
            new Tile(12, TileColor.Mango),
            new Tile(6)
        ]);

        var gs = GraphFirstSolver.Create(set1);
        gs.SearchSolution();
    }
}