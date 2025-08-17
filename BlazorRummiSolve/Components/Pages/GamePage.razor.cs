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
    [Inject] private HumanPlayerService HumanPlayerService { get; set; } = null!;

    [Parameter]
    [SupplyParameterFromQuery(Name = "playerCount")]
    public int? PlayerCount { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "gameId")]
    public string? GameId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "playerNames")]
    public string? PlayerNamesQuery { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "playerTypes")]
    public string? PlayerTypesQuery { get; set; }

    private Player CurrentPlayer { get; set; } = new("Default", [], new ParallelSolverStrategy());
    private List<Player> OtherPlayers => _currentGame.Players.Where(p => p != CurrentPlayer).ToList();
    private int TurnNumber => _currentGame.Turn;
    private Guid Id => _currentGame.Id;
    private bool IsLoading { get; set; }
    private bool ShowHint { get; set; }
    private bool IsWaitingForHumanPlayer { get; set; }
    private bool ShowTileSelection { get; set; }
    private List<bool> PlayerTypes { get; set; } = [];
    private List<Tile> SelectedTilesForPlay { get; set; } = [];

    private bool IsCurrentPlayerHuman =>
        _currentGame.Players.Count > 0 &&
        _currentGame.PlayerIndex < PlayerTypes.Count &&
        PlayerTypes[_currentGame.PlayerIndex];


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

        // Generate player names and types
        var listNames = ParsePlayerNames(playerCount);
        PlayerTypes = ParsePlayerTypes(playerCount);

        // Create a game with custom ID if provided
        if (!string.IsNullOrWhiteSpace(GameId) && Guid.TryParse(GameId, out var parsedId))
            _currentGame = new Game(parsedId);
        else
            _currentGame = new Game();

        // Setup human player service events
        HumanPlayerService.PlayerTurnStarted += OnPlayerTurnStarted;
        HumanPlayerService.PlayerTurnCompleted += OnPlayerTurnCompleted;

        _currentGame.InitializeGame(listNames, PlayerTypes, HumanPlayerService.WaitForPlayerChoice);
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

    private List<bool> ParsePlayerTypes(int count)
    {
        // Default: all players are Real (true)
        var defaultTypes = Enumerable.Repeat(true, count).ToList();

        if (string.IsNullOrWhiteSpace(PlayerTypesQuery)) return defaultTypes;

        var providedTypes = PlayerTypesQuery.Split(',').ToList();
        var finalTypes = new List<bool>();

        for (var i = 0; i < count; i++)
            if (i < providedTypes.Count && !string.IsNullOrWhiteSpace(providedTypes[i]))
            {
                var typeChar = providedTypes[i].Trim().ToUpper();
                finalTypes.Add(typeChar == "R"); // R = Real (true), A = AI (false)
            }
            else
            {
                finalTypes.Add(true); // Default to Real
            }

        return finalTypes;
    }

    private void OnPlayerTurnStarted(object? sender, EventArgs e)
    {
        IsWaitingForHumanPlayer = true;
        InvokeAsync(StateHasChanged);
    }

    private void OnPlayerTurnCompleted(object? sender, EventArgs e)
    {
        IsWaitingForHumanPlayer = false;
        ShowTileSelection = false;
        InvokeAsync(StateHasChanged);
    }

    // Methods for human player actions
    private void OnDrawTile()
    {
        HumanPlayerService.PlayerChooseDraw();
    }

    private void OnSelectTilesToPlay()
    {
        SelectedTilesForPlay.Clear();
        ShowTileSelection = true;
        StateHasChanged();
    }

    private void OnTileSelectionChanged(List<Tile> selectedTiles)
    {
        // Just update the selected tiles, don't confirm yet
        SelectedTilesForPlay = selectedTiles;
        StateHasChanged();
    }

    private void OnTileSelectionConfirmed()
    {
        // User clicked "Confirm" button
        ShowTileSelection = false;
        if (SelectedTilesForPlay.Count > 0) HumanPlayerService.PlayerChoosePlay(SelectedTilesForPlay);

        StateHasChanged();
    }

    private void OnTileSelectionCancelled()
    {
        ShowTileSelection = false;
        SelectedTilesForPlay.Clear();
        StateHasChanged();
    }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }
}