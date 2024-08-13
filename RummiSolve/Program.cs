namespace RummiSolve;

public static class Program
{
    public static void Main()
    {
        var game = new Game();
        game.Initialize();
        
        game.PrintAllTiles();
        
        game.Solve();
    }
    
}