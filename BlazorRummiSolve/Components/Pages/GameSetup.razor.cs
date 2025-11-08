using Microsoft.AspNetCore.Components.Web;

namespace BlazorRummiSolve.Components.Pages;

public partial class GameSetup
{
    private string GameId { get; set; } = string.Empty;
    private List<string> PlayerNames { get; } = ["", ""];
    private List<bool> PlayerTypes { get; } = [true, true]; // true = Real, false = AI
    private bool HasGameIdError { get; set; }
    private string GameIdErrorMessage { get; set; } = string.Empty;
    private int PlayerCount => PlayerNames.Count;
    private bool CanAddPlayer => PlayerCount < 4;
    private bool CanRemovePlayer => PlayerCount > 2;

    private void NavigateToHome()
    {
        Navigation.NavigateTo("/");
    }

    private void AddPlayer()
    {
        if (CanAddPlayer)
        {
            PlayerNames.Add("");
            PlayerTypes.Add(true); // New player defaults to Real
            StateHasChanged();
        }
    }

    private void RemovePlayer(int index)
    {
        if (CanRemovePlayer && index >= 0 && index < PlayerNames.Count)
        {
            PlayerNames.RemoveAt(index);
            PlayerTypes.RemoveAt(index);
            StateHasChanged();
        }
    }

    private void TogglePlayerType(int index)
    {
        if (index >= 0 && index < PlayerTypes.Count)
        {
            PlayerTypes[index] = !PlayerTypes[index];
            StateHasChanged();
        }
    }

    private static string GetDefaultPlayerName(int index)
    {
        var defaultNames = new[] { "Alice", "Bob", "Charlie", "Diana" };
        return index < defaultNames.Length ? defaultNames[index] : $"Player {index + 1}";
    }

    private static void HandleGameIdKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            // On Game ID field, Enter should just move to next field naturally
            // No action needed - browser handles tab order
        }
    }

    private void HandlePlayerNameKeyDown(KeyboardEventArgs e, int playerIndex)
    {
        if (e.Key != "Enter") return;
        if (playerIndex != PlayerCount - 1) return;
        // Only start game when Enter is pressed on the LAST player name input
        if (!HasGameIdError) StartGame();
        // For other player inputs, Enter naturally moves to next field
    }

    private void ValidateGameId()
    {
        if (string.IsNullOrWhiteSpace(GameId))
        {
            HasGameIdError = false;
            GameIdErrorMessage = string.Empty;
            return;
        }

        var trimmedId = GameId.Trim();
        if (!Guid.TryParse(trimmedId, out _))
        {
            HasGameIdError = true;
            GameIdErrorMessage = "Invalid GUID format. Expected format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
        }
        else
        {
            HasGameIdError = false;
            GameIdErrorMessage = string.Empty;
        }
    }

    private void StartGame()
    {
        if (HasGameIdError) return;

        var queryString = $"?playerCount={PlayerCount}";

        // Always include game ID in URL - generate one if not provided
        var gameIdToUse = !string.IsNullOrWhiteSpace(GameId) ? GameId.Trim() : Guid.NewGuid().ToString();
        queryString += $"&gameId={Uri.EscapeDataString(gameIdToUse)}";

        // Add player names if any are provided
        var providedNames = new List<string>();
        for (var i = 0; i < PlayerCount; i++)
            providedNames.Add(!string.IsNullOrWhiteSpace(PlayerNames[i])
                ? Uri.EscapeDataString(PlayerNames[i].Trim())
                : ""); // Empty for default name

        if (providedNames.Any(name => !string.IsNullOrEmpty(name)))
            queryString += $"&playerNames={string.Join(",", providedNames)}";

        // Add player types (R for Real, A for AI)
        var playerTypesString = string.Join(",", PlayerTypes.Select(isReal => isReal ? "R" : "A"));
        queryString += $"&playerTypes={playerTypesString}";

        Navigation.NavigateTo($"/game{queryString}");
    }
}