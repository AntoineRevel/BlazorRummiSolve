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

            new Tile(10, Tile.Color.Yellow),
            new Tile(11, Tile.Color.Yellow),
            new Tile(12, Tile.Color.Yellow),

            new Tile(7, Tile.Color.Red),
            new Tile(7, Tile.Color.Blue),
            new Tile(7, Tile.Color.Black),

            new Tile(4, Tile.Color.Black),
            new Tile(5, Tile.Color.Black),
            new Tile(6, Tile.Color.Black),

            new Tile(3, Tile.Color.Yellow),
            new Tile(3, Tile.Color.Red),
            new Tile(3, Tile.Color.Black),

            new Tile(2, Tile.Color.Yellow),
            new Tile(3, Tile.Color.Yellow),
            new Tile(4, Tile.Color.Yellow),

            new Tile(9, Tile.Color.Black),
            new Tile(9, Tile.Color.Yellow),
            new Tile(9, Tile.Color.Red),

            new Tile(11, Tile.Color.Red),
            new Tile(12, Tile.Color.Red),
            new Tile(13, Tile.Color.Red),

            new Tile(6, Tile.Color.Blue),
            new Tile(6, Tile.Color.Yellow),
            new Tile(6, Tile.Color.Black),

            new Tile(11, Tile.Color.Black),
            new Tile(12, Tile.Color.Black),
            new Tile(13, Tile.Color.Black),
        ]
    };

    private readonly Set _exampleNotValidSet = new()
    {
        Tiles =
        [
            new Tile(1, Tile.Color.Blue),
            new Tile(2, Tile.Color.Blue),
            new Tile(3, Tile.Color.Blue),

            new Tile(7, Tile.Color.Red),
            new Tile(8, Tile.Color.Red),
            new Tile(9, Tile.Color.Red),

            new Tile(5, Tile.Color.Blue),
            new Tile(5, Tile.Color.Red),
            new Tile(5, Tile.Color.Black),

            new Tile(10, Tile.Color.Yellow),
            new Tile(11, Tile.Color.Yellow),
            new Tile(12, Tile.Color.Yellow),

            new Tile(7, Tile.Color.Red),
            new Tile(7, Tile.Color.Blue),
            new Tile(7, Tile.Color.Black),

            new Tile(4, Tile.Color.Black),
            new Tile(5, Tile.Color.Black),
            new Tile(6, Tile.Color.Black),

            new Tile(3, Tile.Color.Yellow),
            new Tile(3, Tile.Color.Red),
            new Tile(3, Tile.Color.Blue),

            new Tile(2, Tile.Color.Yellow),
            new Tile(3, Tile.Color.Yellow),
            new Tile(4, Tile.Color.Yellow),

            new Tile(9, Tile.Color.Black),
            new Tile(9, Tile.Color.Yellow),
            new Tile(9, Tile.Color.Red),

            new Tile(11, Tile.Color.Red),
            new Tile(12, Tile.Color.Red),
            new Tile(13, Tile.Color.Red),

            new Tile(6, Tile.Color.Blue),
            new Tile(6, Tile.Color.Yellow),
            new Tile(6, Tile.Color.Black),

            new Tile(12, Tile.Color.Black),
            new Tile(13, Tile.Color.Black),
        ]
    };

    private static readonly Set ExGroupSet = new()
    {
        Tiles =
        [
            new Tile(10, Tile.Color.Yellow),
            new Tile(13, Tile.Color.Black),
            new Tile(11, Tile.Color.Black),
            new Tile(12, Tile.Color.Black),
            new Tile(10, Tile.Color.Blue),
            new Tile(10, Tile.Color.Black),
        ]
    };
    
    private static readonly Set ExGroupSet2 = new()
    {
        Tiles =
        [
            new Tile(1, Tile.Color.Red),
            new Tile(2, Tile.Color.Red),
            new Tile(3, Tile.Color.Red),
            new Tile(4, Tile.Color.Red),
            new Tile(5, Tile.Color.Red),
            new Tile(6, Tile.Color.Red),
            new Tile(7, Tile.Color.Red),
            new Tile(8, Tile.Color.Red),
            new Tile(9, Tile.Color.Red),
            new Tile(10, Tile.Color.Red),
            new Tile(11, Tile.Color.Red),
            new Tile(12, Tile.Color.Red),
            
            new Tile(2, Tile.Color.Yellow),
            new Tile(2, Tile.Color.Black),
            new Tile(2, Tile.Color.Blue),
            
            new Tile(2, Tile.Color.Black),
            new Tile(2, Tile.Color.Blue),
            new Tile(2, Tile.Color.Red),
           
        ]
    };

    [Benchmark]
    public void GetSolutions()
    {
        var validLocalSet = _exampleValidSet.Copy();
        validLocalSet.GetSolution();

        var notValidLocalSet = _exampleNotValidSet.Copy();
        notValidLocalSet.GetSolution();
    }

    // [Benchmark]
    // public void GetSolutionsArray()
    // {
    //     var validLocalSet = _exampleValidSet.Copy();
    //     validLocalSet.GetSolutionArray();
    //     
    //     var notValidLocalSet = _exampleNotValidSet.Copy();
    //     notValidLocalSet.GetSolutionArray();
    // }

    public static void TestGroup()
    {
        ExGroupSet2.GetSolution().PrintSolution();
    }
    
    public static void TestBench()
    {
        var randSet = GenerateRandomValidSet().ShuffleTiles();
        randSet.PrintAllTiles();
        var randSol = randSet.GetSolution();

        while (randSol.IsValid)
        {
            randSet = GenerateRandomValidSet();
            randSet.PrintAllTiles();
            Console.WriteLine(randSet.Tiles.Count);
            randSet.ShuffleTiles();
            randSol = randSet.GetSolution();
        }

        randSol.PrintSolution();

        //randSolArray.PrintSolution();
    }

    private static Set GenerateRandomValidSet()
    {
        var random = new Random();
        var tiles = new List<Tile>();

        // Décider aléatoirement du nombre de runs et de groupes à générer
        int numberOfRuns = random.Next(1, 3); // 1 à 2 runs
        int numberOfGroups = random.Next(1, 3); // 1 à 2 groupes

        // Générer les runs
        for (int i = 0; i < numberOfRuns; i++)
        {
            var color = (Tile.Color)random.Next(0, 4); // Couleur aléatoire
            int startNumber = random.Next(1, 13); // Un run peut commencer entre 1 et 13
            int maxRunLength = 14 - startNumber; // Calculer la longueur maximale du run possible

            if (maxRunLength >= 3)
            {
                int runLength =
                    random.Next(3, maxRunLength + 1); // Longueur aléatoire entre 3 et la longueur maximale possible

                var run = new List<Tile>();
                for (int j = 0; j < runLength; j++)
                {
                    run.Add(new Tile(startNumber + j, color));
                }

                tiles.AddRange(run);
            }
        }

        // Générer les groupes
        for (int i = 0; i < numberOfGroups; i++)
        {
            int number = random.Next(1, 14); // Numéro aléatoire entre 1 et 13
            int groupSize = random.Next(3, 5); // Taille aléatoire entre 3 et 4

            var availableColors = new List<Tile.Color>
                { Tile.Color.Blue, Tile.Color.Red, Tile.Color.Yellow, Tile.Color.Black };
            var group = new List<Tile>();
            for (int j = 0; j < groupSize; j++)
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