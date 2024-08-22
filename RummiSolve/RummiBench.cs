using BenchmarkDotNet.Attributes;

namespace RummiSolve;

public class RummiBench
{
    private readonly Set _exampleValidSet = new()
    {
        Tiles =
        [
            new Tile(1, Tile.Color.Blue),
            new Tile(2, Tile.Color.Blue),
            new Tile(3, Tile.Color.Blue),

            new Tile(7, Tile.Color.Red),
            new Tile(8, Tile.Color.Red),
            new Tile(9, Tile.Color.Red),

            new Tile(1, Tile.Color.Blue),
            new Tile(2, Tile.Color.Blue),
            new Tile(3, Tile.Color.Blue),

            new Tile(7, Tile.Color.Red),
            new Tile(8, Tile.Color.Red),
            new Tile(9, Tile.Color.Red),

            new Tile(5, Tile.Color.Blue),
            new Tile(5, Tile.Color.Red),
            new Tile(5, Tile.Color.Black),

            new Tile(10, Tile.Color.Mango),
            new Tile(11, Tile.Color.Mango),
            new Tile(12, Tile.Color.Mango),

            new Tile(7, Tile.Color.Red),
            new Tile(7, Tile.Color.Blue),
            new Tile(7, Tile.Color.Black),

            new Tile(4, Tile.Color.Black),
            new Tile(5, Tile.Color.Black),
            new Tile(6, Tile.Color.Black),

            new Tile(3, Tile.Color.Mango),
            new Tile(3, Tile.Color.Red),
            new Tile(3, Tile.Color.Black),

            new Tile(2, Tile.Color.Mango),
            new Tile(3, Tile.Color.Mango),
            new Tile(4, Tile.Color.Mango),

            new Tile(9, Tile.Color.Black),
            new Tile(9, Tile.Color.Mango),
            new Tile(9, Tile.Color.Red),

            new Tile(11, Tile.Color.Red),
            new Tile(12, Tile.Color.Red),
            new Tile(13, Tile.Color.Red),

            new Tile(6, Tile.Color.Blue),
            new Tile(6, Tile.Color.Mango),
            new Tile(6, Tile.Color.Black),

            new Tile(11, Tile.Color.Black),
            new Tile(12, Tile.Color.Black),
            new Tile(13, Tile.Color.Black),
        ]
    };

    private readonly List<Tile> _exampleRackTiles =
    [
        new Tile(4, Tile.Color.Blue),
        new Tile(5, Tile.Color.Blue),

        new Tile(6, Tile.Color.Red),

        new Tile(1, Tile.Color.Blue),
        new Tile(12, Tile.Color.Mango),

        new Tile(4, Tile.Color.Black),
        new Tile(5, Tile.Color.Black),
        new Tile(6, Tile.Color.Black),
        new Tile(3, Tile.Color.Black),

        new Tile(2, Tile.Color.Mango),
        new Tile(3, Tile.Color.Mango),
        new Tile(4, Tile.Color.Mango),

        new Tile(8, Tile.Color.Mango),
        new Tile(8, Tile.Color.Red)
    ];


    [Benchmark]
    public Solution OldHand()
    {
        var game = new Game
        {
            BoardSolution = _exampleValidSet.GetSolution(),
            RackTiles = _exampleRackTiles
        };
        return game.Solve(true);
    }

    public static void TestRandomValidSet()
    {
        var randSet = GenerateRandomValidSet().ShuffleTiles();
        randSet.PrintAllTiles();
        var randSol = randSet.GetSolution();

        while (randSol.IsValid)
        {
            randSet = GenerateRandomValidSet();
            randSet.PrintAllTiles();
            Console.WriteLine(randSet.Tiles.Length);
            randSet.ShuffleTiles();
            randSol = randSet.GetSolution();
            randSol.PrintSolution();
            Console.WriteLine();
        }

        randSol.PrintSolution();
    }

    public static void testCombi()
    {
        List<Tile> tryCombiSet =
        [
            new Tile(1, Tile.Color.Black),
            new Tile(1, Tile.Color.Black),
            new Tile(2, Tile.Color.Black),
            new Tile(3, Tile.Color.Black),
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
        var numberOfRuns = random.Next(1, 6); // 1 à 2 runs
        var numberOfGroups = random.Next(1, 6); // 1 à 2 groupes

        // Générer les runs
        for (var i = 0; i < numberOfRuns; i++)
        {
            var color = (Tile.Color)random.Next(0, 4); // Couleur aléatoire
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

            var availableColors = new List<Tile.Color>
                { Tile.Color.Blue, Tile.Color.Red, Tile.Color.Mango, Tile.Color.Black };
            var group = new List<Tile>();
            for (var j = 0; j < groupSize; j++)
            {
                var color = availableColors[random.Next(availableColors.Count)];
                availableColors.Remove(color); // Assurer que les couleurs sont uniques dans le groupe
                group.Add(new Tile(number, color));
            }

            tiles.AddRange(group);
        }

        return new Set { Tiles = tiles.ToArray() };
    }
}