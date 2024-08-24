using BenchmarkDotNet.Running;
using StackExchange.Redis;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {
        // Configuration de la connexion
        var options = new ConfigurationOptions
        {
            EndPoints = { "192.168.1.88:6379" },
            User = "Rummi.net", // Nom d'utilisateur
            Password = "votre_mot_de_passe", // Mot de passe de l'utilisateur
            AbortOnConnectFail = false, // Permet de ne pas échouer immédiatement en cas de problème de connexion
            Ssl = false // Activer si vous utilisez SSL
        };

        try
        {
            // Création de la connexion
            using (var connection = ConnectionMultiplexer.Connect(options))
            {
                // Accéder à la base de données Redis
                IDatabase db = connection.GetDatabase();

                // Exemple d'opération : SET et GET d'une clé
                db.StringSet("cle", "valeur");
                string valeur = db.StringGet("cle");
                Console.WriteLine($"La valeur pour 'cle' est : {valeur}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la connexion à Redis : {ex.Message}");
        }
    }

    public static void TestBench()
    {
        var rummiBench = new RummiBench();
        //rummiBench.OldHand().PrintSolution();
        rummiBench.getSolValid();
    }

    public static void PlaySoloGame()
    {
        var game = new Game();
        game.PlaySoloGame();
    }

    public static void TestRand()
    {
        RummiBench.TestRandomValidSet();
    }

    public static void Play()
    {
        var game = new Game();
        game.StartConsole();
    }
}