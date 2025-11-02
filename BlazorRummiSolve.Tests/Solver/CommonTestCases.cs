using RummiSolve;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Common test cases extracted from CombinationsSolverTests.
///     Each test case contains: Board, Player, and Expected SolverResult.
/// </summary>
public static class CommonTestCases
{
    /// <summary>
    ///     All test cases from CombinationsSolverTests
    /// </summary>
    public static readonly TestCase[] All =
    [
        new(
            "Valid",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black)
            ]),
            new ExpectedResult(
                true,
                3,
                0
            )
        ),

        new(
            "ValidPlay1",
            new Set([
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red),
                new Tile(4, TileColor.Red)
            ]),
            new Set([
                new Tile(1, TileColor.Red)
            ]),
            new ExpectedResult(
                true,
                1,
                0
            )
        ),

        new(
            "ValidJoker",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red),
                new Tile(true)
            ]),
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red)
            ]),
            new ExpectedResult(
                true,
                2,
                0
            )
        ),

        new(
            "ValidRun",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(4, TileColor.Red)
            ]),
            new ExpectedResult(
                true,
                1,
                0
            )
        ),

        new(
            "ValidRunJoker",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(4, TileColor.Red),
                new Tile(true)
            ]),
            new ExpectedResult(
                true,
                1,
                1
            )
        ),

        new(
            "ValidNotWon",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black),
                new Tile(5),
                new Tile(5),
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new ExpectedResult(
                true,
                6,
                0
            )
        ),

        new(
            "ValidNotWon2",
            new Set([
                new Tile(8),
                new Tile(9),
                new Tile(10),
                new Tile(11)
            ]),
            new Set([
                new Tile(8, TileColor.Black),
                new Tile(8, TileColor.Red),
                new Tile(1)
            ]),
            new ExpectedResult(
                true,
                2,
                0
            )
        ),

        new(
            "ValidNWonIncrscorePlayer",
            new Set([
                new Tile(10),
                new Tile(11),
                new Tile(12),
                new Tile(13),
                new Tile(11, TileColor.Mango),
                new Tile(true),
                new Tile(13, TileColor.Mango)
            ]),
            new Set([
                new Tile(9, TileColor.Mango),
                new Tile(13, TileColor.Red),
                new Tile(13)
            ]),
            new ExpectedResult(
                true,
                3,
                0
            )
        ),

        new(
            "ValidWinJoker",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black),
                new Tile(5, TileColor.Red),
                new Tile(true)
            ]),
            new ExpectedResult(
                true,
                4,
                1
            )
        ),

        new(
            "ValidOneJ",
            new Set([
                new Tile(5),
                new Tile(5, TileColor.Red),
                new Tile(5, TileColor.Black)
            ]),
            new Set([
                new Tile(true)
            ]),
            new ExpectedResult(
                true,
                0,
                1
            )
        ),

        new(
            "Invalid",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(5)
            ]),
            new ExpectedResult(
                false,
                0,
                0
            )
        )
    ];

    public record TestCase(
        string Name,
        Set Board,
        Set Player,
        ExpectedResult Expected
    );

    public record ExpectedResult(
        bool IsValid,
        int TilesToPlayCount,
        int JokerToPlay
    );
}