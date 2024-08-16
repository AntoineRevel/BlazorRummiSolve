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

    [Benchmark]
    public void GetSolutions()
    {
        var validLocalSet = _exampleValidSet.Copy();
        validLocalSet.GetSolution();
        
        var notValidLocalSet = _exampleNotValidSet.Copy();
        notValidLocalSet.GetSolution();
    }
    
    [Benchmark]
    public void GetSolutionsArray()
    {
        var validLocalSet = _exampleValidSet.Copy();
        validLocalSet.GetSolutionArray();
        
        var notValidLocalSet = _exampleNotValidSet.Copy();
        notValidLocalSet.GetSolutionArray();
    }
    
    public Set GenerateRandomValidSet()
    {
        var random = new Random();
        var tiles = new List<Tile>();

        // Décider aléatoirement du nombre de groupes et de runs à générer
        int numberOfRuns = random.Next(1, 3); // 1 à 2 runs
        int numberOfGroups = random.Next(1, 3); // 1 à 2 groupes

        // Générer les runs
        for (int i = 0; i < numberOfRuns; i++)
        {
            var color = (Tile.Color)random.Next(0, 4); // Couleur aléatoire
            int startNumber = random.Next(1, 11); // Un run peut commencer entre 1 et 10 pour permettre un run de 3 tuiles

            var run = new List<Tile>();
            for (int j = 0; j < 3; j++) // Toujours 3 tuiles par run
            {
                run.Add(new Tile(startNumber + j, color));
            }
            tiles.AddRange(run);
        }

        // Générer les groupes
        for (int i = 0; i < numberOfGroups; i++)
        {
            int number = random.Next(1, 14); // Numéro aléatoire entre 1 et 13

            var availableColors = new List<Tile.Color> { Tile.Color.Blue, Tile.Color.Red, Tile.Color.Yellow, Tile.Color.Black };
            var group = new List<Tile>();
            for (int j = 0; j < 3; j++) // Toujours 3 tuiles par groupe
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