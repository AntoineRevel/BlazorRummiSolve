using RummiSolve;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Common test cases for First-turn solvers (no board).
///     Each test case contains: Player tiles, joker count, and Expected result.
/// </summary>
public static class CommonTestFirstCases
{
    /// <summary>
    ///     All test cases for First-turn solvers
    /// </summary>
    public static readonly TestCase[] All =
    [
        new(
            "ThreeTilesGroup",
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black)
            ]),
            new CommonTestCases.ExpectedResult(
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
            "GroupAndRun",
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black),
                new Tile(1),
                new Tile(2),
                new Tile(3)
            ]),
            new CommonTestCases.ExpectedResult(
                true,
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red),
                    new Tile(10, TileColor.Black),
                    new Tile(1),
                    new Tile(2),
                    new Tile(3)
                ],
                0
            )
        ),

        new(
            "ThreeTilesOneJokerGroup",
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black)
            ]),
            new CommonTestCases.ExpectedResult(
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
            "SmallRunInvalid",
            new Set([
                new Tile(1),
                new Tile(2),
                new Tile(3)
            ]),
            new CommonTestCases.ExpectedResult(
                false,
                [],
                0
            )
        ),

        new(
            "FourTilesOneExtra",
            new Set([
                new Tile(1),
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(10, TileColor.Black)
            ]),
            new CommonTestCases.ExpectedResult(
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
            "MultipleSetsWithExtra",
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
            new CommonTestCases.ExpectedResult(
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
            "OnlyJokerInvalid",
            new Set([
                new Tile(true)
            ]),
            new CommonTestCases.ExpectedResult(
                false,
                [],
                0
            )
        ),

        new(
            "TwoTilesOnlyInvalid",
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red)
            ]),
            new CommonTestCases.ExpectedResult(
                false,
                [],
                0
            )
        ),

        new(
            "TwoTilesJokerValid",
            new Set([
                new Tile(10),
                new Tile(10, TileColor.Red),
                new Tile(true)
            ]),
            new CommonTestCases.ExpectedResult(
                true,
                [
                    new Tile(10),
                    new Tile(10, TileColor.Red)
                ]
                ,
                1
            )
        ),

        new(
            "TwoTilesJokerInvalid",
            new Set([
                new Tile(9),
                new Tile(9, TileColor.Red),
                new Tile(true)
            ]),
            new CommonTestCases.ExpectedResult(
                false,
                [],
                0
            )
        )
    ];

    public record TestCase(
        string Name,
        Set Player,
        CommonTestCases.ExpectedResult Expected
    );
}