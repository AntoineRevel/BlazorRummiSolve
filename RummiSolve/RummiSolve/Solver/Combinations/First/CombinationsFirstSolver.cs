using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public class CombinationsFirstSolver(List<Tile> tiles) : ISolver
{
    public bool Found { get; private set; }
    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay { get; private set; } = [];
    public int JokerToPlay { get; private set; }
    public bool Won { get; private set; }

    public static CombinationsFirstSolver Create(Set playerSet)
    {
        return new CombinationsFirstSolver(playerSet.Tiles);
    }

    public void SearchSolution()
    {
        tiles.Sort();

        var tilesFirstTry = new List<Tile>(tiles);

        var playerJokers = tiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tilesFirstTry.RemoveRange(tilesFirstTry.Count - playerJokers, playerJokers);

        var firstBinarySolver = new BinaryFirstBaseSolver(tilesFirstTry.ToArray(), playerJokers);

        var isValid = firstBinarySolver.SearchSolution();

        if (isValid)
        {
            Found = true;
            BestSolution = firstBinarySolver.BinarySolution;
            TilesToPlay = tilesFirstTry;
            JokerToPlay = playerJokers;
            Won = true;
            return;
        }

        tiles.Reverse();

        BinaryFirstBaseSolver? bestSolver = null;

        for (var tileTry = tiles.Count - 1; tileTry > 2; tileTry--)
        {
            // Parallel.ForEach(BaseSolver.GetCombinations(tiles, tileTry),
            //     (combi, loopState) =>
            //     {
            //         if (bestSolver != null)
            //         {
            //             loopState.Stop();
            //             return;
            //         }
            //
            //         var solver = new BinaryFirstBaseSolver(combi.ToArray(), combi.Count(tile => tile.IsJoker));
            //         var found = solver.SearchSolution();
            //         if (!found) return;
            //         bestSolver = solver;
            //         loopState.Stop();
            //     });
            //
            // if (bestSolver == null) continue;
            //
            // Found = true;
            // BestSolution = bestSolver.BinarySolution;
            // TilesToPlay = bestSolver.TilesToPlay;
            // JokerToPlay = bestSolver.JokerToPlay;
            // return;

            foreach (var combi in BaseSolver.GetCombinations(tiles, tileTry))
            {
                var joker = combi.Count(tile => tile.IsJoker);
                if (joker > 0) combi.RemoveRange(tileTry - joker, joker);
                var solver = new BinaryFirstBaseSolver(combi.ToArray(), joker);
                Found = solver.SearchSolution();
                if (!Found) continue;
                BestSolution = solver.BinarySolution;
                TilesToPlay = solver.TilesToPlay;
                JokerToPlay = joker;
                return;
            }
        }
    }
}