using RummiSolve.Solver.Graph.First;

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

        var fourTilesOneExtra = new Set([
            new Tile(1),
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var gs = GraphFirstSolver.Create(fourTilesOneExtra);
        gs.SearchSolution();
    }


    public static void Test2()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void Test3()
    {
        var boardSet = new Set([
            new Tile(2),
            new Tile(3),
            new Tile(4)
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void Test4()
    {
        var boardSet = new Set([
            new Tile(2),
            new Tile(3),
            new Tile(4)
        ]);

        var playerSet = new Set([
            new Tile(true)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void Test5()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black),
            new Tile(5, TileColor.Red),
            new Tile(true)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void Test6()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void Test7Simple()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void TestInvalidAllBoardNotPlayed()
    {
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void TestValidNotWon2()
    {
        var boardSet = new Set([
            new Tile(8),
            new Tile(9),
            new Tile(10),
            new Tile(11)
        ]);

        var playerSet = new Set([
            new Tile(8, TileColor.Black),
            new Tile(8, TileColor.Red),
            new Tile(1)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }

    public static void TestValidNotWon2Id()
    {
        var boardSet = new Set([
            new Tile(8),
            new Tile(9),
            new Tile(10),
            new Tile(11)
        ]);

        var playerSet = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(4),
            new Tile(1, TileColor.Red),
            new Tile(1, TileColor.Black)
        ]);

        var gs = GraphSolver.Create(boardSet, playerSet);

        var sr = gs.SearchSolution();

        sr.BestSolution.PrintSolution();

        foreach (var tile in sr.TilesToPlay) tile.PrintTile();

        Console.WriteLine(sr.BestSolution.IsValid);
    }
}