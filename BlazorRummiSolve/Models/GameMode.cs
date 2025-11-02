namespace BlazorRummiSolve.Models;

/// <summary>
///     Defines the game mode based on player composition.
/// </summary>
public enum GameMode
{
    /// <summary>
    ///     All players are AI. User controls game flow with manual buttons (Show Hint, Next Player).
    /// </summary>
    FullAI,

    /// <summary>
    ///     At least one real player. AI players play automatically with visual delay, no manual "Next Player" button is needed.
    /// </summary>
    Interactive
}