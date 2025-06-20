using RummiSolve.Solver.Abstract;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver.Combinations.First;

public class CombinationsFirstSolver : ISolver
{
    private readonly List<Tile> _tiles;


    private CombinationsFirstSolver(List<Tile> tiles)
    {
        _tiles = tiles;
    }

    public SolverResult SearchSolution()
    {
        _tiles.Sort();

        var tilesFirstTry = new List<Tile>(_tiles);

        var playerJokers = _tiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tilesFirstTry.RemoveRange(tilesFirstTry.Count - playerJokers, playerJokers);

        var firstBinarySolver = new BinaryFirstBaseSolver(tilesFirstTry.ToArray(), playerJokers);

        var found = firstBinarySolver.SearchSolution();

        if (found) return new SolverResult(firstBinarySolver.BinarySolution, tilesFirstTry, playerJokers, true);

        _tiles.Reverse();

        for (var tileTry = _tiles.Count - 1; tileTry > 2; tileTry--)
        {
            foreach (
                var combi in
                BaseSolver.GetCombinations(_tiles, tileTry)
                    .OrderByDescending(l => l.Sum(t => t.Value)))
            {
                var joker = combi.Count(tile => tile.IsJoker);
                if (joker > 0) combi.RemoveRange(tileTry - joker, joker);
                var solver = new BinaryFirstBaseSolver(combi.ToArray(), joker);
                found = solver.SearchSolution();
                if (!found) continue;
                return new SolverResult(solver.BinarySolution, solver.TilesToPlay, solver.JokerToPlay);
            }
        }

        return SolverResult.Invalid;
    }

    public static CombinationsFirstSolver Create(Set playerSet)
    {
        return new CombinationsFirstSolver(playerSet.Tiles);
    }
}