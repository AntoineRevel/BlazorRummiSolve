using RummiSolve;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private Game _currentGame = new();
    private Player _currentPlayer = null!;
    private List<Player>? _otherPlayers;
    private bool IsGameOver => _currentGame.IsGameOver;
    private Player Winner => _currentGame.Winner!;
    private int TurnNumber => _currentGame.Turn;
    private Guid Id => _currentGame.Id;
    private bool IsLoading { get; set; }
    private bool ShowHint { get; set; }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }

    private ActionState _currentState;

    private async Task HandleActionAsync()
    {
        switch (_currentState)
        {
            case ActionState.ShowHint:
                ShowHint = true;
                _currentState = ActionState.ShowSolution;
                break;

            case ActionState.ShowSolution:
                ShowHint = false;
                _currentGame.ShowSolution(_currentPlayer);
                _currentState = ActionState.NextPlayer;
                break;

            case ActionState.NextPlayer:
                _currentGame.NextTurn();
                UpdatePlayers();
                await FindSolution();
                _currentState = ActionState.ShowHint;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void HandleActionBack()
    {
        switch (_currentState)
        {
            
            case ActionState.ShowSolution:
                ShowHint = false;
                _currentState = ActionState.ShowHint;
                break;

            case ActionState.NextPlayer:
                _currentGame.BackSolution();
                _currentPlayer.ShowLastTile();
                ShowHint = true;
                
                _currentState = ActionState.ShowSolution;
                break;
            case ActionState.ShowHint:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetButtonLabel()
    {
        return _currentState switch
        {
            ActionState.ShowHint => $"Show hint for {_currentPlayer?.Name}'s turn",
            ActionState.ShowSolution => $"Play {_currentPlayer?.Name}'s turn",
            ActionState.NextPlayer => "Next Player",
            _ => "Action"
        };
    }

    protected override async Task OnInitializedAsync()
    {
        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };

        _currentGame.InitializeGame(listNames);
        _currentState = ActionState.ShowHint;
        UpdatePlayers();
        await FindSolution();
    }

    private async Task FindSolution()
    {
        IsLoading = true;
        try
        {
            await Task.Run(() => _currentGame.PlayCurrentPlayerTurn(_currentPlayer));
            
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

    private async Task ResetGameAsync()
    {
        _currentGame = new Game();
        await OnInitializedAsync();
    }
}