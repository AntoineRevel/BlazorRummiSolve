using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class GameTests
{
    [Fact]
    public void AllTiles_ShouldRemainAccountedFor_ThroughoutEntireGame()
    {
        // Arrange
        var game = new SimpleGame(Guid.Parse("8f53d490-db85-4962-8886-8a49c0e2afb8"));
        var playerNames = new List<string> { "Antoine", "Matthieu", "Maguy" };

        // Act & Assert - Initialisation
        game.InitializeGame(playerNames);
        Assert.Equal(106, game.AllTiles());

        while (!game.IsGameOver)
        {
            // Act
            game.Play();

            // Assert
            Assert.Equal(106, game.AllTiles());
        }

        // Vérification finale
        Assert.Equal(106, game.AllTiles());
    }
}