using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class GameTests
{
    [Fact]
    public void GameTest()
    {
        SimpleGame game = new(Guid.Parse("8f53d490-db85-4962-8886-8a49c0e2afb8"));

        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);

        game.Play();
    }
}