using BlazorRummiSolve.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RummiSolve;
using RummiSolve.Strategies;
using Timer = System.Timers.Timer;

namespace BlazorRummiSolve.Components.Pages;

public partial class GamePage
{
    private readonly HashSet<Guid> _newTiles = [];
    private readonly HashSet<Guid> _removingTiles = [];

    private readonly List<TileInstance> _selectedTileInstances = [];
    private Solution _board = new();
    private Game _currentGame = new();

    private ActionState _currentState;
    private bool _isGameOver;
    private Solution _lastBoard = new();
    private Set _lastPlayerRack = new();


    private Set _playerRack = new();
    private Timer? _toastTimer;

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
    private List<bool> PlayerTypes { get; set; } = [];
    private List<Tile> SelectedTilesForPlay { get; set; } = [];

    // Drawn tile toast notification properties
    private Tile? DrawnTile { get; set; }
    private bool ShowDrawnTileToast { get; set; }

    // Error message properties
    private string? ErrorMessage { get; set; }
    private bool ShowErrorMessage { get; set; }

    private bool IsCurrentPlayerHuman =>
        _currentGame.Players.Count > 0 &&
        _currentGame.PlayerIndex < PlayerTypes.Count &&
        PlayerTypes[_currentGame.PlayerIndex];

    private bool ShouldShowAIRack()
    {
        // Show AI rack only when:
        // 1. It's not game over
        // 2. Current player is AI (not human)
        // 3. Not waiting for human player input
        if (_isGameOver || IsWaitingForHumanPlayer)
            return false;

        // Find the actual index of CurrentPlayer in the game
        var actualPlayerIndex = _currentGame.Players.IndexOf(CurrentPlayer);

        if (actualPlayerIndex >= 0 && actualPlayerIndex < PlayerTypes.Count)
        {
            var isHuman = PlayerTypes[actualPlayerIndex];
            // Show rack only if current player is AI (not human)
            return !isHuman;
        }

        return false;
    }

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
        HumanPlayerService.InvalidPlayAttempted += OnInvalidPlayAttempted;

        _currentGame.InitializeGame(listNames, PlayerTypes, HumanPlayerService.WaitForPlayerChoice);
        _currentState = ActionState.ShowHint;
        CurrentPlayer = _currentGame.Players[0];
        _playerRack = new Set(CurrentPlayer.Rack);
        _lastPlayerRack = new Set(CurrentPlayer.Rack);

        // If the first player is human, trigger PlayAsync to show the human player interface
        // Don't await to avoid blocking the UI initialization
        if (IsCurrentPlayerHuman) _ = PlayAsync();
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
        OnClearSelection();

        // Hide any active toast notification when turn completes
        ShowDrawnTileToast = false;
        DrawnTile = null;
        _toastTimer?.Stop();
        _toastTimer?.Dispose();
        _toastTimer = null;

        // Clear error message when turn completes
        ShowErrorMessage = false;
        ErrorMessage = null;

