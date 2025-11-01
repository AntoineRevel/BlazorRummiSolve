using BlazorRummiSolve.Models;
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
    private CancellationTokenSource? _aiTurnTimerCts;
    private Solution _board = new();
    private Game _currentGame = new();

    private ActionState _currentState;
    private bool _isFirstTurn = true;
    private bool _isGameOver;

    // AI turn countdown states
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
    private GameMode CurrentGameMode { get; set; }

    // Drawn tile toast notification properties
    private Tile? DrawnTile { get; set; }
    private bool ShowDrawnTileToast { get; set; }

    // Error message properties
    private string? ErrorMessage { get; set; }
    private bool ShowErrorMessage { get; set; }

    // AI turn countdown properties
    private bool IsWaitingAfterAITurn { get; set; }

    private int RemainingSeconds { get; set; }

    private bool AIPlayerDrew { get; set; }

    private List<Tile>? AIPlayedTiles { get; set; }

    private double ProgressDashOffset => 125.66 * RemainingSeconds / 4.0;

    // For Full AI mode: store displayed tile counts
    private Dictionary<Player, int> DisplayedTileCounts { get; } = new();

    private bool IsCurrentPlayerHuman =>
        _currentGame.Players.Count > 0 &&
        _currentGame.PlayerIndex < PlayerTypes.Count &&
        PlayerTypes[_currentGame.PlayerIndex];

    private int GetDisplayedTileCount(Player player)
    {
        // In Full AI mode, use cached counts if available (before showing solution)
        if (CurrentGameMode == GameMode.FullAI && DisplayedTileCounts.ContainsKey(player))
            return DisplayedTileCounts[player];

        // Otherwise, return actual count
        return player.Rack.Tiles.Count;
    }

    private bool ShouldShowAIRack()
    {
        // Don't show rack if game is over or waiting for human player
        if (_isGameOver || IsWaitingForHumanPlayer)
            return false;

        // In Interactive mode, NEVER show AI rack (only show what they play on the board)
        if (CurrentGameMode == GameMode.Interactive)
        {
            return false;
        }

        // In Full AI mode, show rack when it's an AI's turn
        var actualPlayerIndex = _currentGame.Players.IndexOf(CurrentPlayer);
        if (actualPlayerIndex >= 0 && actualPlayerIndex < PlayerTypes.Count)
        {
            var isHuman = PlayerTypes[actualPlayerIndex];
            return !isHuman; // Show if current player is AI
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

                // In Full AI mode, clear displayed tile counts to show actual counts
                if (CurrentGameMode == GameMode.FullAI) DisplayedTileCounts.Clear();

                _isGameOver = _currentGame.IsGameOver;

                if (!_isGameOver) _currentState = ActionState.NextPlayer;
                break;

            case ActionState.NextPlayer:
                _isFirstTurn = false;
                CurrentPlayer = _currentGame.Players[_currentGame.PlayerIndex];
                _playerRack = new Set(_currentGame.Players[_currentGame.PlayerIndex].Rack);
                _lastPlayerRack = new Set(_playerRack);
                _lastBoard = _board;

                // In Full AI mode, save current tile counts before playing
                if (CurrentGameMode == GameMode.FullAI)
                {
                    DisplayedTileCounts.Clear();
                    foreach (var player in _currentGame.Players) DisplayedTileCounts[player] = player.Rack.Tiles.Count;
                }

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
            ActionState.NextPlayer => _isFirstTurn ? "Start" : $"Play {CurrentPlayer.Name}'s turn",
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

        // Detect game mode automatically based on player types
        CurrentGameMode = DetectGameMode();

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
        CurrentPlayer = _currentGame.Players[0];
        _playerRack = new Set(CurrentPlayer.Rack);
        _lastPlayerRack = new Set(CurrentPlayer.Rack);

        // Start the game flow based on the mode
        if (CurrentGameMode == GameMode.Interactive)
        {
            // In Interactive mode, start the automatic flow
            _currentState = ActionState.ShowHint;
            _ = PlayInteractiveModeAsync();
        }
        else // Full AI mode
        {
            // In Full AI mode, start at NextPlayer state so the first click triggers PlayAsync() directly
            _currentState = ActionState.NextPlayer;
        }
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

    private async Task PlayInteractiveModeAsync()
    {
        while (!_currentGame.IsGameOver)
        {
            CurrentPlayer = _currentGame.Players[_currentGame.PlayerIndex];
            _playerRack = new Set(CurrentPlayer.Rack);
            _lastPlayerRack = new Set(_playerRack);
            _lastBoard = _board;

            // Check if current player is human
            if (IsCurrentPlayerHuman)
            {
                // Execute the turn (might draw tile or play)
                var turnCompleted = await _currentGame.ExecuteTurnAsync(await CancellationService.CreateTokenAsync());

                // Update the board and rack after human plays
                _board = _currentGame.Board;
                _playerRack = CurrentPlayer.Rack;

                if (!turnCompleted && HumanPlayerService.IsWaitingForNextAfterDraw)
                {
                    // Player drew a tile, wait for them to click "Next"
                    StateHasChanged();

                    while (HumanPlayerService
                           .IsWaitingForNextAfterDraw) await Task.Delay(100); // Wait for user to click "Next"
                }

                // Advance to next player
                _currentGame.AdvanceToNextPlayer();
            }
            else
            {
                // Execute AI turn and check if they drew a tile
                var turnCompleted = await _currentGame.ExecuteTurnAsync(await CancellationService.CreateTokenAsync());
                AIPlayerDrew = !turnCompleted;

                // Capture tiles played by AI before updating board
                AIPlayedTiles = turnCompleted ? CurrentPlayer.TilesToPlay.ToList() : null;

                // Update the board and rack to show AI's move
                _board = _currentGame.Board;
                _playerRack = CurrentPlayer.Rack;

                StateHasChanged();

                // Show countdown button for 8 seconds
                IsWaitingAfterAITurn = true;
                RemainingSeconds = 8;
                StateHasChanged();

                // Start 8-second countdown
                _aiTurnTimerCts = new CancellationTokenSource();
                try
                {
                    for (var i = 8; i > 0; i--)
                    {
                        RemainingSeconds = i;
                        await InvokeAsync(StateHasChanged);
                        await Task.Delay(1000, _aiTurnTimerCts.Token);
                    }
                }
                catch (TaskCanceledException)
                {
                    // User clicked "Next" before timer finished - that's fine
                }
                finally
                {
                    IsWaitingAfterAITurn = false;
                    AIPlayedTiles = null; // Clear highlighted tiles
                    _aiTurnTimerCts?.Dispose();
                    _aiTurnTimerCts = null;
                }

                // Advance to next player
                _currentGame.AdvanceToNextPlayer();
            }

            // Check if game ended after this turn
            _isGameOver = _currentGame.IsGameOver;
            if (_isGameOver)
            {
                StateHasChanged();
                break;
            }
        }
    }

    private async Task ResetGameAsync()
    {
        _currentGame = new Game();
        _isFirstTurn = true;
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

    private GameMode DetectGameMode()
    {
        // Full AI mode if all players are AI (all false)
        // Interactive mode if at least one player is real (at least one true)
        return PlayerTypes.Any(isReal => isReal) ? GameMode.Interactive : GameMode.FullAI;
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

        // Force UI update to show the new tile in the rack
        StateHasChanged();

        // In Full AI mode, automatically proceed to next player
        // In Interactive mode, the PlayInteractiveModeAsync loop handles progression and delay
        if (CurrentGameMode == GameMode.FullAI) await AutoProceedToNextPlayer();
    }

    // Called by UI when player clicks "Next" after drawing a tile
    private void OnNextAfterDraw()
    {
        HumanPlayerService.PlayerConfirmNext();
        StateHasChanged();
    }

    // Called by UI when player clicks "Next" to skip AI turn countdown
    private void OnSkipAIWait()
    {
        // Cancel the countdown timer
        _aiTurnTimerCts?.Cancel();
        StateHasChanged();
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