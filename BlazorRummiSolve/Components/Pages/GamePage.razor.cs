using RummiSolve;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private SimpleGame _currentGame = new();

    private ActionState _currentState;
    private SimplePlayer CurrentPlayer => _currentGame.Players[_currentGame.PlayerIndex];
    private List<SimplePlayer> OtherPlayers => _currentGame.Players.Where(p => p != CurrentPlayer).ToList();
    private bool IsGameOver => _currentGame.IsGameOver;
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
                _currentState = ActionState.NextPlayer;
                break;

            case ActionState.NextPlayer:
                _currentGame.Play();
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
        _currentGame = new SimpleGame();
        await OnInitializedAsync();
    }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }
}