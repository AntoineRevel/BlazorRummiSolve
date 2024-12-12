using System.Diagnostics;
using BenchmarkDotNet.Attributes;

namespace RummiSolve;

[MemoryDiagnoser]
public class RummiBench
{
    private static readonly Set Set = new([
        new Tile((byte)21),
        new Tile((byte)57),
        new Tile((byte)58),
        new Tile((byte)22),
        new Tile((byte)11),

        new Tile((byte)8),
        new Tile((byte)10),
        new Tile((byte)12),
        new Tile((byte)44),
        new Tile((byte)60),
        new Tile((byte)13),
        new Tile((byte)29),
        new Tile((byte)61),
        new Tile(true)
    ]);

    private static readonly bool[] TabBool = [false, false, false, false, false];


    public static void TestVarRecu()
    {
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
        solB.PrintSolution();

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

        player._played = true;
        var newSol = player.Solve(solB);

        newSol.PrintSolution();
    }


    public static void TestFirstSol()
    {
        var solution = Set.GetSolution();
        solution.PrintSolution();
    }

    
    public void TestMultiPlayerGameNoStatic()
    {
        var game = new Game();
        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }

    public static void TestMultiPlayerGame()
    {
        Game game = new(Guid.Parse("09608d3e-826f-4d6e-bdb0-56b58b46e71f")); 
        //Guid.Parse("9e1bf68d-78eb-436d-8ed9-c148e98a991b")
        
        var listNames = new List<string> { "Antoine", "Matthieu", "Maguy" };
        game.InitializeGame(listNames);
        var gameStopwatch = Stopwatch.StartNew();
        game.Start();
        gameStopwatch.Stop();
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }

    public static void TestGroupJoker()
    {
        var groupSet = new Set
        {
            Tiles =
            [
                new Tile(1),
                new Tile(1, TileColor.Black),
                new Tile(true),
                new Tile(true),


                new Tile(2),
                new Tile(3),
                new Tile(4)
            ]
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
            ]
        };

        var boardSet = new Set
        {
            Tiles =
            [
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
            ]
        };


        setToTest.PrintAllTiles();
        Console.WriteLine();

        var boardSol = boardSet.GetSolution();

        boardSol.PrintSolution();
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
            new(3),
            new(4, TileColor.Mango)
        ];

        foreach (var tiles in Set.GetBestSets(tryCombiSet, 3))
        {
            foreach (var tile in tiles.Tiles) tile.PrintTile();

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
            for (var j = 0; j < runLength; j++) run.Add(new Tile(startNumber + j, color));

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

    public static void TestSupMark()
    {
        var set = new Set([
            
            new Tile(1),
            new Tile(1,TileColor.Black),
            new Tile(1,TileColor.Mango),
            
            new Tile(1),
            new Tile(1,TileColor.Red),
            new Tile(1,TileColor.Mango),
            
            new Tile(2,TileColor.Mango),
            new Tile(3,TileColor.Mango),
            new Tile(true)
        ]);

        var sol = set.GetSolution();

        sol.PrintSolution();
    }
    
    public static void TestSupMark2()
    {
        var set = new Set([
            
            new Tile(10,TileColor.Black),
            new Tile(11,TileColor.Black),
            new Tile(12,TileColor.Black),
            
            new Tile(9,TileColor.Mango),
            new Tile(10,TileColor.Mango),
            new Tile(11,TileColor.Mango),
            new Tile(12,TileColor.Mango),
            
            new Tile(10,TileColor.Red),
            new Tile(11,TileColor.Red),
            new Tile(true),
            
            new Tile(8),
            new Tile(9),
            new Tile(10),
            new Tile(11),
            
            new Tile(13),
            new Tile(13,TileColor.Mango),
            new Tile(13,TileColor.Red)
        ]);



        Console.WriteLine(set.Tiles.Count);
        var gameStopwatch = Stopwatch.StartNew();
        var sol = set.GetSolution();
        gameStopwatch.Stop();
        
        sol.PrintSolution();
        Console.WriteLine(sol.GetSet().Tiles.Count);
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    
    public static void TestSupMark3()
    {
        var set = new Set([
            new Tile(11),
            new Tile(11,TileColor.Red),
            new Tile(11,TileColor.Black),


            new Tile(11,TileColor.Mango),
            new Tile(12,TileColor.Mango), //lui
            new Tile(true),
            
        ]);
        

        Console.WriteLine(set.Tiles.Count);
        var gameStopwatch = Stopwatch.StartNew();
        var sol = set.GetSolution();
        gameStopwatch.Stop();
        
        sol.PrintSolution();
        Console.WriteLine(sol.GetSet().Tiles.Count);
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    
    public static void TestSupMark4()
    {
        var set = new Set([
            
            new Tile(7),
            new Tile(7,TileColor.Mango),
            new Tile(7,TileColor.Black),
            new Tile(7,TileColor.Red),
            
            new Tile(1,TileColor.Black),
            new Tile(2,TileColor.Black),
            new Tile(3,TileColor.Black),

            new Tile(7,TileColor.Red),
            new Tile(8,TileColor.Red),
            new Tile(9,TileColor.Red),

            new Tile(6,TileColor.Red),
            new Tile(6,TileColor.Mango),
            new Tile(6,TileColor.Black),
            
            
        ]);
        

        Console.WriteLine(set.Tiles.Count);
        var gameStopwatch = Stopwatch.StartNew();
        var sol = set.GetSolution();
        gameStopwatch.Stop();
        
        sol.PrintSolution();
        Console.WriteLine(sol.GetSet().Tiles.Count);
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
    public static void TestSupMark5()
    {
        var set = new Set([
            
            new Tile(11,TileColor.Red),
            new Tile(true),
            new Tile(13,TileColor.Red)
        ]);
        

        Console.WriteLine(set.Tiles.Count);
        var gameStopwatch = Stopwatch.StartNew();
        var sol = set.GetFirstSolution();
        gameStopwatch.Stop();
        
        sol.PrintSolution();
        Console.WriteLine(sol.GetSet().Tiles.Count);
        Console.WriteLine($"Game duration: {gameStopwatch.Elapsed.TotalSeconds} seconds");
    }
    
}