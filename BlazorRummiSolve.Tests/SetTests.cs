using RummiSolve;

namespace BlazorRummiSolve.Tests;

public class SetTests
{
    [Fact]
    public void AddTile_IncrementsTilesCount()
    {
        // Arrange
        var set = new Set();
        var tile = new Tile(5, TileColor.Red);

        // Act
        set.AddTile(tile);

        // Assert
        Assert.Single(set.Tiles);
        Assert.Contains(tile, set.Tiles);
    }

    [Fact]
    public void GetSolution_ReturnsInvalidSolutionForSmallSet()
    {
        // Arrange
        var tiles = new List<Tile>
        {
            new Tile(1, TileColor.Red),
            new Tile(2),
        };
        var set = new Set(tiles);

        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.False(solution.IsValid);
    }

    [Fact]
    public void GetSolution_ReturnsInvalidSolution()
    {
        // Arrange
        var set = new Set([
            new Tile(1, TileColor.Black),
            new Tile(2, TileColor.Black),
            new Tile(3, TileColor.Black),

            new Tile(2),
            new Tile(true),
            new Tile(4),

            new Tile(7, TileColor.Red),
            new Tile(7, TileColor.Mango),
            new Tile(7, TileColor.Black),

            new Tile(3, TileColor.Red),
            new Tile(3, TileColor.Mango),
            new Tile(3, TileColor.Black),

            new Tile(6),
            new Tile(6, TileColor.Red),
            new Tile(6, TileColor.Black),

            new Tile(5),
            new Tile(5, TileColor.Mango),
            new Tile(5, TileColor.Black),

            new Tile(8, TileColor.Black),
            new Tile(5, TileColor.Red),
            new Tile(13, TileColor.Red),
            new Tile(12, TileColor.Red),
            new Tile(6, TileColor.Mango),
            new Tile(9, TileColor.Black),
            new Tile(2, TileColor.Mango),
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.False(solution.IsValid);
    }
    
    [Fact]
    public void GetSolution_ReturnsInvalidSolution2()
    {
        // Arrange
        var set = new Set([
            new Tile(8,TileColor.Black),
            new Tile(9,TileColor.Black),
            new Tile(10,TileColor.Black),
            
            new Tile(4,TileColor.Black),
            new Tile(5,TileColor.Black),
            new Tile(6,TileColor.Black),
            
            new Tile(9,TileColor.Mango),
            new Tile(10,TileColor.Mango),
            new Tile(11,TileColor.Mango),
            
            new Tile(5,TileColor.Red),
            new Tile(6,TileColor.Red),
            new Tile(7,TileColor.Red),
            new Tile(true),
            new Tile(9,TileColor.Red),
            
            new Tile(10),
            new Tile(10,TileColor.Mango),
            new Tile(10,TileColor.Red),
            
            new Tile(3),
            new Tile(3,TileColor.Black),
            new Tile(3,TileColor.Red),
            
            new Tile(6,TileColor.Black),
            new Tile(13,TileColor.Black),
            new Tile(7,TileColor.Black),
            new Tile(4,TileColor.Red),
            new Tile(9),
            new Tile(2,TileColor.Red),
            new Tile(4,TileColor.Red),
            new Tile(12,TileColor.Black),
            new Tile(3,TileColor.Mango),
            new Tile(4,TileColor.Black),
            new Tile(5,TileColor.Black),
            new Tile(5,TileColor.Red),
            new Tile(12,TileColor.Black),
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.False(solution.IsValid);
    }

    [Fact]
    public void GetFirstSolution_ReturnsValidSolutionIfPossible()
    {
        // Arrange
        var tiles = new List<Tile>
        {
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red)
        };
        var set = new Set(tiles);

        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);
    }

    [Fact]
    public void GetFirstSolution_ReturnsValidSolution2()
    {
        // Arrange

        var setB = new Set([
            new Tile(8),
            new Tile(9, TileColor.Blue, true),
            new Tile(10),

            new Tile(12),
            new Tile(12, TileColor.Mango),
            new Tile(12, TileColor.Black),

            new Tile(13),
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Black)
        ]);

        var solB = setB.GetSolution();

        var player = new Player("Maguy", [
            new Tile(11),
            new Tile(4, TileColor.Black),
            new Tile(6, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(5),
            new Tile(10, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(13, TileColor.Mango),
            new Tile(9, TileColor.Black),
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Mango)
        ]);

        player.PrintRackTiles();

        //player._played = true;

        // Act
        var newSol = player.Solve(solB);

        // Assert
        Assert.True(newSol.IsValid);
        Assert.Single(newSol.Runs);
        Assert.Equal(144, newSol.GetSet().GetScore());
    }
    
    [Fact]
    public void GetFirstSolution_ReturnsValidSolution3()
    {
        // Arrange

        var setB = new Set([
            new Tile(12),
            new Tile(12, TileColor.Red),
            new Tile(12, TileColor.Black),

            new Tile(11),
            new Tile(11, TileColor.Mango),
            new Tile(11, TileColor.Black),

            new Tile(6),
            new Tile(6, TileColor.Red),
            new Tile(6, TileColor.Mango),

            new Tile(5),
            new Tile(5, TileColor.Mango),
            new Tile(5, TileColor.Black)
        ]);

        var solB = setB.GetSolution();

        var player = new Player("Maguy", [
            new Tile(11, TileColor.Mango),
            new Tile(10, TileColor.Mango),
            new Tile(true)
        ]);

        player.PrintRackTiles();

        //player._played = true;

        // Act
        var newSol = player.Solve(solB);

        // Assert
        Assert.True(newSol.IsValid);
        Assert.Single(newSol.Runs);
    }

    [Fact]
    public void GetSolution_ReturnsValidGroupJoker()
    {
        // Arrange
        var groupSet = new Set(
            [
                new Tile(1),
                new Tile(1, TileColor.Black),
                new Tile(true),
                new Tile(true),

                new Tile(2),
                new Tile(3),
                new Tile(4)
            ]
        );


        // Act
        var solution = groupSet.GetSolution();

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(groupSet.Tiles.Count + 2, solution.GetSet().Tiles.Count);
    }

    [Fact]
    public void GetSolution_ReturnsValidRunJoker()
    {
        // Arrange
        var set = new Set([
            new Tile(1),
            new Tile(2),
            new Tile(3),
            new Tile(true),
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(set.Tiles.Count + 1, solution.GetSet().Tiles.Count);
    }

    [Fact]
    public void GetSolution_ReturnsValidGroupAndRun()
    {
        // Arrange
        var set = new Set([
            new Tile(7),
            new Tile(7, TileColor.Mango),
            new Tile(7, TileColor.Black),
            new Tile(7, TileColor.Red),

            new Tile(1, TileColor.Black),
            new Tile(2, TileColor.Black),
            new Tile(3, TileColor.Black),

            new Tile(7, TileColor.Red),
            new Tile(8, TileColor.Red),
            new Tile(9, TileColor.Red),

            new Tile(6, TileColor.Red),
            new Tile(6, TileColor.Mango),
            new Tile(6, TileColor.Black),
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);
        Assert.Equal(set.Tiles.Count, solution.GetSet().Tiles.Count);
    }

    [Fact]
    public void GetSolution_ReturnsValidGroupAndRunJoker()
    {
        // Arrange
        var set = new Set([
            new Tile(11),
            new Tile(11, TileColor.Red),
            new Tile(11, TileColor.Black),
            
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango), //lui
            new Tile(true),
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(set.Tiles.Count + 1, solution.GetSet().Tiles.Count);
    }

    [Fact]
    public void GetSolution_ReturnsValidGroupAndRunJoker2()
    {
        // Arrange
        var set = new Set([
            new Tile(10, TileColor.Black),
            new Tile(11, TileColor.Black),
            new Tile(12, TileColor.Black),

            new Tile(9, TileColor.Mango),
            new Tile(10, TileColor.Mango),
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango),

            new Tile(10, TileColor.Red),
            new Tile(11, TileColor.Red),
            new Tile(true),

            new Tile(8),
            new Tile(9),
            new Tile(10),
            new Tile(11),

            new Tile(13),
            new Tile(13, TileColor.Mango),
            new Tile(13, TileColor.Red)
        ]);


        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(set.Tiles.Count + 1, solution.GetSet().Tiles.Count);
    }

    [Fact]
    public void GetSolution_ReturnsValidGroupAndRunJoker3()
    {
        // Arrange
        var set = new Set([
            new Tile(1),
            new Tile(1, TileColor.Black),
            new Tile(1, TileColor.Mango),

            new Tile(1),
            new Tile(1, TileColor.Red),
            new Tile(1, TileColor.Mango),

            new Tile(2, TileColor.Mango),
            new Tile(3, TileColor.Mango),
            new Tile(true)
        ]);

        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(set.Tiles.Count + 1, solution.GetSet().Tiles.Count);
    }
    
    [Fact]
    public void GetSolution_ReturnsValidGroupAndRunJoker4()
    {
        // Arrange
        var set = new Set([
            new Tile(5, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(true), // Joker
            new Tile(8, TileColor.Black),
            new Tile(9, TileColor.Black),
            
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango),
            new Tile(13, TileColor.Mango),
            
            new Tile(9, TileColor.Mango),
            new Tile(10, TileColor.Mango),
            new Tile(11, TileColor.Mango),
            new Tile(12, TileColor.Mango),
            
            new Tile(8,TileColor.Mango),
            
        ]);

        // Act
        var solution = set.GetSolution();

        // Assert
        Assert.True(solution.IsValid);

        Assert.Equal(set.Tiles.Count + 1, solution.GetSet().Tiles.Count);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void GetBestSets_ReturnsCorrectNumberOfCombinationsForVariousK(int k)
    {
        // Arrange
        var tiles = new List<Tile>
        {
            new(1),
            new(2),
            new(3),
            new(4),
            new(5),
            new(6),
            new(7)
        };

        // Act
        var result = Set.GetBestSets(tiles, k).ToList();

        // Assert
        var expectedCount = Combination(tiles.Count, k);
        Assert.Equal(expectedCount, result.Count);
    }

    private static int Combination(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;

        var numerator = 1;
        var denominator = 1;
        for (var i = 1; i <= k; i++)
        {
            numerator *= n - (k - i);
            denominator *= i;
        }

        return numerator / denominator;
    }

    [Fact]
    public void GenerateRandomValidSet_ProducesValidSolution()
    {
        for (int i = 0; i < 10; i++)
        {
            var randSet = GenerateRandomValidSet();
            var solution = randSet.GetSolution();

            Assert.True(solution.IsValid, "La solution générée n'est pas valide.");
        }
    }

    private static Set GenerateRandomValidSet()
    {
        var random = new Random();
        var tiles = new List<Tile>();

        var numberOfRuns = random.Next(3, 7);
        var numberOfGroups = random.Next(3, 7);

        for (var i = 0; i < numberOfRuns; i++)
        {
            var color = (TileColor)random.Next(0, 4);
            var startNumber = random.Next(1, 13);
            var maxRunLength = 14 - startNumber;

            if (maxRunLength < 3) continue;
            var runLength = random.Next(3, maxRunLength + 1);

            var run = new List<Tile>();
            for (var j = 0; j < runLength; j++) run.Add(new Tile(startNumber + j, color));

            tiles.AddRange(run);
        }


        for (var i = 0; i < numberOfGroups; i++)
        {
            var number = random.Next(1, 14);
            var groupSize = random.Next(3, 5);

            var availableColors = new List<TileColor>
                { TileColor.Blue, TileColor.Red, TileColor.Mango, TileColor.Black };
            var group = new List<Tile>();
            for (var j = 0; j < groupSize; j++)
            {
                var color = availableColors[random.Next(availableColors.Count)];
                availableColors.Remove(color);
                group.Add(new Tile(number, color));
            }

            tiles.AddRange(group);
        }

        return new Set { Tiles = tiles };
    }
}