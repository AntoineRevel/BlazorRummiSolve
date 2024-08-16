using BenchmarkDotNet.Attributes;

namespace RummiSolve;

public class RummiBench
{
    private readonly Set _exampleValidSet = new()
    {
        Tiles =
        [
            new Tile(1, Color.Blue),
            new Tile(2, Color.Blue),
            new Tile(3, Color.Blue),

            new Tile(7, Color.Red),
            new Tile(8, Color.Red),
            new Tile(9, Color.Red),

            new Tile(5, Color.Blue),
            new Tile(5, Color.Red),
            new Tile(5, Color.Black),

            new Tile(10, Color.Yellow),
            new Tile(11, Color.Yellow),
            new Tile(12, Color.Yellow),

            new Tile(7, Color.Red),
            new Tile(7, Color.Blue),
            new Tile(7, Color.Black),

            new Tile(4, Color.Black),
            new Tile(5, Color.Black),
            new Tile(6, Color.Black),

            new Tile(3, Color.Yellow),
            new Tile(3, Color.Red),
            new Tile(3, Color.Blue),

            new Tile(2, Color.Yellow),
            new Tile(3, Color.Yellow),
            new Tile(4, Color.Yellow),

            new Tile(9, Color.Black),
            new Tile(9, Color.Yellow),
            new Tile(9, Color.Red),

            new Tile(11, Color.Red),
            new Tile(12, Color.Red),
            new Tile(13, Color.Red),

            new Tile(6, Color.Blue),
            new Tile(6, Color.Yellow),
            new Tile(6, Color.Black),

            new Tile(12, Color.Black),
            new Tile(13, Color.Black),
            new Tile(11, Color.Black)
        ]
    };
    
    private readonly Set _exampleNotValidSet = new()
    {
        Tiles =
        [
            new Tile(1, Color.Blue),
            new Tile(2, Color.Blue),
            new Tile(3, Color.Blue),

            new Tile(7, Color.Red),
            new Tile(8, Color.Red),
            new Tile(9, Color.Red),

            new Tile(5, Color.Blue),
            new Tile(5, Color.Red),
            new Tile(5, Color.Black),

            new Tile(10, Color.Yellow),
            new Tile(11, Color.Yellow),
            new Tile(12, Color.Yellow),

            new Tile(7, Color.Red),
            new Tile(7, Color.Blue),
            new Tile(7, Color.Black),

            new Tile(4, Color.Black),
            new Tile(5, Color.Black),
            new Tile(6, Color.Black),

            new Tile(3, Color.Yellow),
            new Tile(3, Color.Red),
            new Tile(3, Color.Blue),

            new Tile(2, Color.Yellow),
            new Tile(3, Color.Yellow),
            new Tile(4, Color.Yellow),

            new Tile(9, Color.Black),
            new Tile(9, Color.Yellow),
            new Tile(9, Color.Red),

            new Tile(11, Color.Red),
            new Tile(12, Color.Red),
            new Tile(13, Color.Red),

            new Tile(6, Color.Blue),
            new Tile(6, Color.Yellow),
            new Tile(6, Color.Black),

            new Tile(12, Color.Black),
            new Tile(13, Color.Black),
        ]
    };

    [Benchmark]
    public void FindSolution()
    {
        _exampleValidSet.GetSolution();
        _exampleNotValidSet.GetSolution();
    }
}