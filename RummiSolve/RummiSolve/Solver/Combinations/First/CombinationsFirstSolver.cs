using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public class CombinationsFirstSolver : ISolver
{
    private readonly List<Tile> _tiles;
    public bool Found { get; private set; }
    public Solution BestSolution { get; private set; } = new();
    public IEnumerable<Tile> TilesToPlay { get; private set; } = [];
    public int JokerToPlay { get; private set; }
    public bool Won { get; private set; }

    private CombinationsFirstSolver(List<Tile> tiles)
    {
        _tiles = tiles;
    }
    public static CombinationsFirstSolver Create(Set playerSet)
    {
        return new CombinationsFirstSolver(playerSet.Tiles);
    }

    public void SearchSolution()
    {
        _tiles.Sort();

        var tilesFirstTry = new List<Tile>(_tiles);

        var playerJokers = _tiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tilesFirstTry.RemoveRange(tilesFirstTry.Count - playerJokers, playerJokers);

        var firstBinarySolver = new BinaryFirstBaseSolver(tilesFirstTry.ToArray(), playerJokers);

        Found = firstBinarySolver.SearchSolution();

        if (Found)
        {
            BestSolution = firstBinarySolver.BinarySolution;
            TilesToPlay = tilesFirstTry;
            JokerToPlay = playerJokers;
            Won = true;
            return;
        }

        _tiles.Reverse();

        //BinaryFirstBaseSolver? bestSolver = null;

        for (var tileTry = _tiles.Count - 1; tileTry > 2; tileTry--)
        {
            // var size = tileTry;
            // Parallel.ForEach(BaseSolver.GetCombinations(tiles, tileTry),
            //     (combi, loopState) =>
            //     {
            //         if (bestSolver != null)
            //         {
            //             loopState.Stop();
            //             return;
            //         }
            //         
            //         var joker = combi.Count(tile => tile.IsJoker);
            //         if (joker > 0) combi.RemoveRange(size - joker, joker);
            //         var solver = new BinaryFirstBaseSolver(combi.ToArray(), joker);
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

            foreach (var combi in
                     BaseSolver.GetCombinations(_tiles, tileTry).OrderByDescending(l => l.Sum(t => t.Value)))
            {
                var joker = combi.Count(tile => tile.IsJoker);
                if (joker > 0) combi.RemoveRange(tileTry - joker, joker);
                var solver = new BinaryFirstBaseSolver(combi.ToArray(), joker);
                Found = solver.SearchSolution();
                if (!Found) continue;
                BestSolution = solver.BinarySolution;
                TilesToPlay = solver.TilesToPlay;
                JokerToPlay = solver.JokerToPlay;
                return;
            }
        }
    }
}