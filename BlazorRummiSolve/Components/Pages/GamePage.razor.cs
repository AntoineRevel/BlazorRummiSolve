using RummiSolve;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private readonly Game _currentGame = new();
    private Player? _currentPlayer;
    private List<Player>? _otherPlayers;
    private bool IsGameOver => _currentGame.IsGameOver;
    private Player Winner => _currentGame.Winner!;
    private int TurnNumber => _currentGame.Turn;
    private Guid Id => _currentGame.Id;

    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        _currentGame.AddPlayer("Antoine");
        _currentGame.AddPlayer("Matthieu");

        _currentGame.InitializeGame();

        UpdatePlayers();
    }

    private async Task PlayTurnAsync()
    {
        IsLoading = true;
        try
        {
            await Task.Run(() => _currentGame.PlayCurrentPlayerTurn());
            UpdatePlayers();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdatePlayers()
    {
        _currentPlayer = _currentGame.Players[_currentGame.CurrentPlayerIndex];
        _otherPlayers = _currentGame.Players.Where(p => p != _currentPlayer).ToList();
    }
}