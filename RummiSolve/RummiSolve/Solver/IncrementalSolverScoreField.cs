using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Solver;

public sealed class IncrementalSolverScoreField : SolverBase, IIncrementalSolver
{
    private readonly bool[] _isPlayerTile;
    private readonly int _boardJokers;
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _solutionScore;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _isPlayerTile[i] && _bestUsedTiles[i]);
    public int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    private IncrementalSolverScoreField(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(tiles,
        jokers)
    {
        _availableJokers = jokers;
        _isPlayerTile = isPlayerTile;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
        _bestSolutionScore = 1;
    }

    public static IncrementalSolverScoreField Create(Set boardSet, Set playerSet)
    {
        var capacity = boardSet.Tiles.Count + playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(boardSet.Tiles.Select(tile => (tile, false)));
        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = boardSet.Jokers + playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new IncrementalSolverScoreField(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers
        );
    }

    public bool SearchSolution()
    {
        if (Tiles.Length + Jokers <= 2) return false;

        while (true)
        {
            var newSolution = FindSolution(new Solution(), 0);

            if (!newSolution.IsValid) return false;
            BestSolution = newSolution;
            _bestSolutionScore = _solutionScore;
            _bestUsedTiles = UsedTiles.ToArray();
            _remainingJoker = Jokers;
            //PrintInfo();
            if (UsedTiles.All(b => b)) return true;
            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
            _solutionScore = 0;
        }
    }

    private void PrintInfo()
    {
        BestSolution.PrintSolution();
        Console.WriteLine();
        Console.WriteLine("Tile to play");
        foreach (var tile in TilesToPlay)
        {
            tile.PrintTile();
        }

        Console.WriteLine();
        Console.WriteLine(_bestSolutionScore);
    }

    private bool ValidateCondition(int solutionScore)
    {
        var allBoardTilesUsed =
            !UsedTiles.Where((use, i) => !use && !_isPlayerTile[i]).Any(); //check pas de joker restant ?

        return allBoardTilesUsed && solutionScore > _bestSolutionScore;
    }


    private Solution FindSolution(Solution solution, int startIndex)
    {
        while (startIndex < UsedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(UsedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TrySet(GetRuns(startIndex), solution, startIndex,
                (sol, run) => sol.AddRun(run));

            if (solRun.IsValid) return solRun;

            var solGroup = TrySet(GetGroups(startIndex), solution, startIndex,
                (sol, group) => sol.AddGroup(group));

            if (solGroup.IsValid) return solGroup;

            if (_isPlayerTile[startIndex]) startIndex++;
            else return solution;
        }

        return solution;
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        UsedTiles[firstUnusedTileIndex] = true;
        _solutionScore += _isPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, firstUnusedTileIndex);

            // var normal = Tiles.Where((_, i) => _isPlayerTile[i] && UsedTiles[i]).Sum(t => t.Value);
            // if (_solutionScore != normal) throw new Exception();

            if (ValidateCondition(_solutionScore)) solution.IsValid = true;

            else solution = FindSolution(solution, firstUnusedTileIndex);

            if (solution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUnused(set, firstUnusedTileIndex);
        }

        UsedTiles[firstUnusedTileIndex] = false;
        _solutionScore -= _isPlayerTile[firstUnusedTileIndex] ? Tiles[firstUnusedTileIndex].Value : 0;

        return solution;
    }

    private new void MarkTilesAsUsed(ValidSet set, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles.Skip(1))
        {
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (var i = firstUnusedIndex + 1; i < Tiles.Length; i++)
            {
                if (UsedTiles[i] || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = true;

                if (_isPlayerTile[i])
                {
                    _solutionScore += tile.Value;
                }

                break;
            }
        }
    }

    private new void MarkTilesAsUnused(ValidSet set, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles.Skip(1))
        {
            if (tile.IsJoker)
            {
                Jokers += 1;
                continue;
            }

            for (var i = firstUnusedIndex + 1; i < Tiles.Length; i++)
            {
                if (!UsedTiles[i] || !Tiles[i].Equals(tile)) continue;

                UsedTiles[i] = false;

                if (_isPlayerTile[i])
                {
                    _solutionScore -= tile.Value;
                }

                break;
            }
        }
    }
}