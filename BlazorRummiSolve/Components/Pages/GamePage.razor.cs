using RummiSolve;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private Solution _board = new();
    private Game _currentGame = new();

    private ActionState _currentState;
    private bool _isGameOver;
    private Solution _lastBoard = new();
    private Set _lastPlayerRack = new();


    private Set _playerRack = new();

    private Player CurrentPlayer { get; set; } = new("Default", []);
    private List<Player> OtherPlayers => _currentGame.Players.Where(p => p != CurrentPlayer).ToList();
    private int TurnNumber => _currentGame.Turn;
    private Guid Id => _currentGame.Id;
    private bool IsLoading { get; set; }
    private bool ShowHint { get; set; }


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
                _board = _currentGame.Board;
                _playerRack = CurrentPlayer.Rack;

                _isGameOver = _currentGame.IsGameOver;

                if (!_isGameOver) _currentState = ActionState.NextPlayer;
                break;

            case ActionState.NextPlayer:
                CurrentPlayer = _currentGame.Players[_currentGame.PlayerIndex];
                _playerRack = new Set(_currentGame.Players[_currentGame.PlayerIndex].Rack);
                _lastPlayerRack = new Set(_playerRack);
                _lastBoard = _board;
                await Play();
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
                _board = _lastBoard;
                _playerRack = _lastPlayerRack;
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
            ActionState.ShowHint => $"Show hint for {CurrentPlayer.Name}'s turn",
            ActionState.ShowSolution => $"Play {CurrentPlayer.Name}'s turn",
            ActionState.NextPlayer => "Next Player",
            _ => "Action"
        };
    }

    protected override async Task OnInitializedAsync()
    {
        var listNames = new List<string> { "Antoine", "Maguy", "Matthieu" };

        _currentGame.InitializeGame(listNames);
        _currentState = ActionState.ShowHint;
        CurrentPlayer = _currentGame.Players[0];
        _playerRack = new Set(CurrentPlayer.Rack);
        _lastPlayerRack = new Set(CurrentPlayer.Rack);

        await Play();
    }

    private async Task Play()
    {
        IsLoading = true;
        try
        {
            await Task.Run(() => _currentGame.Play());
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ResetGameAsync()
    {
        _currentGame = new Game();
        await OnInitializedAsync();
    }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }
}