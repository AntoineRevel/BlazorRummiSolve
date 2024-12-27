namespace RummiSolve;

public class IncrementalFirstSolver
{
    private readonly Tile[] _tiles;
    private readonly bool[] _usedTiles;
    private readonly bool[] _isPlayerTile;
    private readonly int _availableJokers;
    private int _jokers;

    public Solution BestSolution { get; private set; } = new();
    private bool[] _bestUsedTiles;
    private int _remainingJoker;
    private int _bestSolutionScore;

    public IEnumerable<Tile> TilesToPlay => _tiles.Where((_, i) => _isPlayerTile[i] && _bestUsedTiles[i]);
    public int JokerToPlay => _availableJokers - _remainingJoker;

    private IncrementalFirstSolver(Tile[] tiles, int jokers, bool[] isPlayerTile)
    {
        _tiles = tiles;
        _jokers = jokers;
        _availableJokers = jokers;
        _usedTiles = new bool[tiles.Length];
        _isPlayerTile = isPlayerTile;
        _bestUsedTiles = _usedTiles;
        _bestSolutionScore = 29;
    }

    public static IncrementalFirstSolver Create(Set playerSet)
    {
        var capacity = playerSet.Tiles.Count;
        var combined = new List<(Tile tile, bool isPlayerTile)>(capacity);

        combined.AddRange(playerSet.Tiles.Select(tile => (tile, true)));

        var totalJokers = playerSet.Jokers;

        combined.Sort((x, y) =>
        {
            var tileCompare = x.tile.CompareTo(y.tile);
            return tileCompare != 0 ? tileCompare : x.isPlayerTile.CompareTo(y.isPlayerTile);
        });

        if (totalJokers > 0) combined.RemoveRange(combined.Count - totalJokers, totalJokers);

        var finalTiles = combined.Select(pair => pair.tile).ToArray();
        var isPlayerTile = combined.Select(pair => pair.isPlayerTile).ToArray();

        return new IncrementalFirstSolver(
            finalTiles,
            totalJokers,
            isPlayerTile
        );
    }

    private bool ValidateCondition(int solutionScore)
    {
        var allBoardTilesUsed =
            !_usedTiles.Where((use, i) => !use && !_isPlayerTile[i]).Any(); //check pas de joker restant ?

        if (!allBoardTilesUsed || solutionScore <= _bestSolutionScore) return false;

        _bestSolutionScore = solutionScore;
        return true;
    }


    public bool SearchSolution()
    {
        if (_tiles.Length + _jokers <= 2) return false;


        while (true)
        {
            var newSolution = FindSolution(new Solution(), 0, 0);

            if (!newSolution.IsValid) return false;
            BestSolution = newSolution;
            _bestUsedTiles = _usedTiles.ToArray();
            _remainingJoker = _jokers;
            Array.Fill(_usedTiles, false);
            _jokers = _availableJokers;
        }
    }


    private Solution FindSolution(Solution solution, int solutionScore, int startIndex)
    {
        startIndex = Array.FindIndex(_usedTiles, startIndex, used => !used);

        if (startIndex == -1) return solution;

        var solRun = TrySet(GetRuns(startIndex), solution, solutionScore, startIndex,
            (sol, run) => sol.AddRun(run));

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(startIndex), solution, solutionScore, startIndex,
            (sol, group) => sol.AddGroup(group));

        return solGroup;
    }


    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        _usedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, firstUnusedTileIndex);
            var newSolution = solution;

            var newSolutionScore = solutionScore + set.GetScore();

            if (ValidateCondition(newSolutionScore)) solution.IsValid = true;
            else newSolution = FindSolution(solution, newSolutionScore, firstUnusedTileIndex);

            if (newSolution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUsed(set, false, firstUnusedTileIndex);
        }

        _usedTiles[firstUnusedTileIndex] = false;

        return solution;
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles[0].IsJoker ? set.Tiles.Skip(2) : set.Tiles.Skip(1))
        {
            for (var i = firstUnusedIndex + 1; i < _tiles.Length; i++)
            {
                if (_usedTiles[i] == isUsed || !_tiles[i].Equals(tile)) continue;

                _usedTiles[i] = isUsed;
                break;
            }
        }

        _jokers += isUsed ? -set.Jokers : set.Jokers;
    }

    private IEnumerable<Run> GetRuns(int tileIndex)
    {
        var availableJokers = _jokers;
        var color = _tiles[tileIndex].Color;
        var currentRun = new List<Tile> { _tiles[tileIndex] };
        var i = tileIndex + 1;

        while (true)
        {
            for (; i < _tiles.Length; i++)
            {
                if (_tiles[i].Color != color)
                {
                    i = _tiles.Length;
                    break;
                }

                if (_usedTiles[i] || _tiles[i].Value == currentRun[^1].Value) continue;

                if (_tiles[i].Value != currentRun[^1].Value + 1) break;

                currentRun.Add(_tiles[i]);

                if (currentRun.Count < 3) continue;

                var jokersUsed = _jokers - availableJokers;

                yield return new Run
                {
                    Tiles = [..currentRun],
                    Jokers = jokersUsed
                };
            }

            if (availableJokers <= 0) yield break;

            if (currentRun[^1].Value != 13) currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));

            else if (currentRun[0].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));

            availableJokers--;

            if (currentRun.Count < 3) continue;

            var jokersUsedJ = _jokers - availableJokers;

            yield return new Run
            {
                Tiles = [..currentRun],
                Jokers = jokersUsedJ
            };
        }
    }

    private IEnumerable<Group> GetGroups(int firstTileIndex)
    {
        var sameNumberTiles = _tiles
            .Where((tile, index) => !_usedTiles[index] &&
                                    tile.Value == _tiles[firstTileIndex].Value &&
                                    tile.Color != _tiles[firstTileIndex].Color)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

        if (_jokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { _tiles[firstTileIndex] };

            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
            {
                yield return new Group
                {
                    Tiles = [_tiles[firstTileIndex], sameNumberTiles[i], sameNumberTiles[j]]
                };
            }
        }
        else
        {
            for (var jokersUsed = 0; jokersUsed <= _jokers; jokersUsed++)
            for (var tilesUsed = 2; tilesUsed <= size + 1; tilesUsed++)
            {
                var totalTiles = tilesUsed + jokersUsed;
                if (totalTiles is < 3 or > 4) continue;

                var combinations = GetCombinations(sameNumberTiles, tilesUsed - 1);

                foreach (var tiles in combinations)
                {
                    var groupTiles = new Tile[totalTiles];

                    groupTiles[0] = _tiles[firstTileIndex];

                    for (var i = 0; i < tilesUsed - 1; i++) groupTiles[i + 1] = tiles[i];

                    for (var k = 0; k < jokersUsed; k++)
                        groupTiles[tilesUsed + k] = new Tile(_tiles[firstTileIndex].Value, isJoker: true);

                    yield return new Group
                    {
                        Tiles = groupTiles,
                        Jokers = jokersUsed
                    };
                }
            }
        }
    }


    private static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length)
    {
        if (length == 0) yield return [];

        for (var i = 0; i < list.Count; i++)
        {
            var element = list[i];
            foreach (var combination in GetCombinations(list.Skip(i + 1).ToList(), length - 1))
            {
                combination.Add(element);
                yield return combination;
            }
        }
    }
}