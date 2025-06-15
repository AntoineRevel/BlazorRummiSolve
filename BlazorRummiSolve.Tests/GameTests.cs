using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class GameTests
{
    [Fact]
    public void AllTiles_ShouldRemainAccountedFor_ThroughoutEntireGame()
    {
        // Arrange
        var game = new SimpleGame();
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