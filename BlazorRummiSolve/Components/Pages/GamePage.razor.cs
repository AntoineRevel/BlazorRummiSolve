using BlazorRummiSolve.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RummiSolve;
using RummiSolve.Strategies;

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

    [Inject] private CancellationService CancellationService { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "playerCount")]
    public int? PlayerCount { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "gameId")]
    public string? GameId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "playerNames")]
    public string? PlayerNamesQuery { get; set; }

    private Player CurrentPlayer { get; set; } = new("Default", [], new ParallelSolverStrategy());
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
                await PlayAsync();
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
        // Parse and validate player count
        var playerCount = PlayerCount ?? 3;
        if (playerCount is < 2 or > 4) playerCount = 3;

        // Generate player names
        var listNames = ParsePlayerNames(playerCount);

        // Create a game with custom ID if provided
        if (!string.IsNullOrWhiteSpace(GameId) && Guid.TryParse(GameId, out var parsedId))
            _currentGame = new Game(parsedId);
        else
            _currentGame = new Game();

        _currentGame.InitializeGame(listNames);
        _currentState = ActionState.ShowHint;
        CurrentPlayer = _currentGame.Players[0];
        _playerRack = new Set(CurrentPlayer.Rack);
        _lastPlayerRack = new Set(CurrentPlayer.Rack);

        await PlayAsync();
    }

    private async Task PlayAsync()
    {
        IsLoading = true;

        try
        {
            var cancellationToken = await CancellationService.CreateTokenAsync();
            await _currentGame.PlayAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Game operation was cancelled");
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

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !IsLoading && !_isGameOver)
        {
            await HandleActionAsync();
        }
    }

    private List<string> ParsePlayerNames(int count)
    {
        var defaultNames = new List<string> { "Alice", "Bob", "Charlie", "Diana" };

        if (string.IsNullOrWhiteSpace(PlayerNamesQuery)) return defaultNames.Take(count).ToList();

        var providedNames = PlayerNamesQuery.Split(',').Select(Uri.UnescapeDataString).ToList();
        var finalNames = new List<string>();

        for (var i = 0; i < count; i++)
            if (i < providedNames.Count && !string.IsNullOrWhiteSpace(providedNames[i]))
                finalNames.Add(providedNames[i].Trim());
            else
                finalNames.Add(i < defaultNames.Count ? defaultNames[i] : $"Player {i + 1}");

        return finalNames;
    }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }
}