        InvokeAsync(StateHasChanged);
    }

    private void OnInvalidPlayAttempted(object? sender, string errorMessage)
    {
        ErrorMessage = errorMessage;
        ShowErrorMessage = true;
        InvokeAsync(StateHasChanged);
    }

    // Methods for human player actions
    private async Task OnDrawTile()
    {
        // Capture current rack to identify drawn tile later
        var currentRackTiles = CurrentPlayer.Rack.Tiles.ToList();

        HumanPlayerService.PlayerChooseDraw();

        // Wait for the game state to update and capture the drawn tile
        await Task.Delay(200); // Give time for game state to update

        // Capture and show the drawn tile
        CaptureAndShowDrawnTile(currentRackTiles);

        // Update the board and player rack to reflect the draw
        _board = _currentGame.Board;
        _playerRack = CurrentPlayer.Rack;

        // Automatically proceed to next player
        await AutoProceedToNextPlayer();
    }

    private void OnPlaySelection()
    {
        if (SelectedTilesForPlay.Count > 0)
        {
            // Skip directly to the post-hint step by modifying the game state
            HumanPlayerService.PlayerChoosePlay(SelectedTilesForPlay);

            // Instead of waiting for normal flow, directly advance to showing the solution
            _currentState = ActionState.ShowSolution;
            ShowHint = true;
        }

        StateHasChanged();
    }

    private void OnClearSelection()
    {
        _selectedTileInstances.Clear();
        SelectedTilesForPlay.Clear();
        _removingTiles.Clear();
        _newTiles.Clear();
        StateHasChanged();
    }

    // Tile selection logic from popup
    private async Task OnTileInstanceClick(TileInstance tileInstance)
    {
        _removingTiles.Add(tileInstance.Id);
        StateHasChanged();

        await Task.Delay(300);

        _selectedTileInstances.Remove(tileInstance);
        _removingTiles.Remove(tileInstance.Id);

        SyncSelectedTiles();
        StateHasChanged();
    }

    private void SyncSelectedTiles()
    {
        SelectedTilesForPlay.Clear();
        SelectedTilesForPlay.AddRange(_selectedTileInstances.Select(ti => ti.Tile));
    }

    private async Task OnTileStackClick(Tile tileType)
    {
        var totalAvailable = CurrentPlayer.Rack.Tiles.Count(t => t.Equals(tileType));
        var currentSelectedCount = GetSelectedCountForTile(tileType);

        var newSelectedCount = (currentSelectedCount + 1) % (totalAvailable + 1);

        if (newSelectedCount > currentSelectedCount)
        {
            var tilesToAdd = newSelectedCount - currentSelectedCount;
            for (var i = 0; i < tilesToAdd; i++)
            {
                var newInstance = new TileInstance(tileType);
                _selectedTileInstances.Add(newInstance);
                _newTiles.Add(newInstance.Id);
            }
        }
        else if (newSelectedCount < currentSelectedCount)
        {
            var tilesToRemove = currentSelectedCount - newSelectedCount;
            for (var i = 0; i < tilesToRemove; i++)
            for (var j = _selectedTileInstances.Count - 1; j >= 0; j--)
            {
                if (!_selectedTileInstances[j].Tile.Equals(tileType)) continue;
                var instanceToRemove = _selectedTileInstances[j];
                _selectedTileInstances.RemoveAt(j);
                _newTiles.Remove(instanceToRemove.Id);
                break;
            }
        }
        else
        {
            var instancesToRemove = _selectedTileInstances.Where(ti => ti.Tile.Equals(tileType)).ToList();
            foreach (var instance in instancesToRemove)
            {
                _selectedTileInstances.Remove(instance);
                _newTiles.Remove(instance.Id);
            }
        }

        SyncSelectedTiles();
        StateHasChanged();

        if (newSelectedCount > currentSelectedCount)
        {
            await Task.Delay(300);
            var instancesToClean = _selectedTileInstances
                .Where(ti => ti.Tile.Equals(tileType) && _newTiles.Contains(ti.Id)).ToList();
            foreach (var instance in instancesToClean) _newTiles.Remove(instance.Id);

            StateHasChanged();
        }
    }

    private int GetSelectedCountForTile(Tile tileType)
    {
        return _selectedTileInstances.Count(ti => ti.Tile.Equals(tileType));
    }

    private IEnumerable<IGrouping<Tile, Tile>> GetGroupedTiles()
    {
        return GetSortedTiles()
            .GroupBy(tile => tile)
            .OrderBy(g => g.Key.IsJoker ? 1 : 0)
            .ThenBy(g => g.Key.Color)
            .ThenBy(g => g.Key.Value);
    }

    private List<Tile> GetSortedTiles()
    {
        return CurrentPlayer.Rack.Tiles
            .OrderBy(tile => tile.IsJoker ? 1 : 0)
            .ThenBy(tile => tile.Color)
            .ThenBy(tile => tile.Value)
            .ToList();
    }

    // Auto-proceed to next player method
    private async Task AutoProceedToNextPlayer()
    {
        // Set the current player and update racks
        CurrentPlayer = _currentGame.Players[_currentGame.PlayerIndex];
        _playerRack = new Set(_currentGame.Players[_currentGame.PlayerIndex].Rack);
        _lastPlayerRack = new Set(_playerRack);
        _lastBoard = _board;

        // Start the next player's turn
        await PlayAsync();
        _currentState = ActionState.ShowHint;

        StateHasChanged();
    }

    // Drawn tile methods
    private void CaptureAndShowDrawnTile(List<Tile> previousRackTiles)
    {
        var currentRackTiles = CurrentPlayer.Rack.Tiles.ToList();

        // Find the newly drawn tile by comparing current rack with previous rack
        var drawnTiles = currentRackTiles.Except(previousRackTiles).ToList();

        if (drawnTiles.Count == 1)
        {
            DrawnTile = drawnTiles.First();
            ShowDrawnTileToast = true;

            // Set up timer to hide toast after 3 seconds
            _toastTimer?.Stop();
            _toastTimer = new Timer(3000);
            _toastTimer.Elapsed += (_, _) =>
            {
                InvokeAsync(() =>
                {
                    ShowDrawnTileToast = false;
                    DrawnTile = null;
                    _toastTimer?.Stop();
                    _toastTimer?.Dispose();
                    _toastTimer = null;
                    StateHasChanged();
                });
            };
            _toastTimer.Start();

            StateHasChanged();
        }
    }

    // Tile selection management
    private readonly record struct TileInstance(Tile Tile)
    {
        public Tile Tile { get; } = Tile;
        public Guid Id { get; } = Guid.NewGuid();
    }

    private enum ActionState
    {
        ShowHint,
        ShowSolution,
        NextPlayer
    }
}