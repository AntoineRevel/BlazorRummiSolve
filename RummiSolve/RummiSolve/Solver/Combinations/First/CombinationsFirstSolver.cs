using RummiSolve.Results;
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

    public SolverResult SearchSolution(CancellationToken cancellationToken = default)
    {
        _tiles.Sort();

        var tilesFirstTry = new List<Tile>(_tiles);

        var playerJokers = _tiles.Count(tile => tile.IsJoker);

        if (playerJokers > 0) tilesFirstTry.RemoveRange(tilesFirstTry.Count - playerJokers, playerJokers);

        var firstBinarySolver = new BinaryFirstBaseSolver(tilesFirstTry.ToArray(), playerJokers);

        var result = firstBinarySolver.SearchSolution();
        var found = result.Found;

        if (found)
            return new SolverResult(GetType().Name, firstBinarySolver.BinarySolution, tilesFirstTry, playerJokers,
                true);

        _tiles.Reverse();

        for (var tileTry = _tiles.Count - 1; tileTry > 2; tileTry--)
        {
            if (cancellationToken.IsCancellationRequested)
                return new SolverResult(GetType().Name);

            foreach (
                var combi in
                BaseSolver.GetCombinations(_tiles, tileTry, cancellationToken)
                    .OrderByDescending(l => l.Sum(t => t.Value)))
            {
                if (cancellationToken.IsCancellationRequested)
                    return new SolverResult(GetType().Name);

                var joker = combi.Count(tile => tile.IsJoker);
                if (joker > 0) combi.RemoveRange(tileTry - joker, joker);
                var solver = new BinaryFirstBaseSolver(combi.ToArray(), joker);
                var solverResult = solver.SearchSolution();
                found = solverResult.Found;
                if (!found) continue;
                return new SolverResult(GetType().Name, solver.BinarySolution, solver.TilesToPlay, solver.JokerToPlay);
            }
        }

        return new SolverResult(GetType().Name);
        ;
    }

    public static CombinationsFirstSolver Create(Set playerSet)
    {
        return new CombinationsFirstSolver(playerSet.Tiles);
    }
}