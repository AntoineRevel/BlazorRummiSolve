using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace RummiSolve;

[MemoryDiagnoser]
public class RummiBench
{
    private static readonly Set Set = new()
    {
        Tiles =
        [
            new Tile(1, TileColor.Black),
            new Tile(2, TileColor.Black),
            new Tile(2, TileColor.Black),
            new Tile(3, TileColor.Black),
            new Tile(4, TileColor.Black),
            new Tile(5, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(7, TileColor.Black),
            new Tile(8, TileColor.Black),
            new Tile(9, TileColor.Black),
            new Tile(10, TileColor.Mango),
        ]
    };
    
    private static readonly Set SetGroup = new()
    {
        Tiles =
        [
            new Tile(1, TileColor.Black),
            new Tile(1, TileColor.Mango),
            new Tile(1, TileColor.Red),
            new Tile(1, TileColor.Red),
            new Tile(1, TileColor.Blue),
        ]
    };

    private static readonly bool[] TabBool = [false, false, false,false,false];

    [Benchmark]
    public void NoSpan()
    {
        Set.GetRuns(0, TabBool, 1);
    }

    [Benchmark]
    public void WithSpan()
    {
        Set.GetRunsSpan(0, TabBool, 1);
    }

    public static void TestRunSpan()
    {
        var result = Set.GetRuns(0, TabBool, 1);
        foreach (var run in result)
        {
            foreach (var tile in run.Tiles)
            {
                tile.PrintTile();
            }

            Console.WriteLine();
            Console.WriteLine(run.Jokers);
        }
    }

  
    
    public static void TestYieldGroup()
    {
        var result = SetGroup.GetGroups(0, TabBool, 2);
        foreach (var run in result)
        {
            foreach (var tile in run.Tiles)
            {
                tile.PrintTile();
            }

            Console.WriteLine();
            Console.WriteLine(run.Jokers);
        }
    }

    public static void TestMultiPlayerGame()
    {
        var game = new Game();
        game.AddPlayer("Antoine");
        game.AddPlayer("Matthieu");
        game.AddPlayer("David");
        game.AddPlayer("Maguy");
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }

    public static void TestGroupJoker()
    {
        var groupSet = new Set()
        {
            Tiles =
            [
                new Tile(1, TileColor.Blue),
                new Tile(1, TileColor.Black),
                new Tile(true),
                new Tile(true),


                new Tile(2, TileColor.Blue),
                new Tile(3, TileColor.Blue),
                new Tile(4, TileColor.Blue),
            ],
        };

        groupSet.PrintAllTiles();

        Console.WriteLine();
        groupSet.GetSolution().PrintSolution();
    }

    public static void TestJoker()
    {
        var setToTest = new Set
        {
            Tiles =
            [
                new Tile(11, TileColor.Mango),
                new Tile(10, TileColor.Mango),
                new Tile(true)
            ],
        };

        var boardSet = new Set
        {
            Tiles =
            [
                new Tile(12, TileColor.Blue),
                new Tile(12, TileColor.Red),
                new Tile(12, TileColor.Black),

                new Tile(11, TileColor.Blue),
                new Tile(11, TileColor.Mango),
                new Tile(11, TileColor.Black),

                new Tile(6, TileColor.Blue),
                new Tile(6, TileColor.Red),
                new Tile(6, TileColor.Mango),

                new Tile(5, TileColor.Blue),
                new Tile(5, TileColor.Mango),
                new Tile(5, TileColor.Black),
            ],
        };

        setToTest.Sort();

        setToTest.PrintAllTiles();
        Console.WriteLine();

        var boardSol = boardSet.GetSolution();

        boardSol.PrintSolution();

        var game = new Game()
        {
            BoardSolution = boardSol
        };
    }

    public static void TestRandomValidSet()
    {
        var isValid = true;

        while (isValid)
        {
            var randSet = GenerateRandomValidSet();
            randSet.PrintAllTiles();
            Console.WriteLine("ok");
            var shuffleTiles = randSet.ShuffleTiles();
            shuffleTiles.PrintAllTiles();
            Console.WriteLine(shuffleTiles.Tiles.Count);
            var randSol = randSet.GetSolution();
            randSol.PrintSolution();
            isValid = randSol.IsValid;
            Console.WriteLine("   ---   ");
        }
    }

    public static void testCombi()
    {
        List<Tile> tryCombiSet =
        [
            new(1, TileColor.Black),
            new(2, TileColor.Red),
            new(3, TileColor.Blue),
            new(4, TileColor.Mango)

        ];

        foreach (var tiles in Set.GetBestSets(tryCombiSet, 3))
        {
            foreach (var tile in tiles.Tiles)
            {
                tile.PrintTile();
            }

            Console.WriteLine();
        }
    }

    private static Set GenerateRandomValidSet()
    {
        var random = new Random();
        var tiles = new List<Tile>();

        // Décider aléatoirement du nombre de runs et de groupes à générer
        var numberOfRuns = random.Next(3, 7); // 1 à 2 runs
        var numberOfGroups = random.Next(3, 7); // 1 à 2 groupes

        // Générer les runs
        for (var i = 0; i < numberOfRuns; i++)
        {
            var color = (TileColor)random.Next(0, 4); // Couleur aléatoire
            var startNumber = random.Next(1, 13); // Un run peut commencer entre 1 et 13
            var maxRunLength = 14 - startNumber; // Calculer la longueur maximale du run possible

            if (maxRunLength < 3) continue;
            var runLength =
                random.Next(3, maxRunLength + 1); // Longueur aléatoire entre 3 et la longueur maximale possible

            var run = new List<Tile>();
            for (var j = 0; j < runLength; j++)
            {
                run.Add(new Tile(startNumber + j, color));
            }

            tiles.AddRange(run);
        }

        // Générer les groupes
        for (var i = 0; i < numberOfGroups; i++)
        {
            var number = random.Next(1, 14); // Numéro aléatoire entre 1 et 13
            var groupSize = random.Next(3, 5); // Taille aléatoire entre 3 et 4

            var availableColors = new List<TileColor>
                { TileColor.Blue, TileColor.Red, TileColor.Mango, TileColor.Black };
            var group = new List<Tile>();
            for (var j = 0; j < groupSize; j++)
            {
                var color = availableColors[random.Next(availableColors.Count)];
                availableColors.Remove(color); // Assurer que les couleurs sont uniques dans le groupe
                group.Add(new Tile(number, color));
            }

            tiles.AddRange(group);
        }

        return new Set { Tiles = tiles };
    }
}