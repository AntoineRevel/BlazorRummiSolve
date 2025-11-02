using RummiSolve;
using RummiSolve.Results;
using RummiSolve.Solver.Combinations;
using RummiSolve.Solver.Combinations.First;

namespace BlazorRummiSolve.Services;

public class HumanPlayerService
{
    private Set _currentBoard = new();
    private TaskCompletionSource<SolverResult>? _currentPlayerChoice;
    private bool _hasPlayed;

    public string? LastErrorMessage { get; private set; }
    public bool IsWaitingForNextAfterDraw { get; private set; }

    public event EventHandler? PlayerTurnStarted;
    public event EventHandler? PlayerTurnCompleted;
    public event EventHandler<string>? InvalidPlayAttempted;

    // Called by HumanPlayerStrategy to wait for player input
    public async Task<SolverResult> WaitForPlayerChoice(Set board, bool hasPlayed, CancellationToken cancellationToken)
    {
        _currentBoard = board;
        _hasPlayed = hasPlayed;

        _currentPlayerChoice = new TaskCompletionSource<SolverResult>();

        // Register cancellation
        cancellationToken.Register(() => _currentPlayerChoice?.TrySetCanceled());

        // Notify UI that it's the player's turn
        PlayerTurnStarted?.Invoke(this, EventArgs.Empty);


        try
        {
            return await _currentPlayerChoice.Task;
        }
        finally
        {
            // Only trigger PlayerTurnCompleted if not waiting for "Next" after draw
            if (!IsWaitingForNextAfterDraw) PlayerTurnCompleted?.Invoke(this, EventArgs.Empty);

            _currentPlayerChoice = null;
        }
    }

    // Called by UI when player chooses to draw a tile
    public void PlayerChooseDraw()
    {
        if (_currentPlayerChoice == null) return;

        IsWaitingForNextAfterDraw = true;
        var result = SolverResult.Invalid("Human Player - Draw");
        _currentPlayerChoice.TrySetResult(result);
    }

    // Called by UI when player clicks "Next" after drawing a tile
    public void PlayerConfirmNext()
    {
        IsWaitingForNextAfterDraw = false;
        // Now trigger PlayerTurnCompleted to close the human player interface
        PlayerTurnCompleted?.Invoke(this, EventArgs.Empty);
    }

    // Called by UI when player selects tiles to play
    public void PlayerChoosePlay(List<Tile> selectedTiles)
    {
        if (_currentPlayerChoice == null) return;

        // Count jokers in selected tiles
        var jokerCount = selectedTiles.Count(t => t.IsJoker);
        var nonJokerTiles = selectedTiles.Where(t => !t.IsJoker).ToArray();

        SolverResult result;
        string errorMessage;

        if (_hasPlayed)
        {
            // Player has already played, validate with board tiles
            var boardTiles = _currentBoard.Tiles.ToArray();
            var allTiles = boardTiles.Concat(nonJokerTiles).Order().ToList();

            var jokers = _currentBoard.Jokers;
            if (jokers > 0) allTiles.RemoveRange(allTiles.Count - jokers, jokers);

            var combiSolver = new BinaryBaseSolver(allTiles.ToArray(), jokers + jokerCount)
            {
                TilesToPlay = nonJokerTiles,
                JokerToPlay = jokerCount
            };

            result = combiSolver.SearchSolution();
            errorMessage = "Invalid combination with current board.";
        }
        else
        {
            // First play validation
            var combiSolver = new BinaryFirstBaseSolver(nonJokerTiles, jokerCount);
            result = combiSolver.SearchSolution();

            errorMessage = result.Score < 30
                ? $"First play must be at least 30 points. Current: {result.Score}."
                : "Invalid combination.";
        }

        if (result.Found)
        {
            LastErrorMessage = null;
            _currentPlayerChoice.TrySetResult(result);
        }
        else
        {
            LastErrorMessage = errorMessage;
            InvalidPlayAttempted?.Invoke(this, errorMessage);
        }
    }
}