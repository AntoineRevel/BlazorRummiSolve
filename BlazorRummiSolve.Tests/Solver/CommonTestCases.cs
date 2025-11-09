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
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red),
                    new Tile(10, TileColor.Black)
                ],
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
                [
                    new Tile(1, TileColor.Red)
                ],
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
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red)
                ],
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
                [
                    new Tile(4, TileColor.Red)
                ],
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
                [
                    new Tile(4, TileColor.Red)
                ],
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
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red),
                    new Tile(10, TileColor.Black),
                    new Tile(1, TileColor.Red),
                    new Tile(2, TileColor.Red),
                    new Tile(3, TileColor.Red)
                ],
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
                [
                    new Tile(8, TileColor.Black),
                    new Tile(8, TileColor.Red)
                ],
                0
            )
        ),

        new(
            "ValidNWonIncrScorePlayer",
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
                [
                    new Tile(9, TileColor.Mango),
                    new Tile(13, TileColor.Red),
                    new Tile(13)
                ],
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
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red),
                    new Tile(10, TileColor.Black),
                    new Tile(5, TileColor.Red)
                ],
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
                [],
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
                [],
                0
            )
        ),
        new(
            "ValidMultipleDuplicates",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(4, TileColor.Red),
                new Tile(5, TileColor.Red)
            ]),
            new Set([
                new Tile(3, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(3, TileColor.Red),
                    new Tile(3, TileColor.Red)
                ],
                0
            )
        ),

        new(
            "ValidMultipleDuplicatesGroups",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(1, TileColor.Black),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black),
                new Tile(10)
            ]),
            new Set([
                new Tile(1, TileColor.Mango),
                new Tile(1, TileColor.Mango),
                new Tile(10, TileColor.Mango),
                new Tile(10, TileColor.Mango)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(1, TileColor.Mango),
                    new Tile(10, TileColor.Mango)
                ],
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
                [
                    new Tile(8, TileColor.Black),
                    new Tile(8, TileColor.Red)
                ],
                0
            )
        ),

        new(
            "ValidRunEnd",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red),
                new Tile(true)
            ]),
            new Set([
                new Tile(12),
                new Tile(13)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(12),
                    new Tile(13)
                ],
                0
            )
        ),

        new(
            "ValidJokerOnBoard",
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
                [],
                1
            )
        ),

        new(
            "ReplaceJokerWithRealTile",
            new Set([
                new Tile(11, TileColor.Mango),
                new Tile(true),
                new Tile(13, TileColor.Mango)
            ]),
            new Set([
                new Tile(12, TileColor.Mango),
                new Tile(13, TileColor.Red),
                new Tile(13)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(12, TileColor.Mango),
                    new Tile(13, TileColor.Red),
                    new Tile(13)
                ],
                0
            )
        ),

        // Complex scenarios with board
        new(
            "LargePlayerMultipleSets",
            new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red)
            ]),
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black),
                new Tile(4),
                new Tile(8),
                new Tile(11),
                new Tile(11),
                new Tile(13),
                new Tile(13, TileColor.Red),
                new Tile(13, TileColor.Black)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red),
                    new Tile(10, TileColor.Black),
                    new Tile(13),
                    new Tile(13, TileColor.Red),
                    new Tile(13, TileColor.Black)
                ],
                0
            )
        ),
        new(
            "ValidJustOneFirstToPlay",
            new Set([
                new Tile(2),
                new Tile(3),
                new Tile(4)
            ]),
            new Set([
                new Tile(1),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black)
            ]),
            new ExpectedResult(
                true,
                [
                    new Tile(1)
                ],
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
        IEnumerable<Tile> TilesToPlay,
        int JokerToPlay
    );
}