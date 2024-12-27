namespace RummiSolve.Solver;

public class IncrementalSolver : Solver
{
    private readonly bool[] _isPlayerTile;
    private readonly int _boardJokers;
    private readonly int _availableJokers;

    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => Tiles.Where((_, i) => _isPlayerTile[i] && _bestUsedTiles[i]);
    public int JokerToPlay => _availableJokers - _remainingJoker - _boardJokers;

    private IncrementalSolver(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardJokers) : base(tiles, jokers)
    {
        _availableJokers = jokers;
        _isPlayerTile = isPlayerTile;
        _boardJokers = boardJokers;
        _bestUsedTiles = UsedTiles;
        _bestSolutionScore = 1;
    }

    public static IncrementalSolver Create(Set boardSet, Set playerSet)
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
        
        return new IncrementalSolver(
            finalTiles,
            totalJokers,
            isPlayerTile,
            boardSet.Jokers
        );
    }

    public override bool SearchSolution()
    {
        if (Tiles.Length + Jokers <= 2) return false;
        
        while (true)
        {
            var newSolution = FindSolution(new Solution(), 0);

            if (!newSolution.IsValid) return false;
            BestSolution = newSolution;
            _bestUsedTiles = UsedTiles.ToArray();
            _bestSolutionScore = GetPlayerScore();
            _remainingJoker = Jokers;
            if (UsedTiles.All(b => b)) return true;
            Array.Fill(UsedTiles, false);
            Jokers = _availableJokers;
        }
    }

    protected override bool ValidateCondition()
    {
        var allBoardTilesUsed =
            !UsedTiles.Where((use, i) => !use && !_isPlayerTile[i]).Any(); //check pas de joker restant ?

        return allBoardTilesUsed && GetPlayerScore() > _bestSolutionScore;
    }

    private int GetPlayerScore()
    {
        return Tiles.Where((_, i) => _isPlayerTile[i] && UsedTiles[i]).Sum(t => t.Value);
    }

    protected override Solution FindSolution(Solution solution, int startIndex)
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
}