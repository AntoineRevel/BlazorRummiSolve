using RummiSolve;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private Game _currentGame = new();
    private Player? _currentPlayer;
    private List<Player>? _otherPlayers;
    private bool IsGameOver => _currentGame.IsGameOver;
    private Player Winner => _currentGame.Winner!;
    private int TurnNumber => _currentGame.Turn;
    private Guid Id => _currentGame.Id;

    private bool IsLoading { get; set; }
    
    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }

    private ActionState _currentState = ActionState.ShowHint;

    private async Task HandleActionAsync()
    {
        switch (_currentState)
        {
            case ActionState.ShowHint:
                await ShowHintAsync();
                _currentState = ActionState.ShowSolution;
                break;

            case ActionState.ShowSolution:
                 _currentGame.ShowSolution();
                _currentState = ActionState.NextPlayer;
                break;

            case ActionState.NextPlayer:
                UpdatePlayers();
                _currentState = ActionState.ShowHint;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetButtonLabel()
    {
        return _currentState switch
        {
            ActionState.ShowHint => $"Show Hint for {_currentPlayer?.Name}'s turn",
            ActionState.ShowSolution => $"Show Solution for {_currentPlayer?.Name}'s turn",
            ActionState.NextPlayer => "Next Player",
            _ => "Action"
        };
    }

    protected override void OnInitialized()
    {
        _currentGame.AddPlayer("Antoine");
        _currentGame.AddPlayer("Matthieu");

        _currentGame.InitializeGame();

        UpdatePlayers();
    }

    private async Task ShowHintAsync()
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
    
    private Task ResetGameAsync()
    {
        _currentGame = new Game();
        OnInitialized();
        return Task.CompletedTask;
    }
}