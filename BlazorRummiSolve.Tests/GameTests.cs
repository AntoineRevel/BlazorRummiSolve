using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class GameTests
{
    [Fact]
    public async Task AllTiles_ShouldRemainAccountedFor_ThroughoutEntireGame()
    {
        // Arrange
        var game = new Game();

        var playerNames = new List<string> { "Antoine", "Matthieu", "Maguy" };

        // Act & Assert - Initialisation
        game.InitializeGame(playerNames);
        Assert.Equal(106, game.AllTiles());

        while (!game.IsGameOver)
        {
            // Act
            await game.PlayAsync();

            // Assert
            Assert.Equal(106, game.AllTiles());
        }

        // Vérification finale
        Assert.True(game.AllTiles() == 106, $"game.Id = {game.Id}");
    }
}