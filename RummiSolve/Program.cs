using BenchmarkDotNet.Running;
using DotNetEnv;
using StackExchange.Redis;

namespace RummiSolve;

public static class Program
{
    private static void Main(string[] args)
    {


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

    public static void Redis()
    {
        Env.Load("/Users/antoinerevel/Documents/Projet perso/RummiSolve/RummiSolve/RummiSolve/.env");

        var redisUser = Env.GetString("REDIS_USER");
        var redisPassword = Env.GetString("REDIS_PASSWORD");
        var redisHost = Env.GetString("REDIS_HOST");
        var redisPort = Env.GetString("REDIS_PORT");
        
        var options = new ConfigurationOptions
        {
            EndPoints = { $"{redisHost}:{redisPort}" },
            User = redisUser,
            Password = redisPassword,
            AbortOnConnectFail = false,
            Ssl = false
        };

        var redisConn = ConnectionMultiplexer.Connect(options);
        var db = redisConn.GetDatabase();
        db.Ping();
        db.StringGetSet("foo","bar");
        Console.WriteLine(db.StringGet("foo"));
    }
}