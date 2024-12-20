namespace RummiSolve;

public class Set : ISet
{
    private bool _isSorted;
    private int _jokers;
    public List<Tile> Tiles;
    private bool[] _usedTiles = [];

    public Set()
    {
        Tiles = [];
    }

    public Set(List<Tile> tiles)
    {
        Tiles = [..tiles];
        _jokers = Tiles.Count(tile => tile.IsJoker);
    }

    public Set(Set set)
    {
        Tiles = [..set.Tiles];
        _jokers = set._jokers;
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }

    public void AddTile(Tile tile)
    {
        Tiles.Add(tile);
        if (tile.IsJoker) _jokers++;
        _isSorted = false;
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        Tiles.AddRange(set.Tiles);
        _jokers += set.Jokers;
        _isSorted = false;
    }

    public Set ConcatNew(Set set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var newSet = new Set
        {
            Tiles = [..Tiles],
            _jokers = _jokers
        };

        newSet.Tiles.AddRange(set.Tiles);
        newSet._jokers += set._jokers;
        return newSet;
    }


    public void Remove(Tile tile)
    {
        Tiles.Remove(tile);

        if (tile.IsJoker) _jokers--;
    }


    public void PrintAllTiles()
    {
        foreach (var tile in Tiles) tile.PrintTile();
    }

    private void Sort()
    {
        if (_isSorted) return;
        Tiles.Sort();
        _isSorted = true;
    }

    public Solution GetSolution()
    {
        Sort();

        if (_jokers > 0) Tiles.RemoveRange(Tiles.Count - _jokers, _jokers); //remove remove todo
        var length = Tiles.Count;
        _usedTiles = new bool[length];

        if (length + _jokers <= 2) return length == 0 ? new Solution() : Solution.GetInvalidSolution();

        var solution = FindSolution(new Solution(), 0);

        return solution.IsValid ? solution : Solution.GetInvalidSolution();
    }

    public Solution GetFirstSolution()
    {
        Sort();

        if (_jokers > 0) Tiles.RemoveRange(Tiles.Count - _jokers, _jokers);
        var length = Tiles.Count;

        if (length + _jokers <= 2) return length == 0 ? new Solution { IsValid = true } : Solution.GetInvalidSolution();

        _usedTiles = new bool[length];

        return FindFirstSolution(new Solution(), 0, 0);
    }

    private Solution FindFirstSolution(Solution solution, int solutionScore, int startIndex)
    {
        while (startIndex < _usedTiles.Length - 1)
        {
            startIndex = Array.FindIndex(_usedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TryFirstSet(GetRuns(startIndex), solution, solutionScore, startIndex,
                (sol, run) => sol.AddRun(run));

            if (solRun.IsValid) return solRun;

            var solGroup = TryFirstSet(GetGroups(startIndex), solution, solutionScore, startIndex,
                (sol, group) => sol.AddGroup(group));

            if (solGroup.IsValid) return solGroup;

            startIndex++;
        }

        return solution;
    }

    private Solution TryFirstSet<TS>(IEnumerable<TS> sets, Solution solution, int solutionScore, int firstUnusedIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        _usedTiles[firstUnusedIndex] = true;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, firstUnusedIndex);

            var newSolutionScore = solutionScore + set.GetScore();

            if (newSolutionScore >= 30) solution.IsValid = true;

            else solution = FindFirstSolution(solution, newSolutionScore, firstUnusedIndex);

            if (solution.IsValid)
            {
                addSetToSolution(solution, set);
                return solution;
            }

            MarkTilesAsUsed(set, false, firstUnusedIndex);
        }

        _usedTiles[firstUnusedIndex] = false;

        return solution;
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed, int firstUnusedIndex)
    {
        foreach (var tile in set.Tiles[0].IsJoker ? set.Tiles.Skip(2) : set.Tiles.Skip(1))
        {
            for (var i = firstUnusedIndex + 1; i < Tiles.Count; i++)
            {
                if (_usedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                _usedTiles[i] = isUsed;
                break;
            }
        }

        _jokers += isUsed ? -set.Jokers : set.Jokers;
    }


    private Solution FindSolution(Solution solution, int firstUnusedIndex)
    {
        firstUnusedIndex = Array.FindIndex(_usedTiles, firstUnusedIndex, used => !used);

        var solRun = TrySet(GetRuns(firstUnusedIndex), solution, firstUnusedIndex,
            (sol, run) => sol.AddRun(run));

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(firstUnusedIndex), solution, firstUnusedIndex,
            (sol, group) => sol.AddGroup(group));

        return solGroup.IsValid ? solGroup : Solution.GetInvalidSolution();
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int firstUnusedTileIndex,
        Action<Solution, TS> addSetToSolution)
        where TS : ValidSet
    {
        _usedTiles[firstUnusedTileIndex] = true;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, firstUnusedTileIndex);

            var newSolution = solution;
            switch (_usedTiles.Count(b => !b) + _jokers)
            {
                case 0:
                    solution.IsValid = true;
                    break;
                case > 2:
                    newSolution = FindSolution(solution, firstUnusedTileIndex);
                    break;
            }

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

    private IEnumerable<Run> GetRuns(int tileIndex)
    {
        var availableJokers = _jokers;
        var color = Tiles[tileIndex].Color;
        var currentRun = new List<Tile> { Tiles[tileIndex] };
        var i = tileIndex + 1;

        while (true)
        {
            for (; i < Tiles.Count; i++)
            {
                if (Tiles[i].Color != color)
                {
                    i = Tiles.Count;
                    break;
                }

                if (_usedTiles[i] || Tiles[i].Value == currentRun[^1].Value) continue;

                if (Tiles[i].Value != currentRun[^1].Value + 1) break;

                currentRun.Add(Tiles[i]);

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
        var sameNumberTiles = Tiles
            .Where((tile, index) => !_usedTiles[index] &&
                                    tile.Value == Tiles[firstTileIndex].Value &&
                                    tile.Color != Tiles[firstTileIndex].Color)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

        if (_jokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { Tiles[firstTileIndex] };

            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
            {
                yield return new Group
                {
                    Tiles = [Tiles[firstTileIndex], sameNumberTiles[i], sameNumberTiles[j]]
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

                    groupTiles[0] = Tiles[firstTileIndex];

                    for (var i = 0; i < tilesUsed - 1; i++) groupTiles[i + 1] = tiles[i];

                    for (var k = 0; k < jokersUsed; k++)
                        groupTiles[tilesUsed + k] = new Tile(Tiles[firstTileIndex].Value, isJoker: true);

                    yield return new Group
                    {
                        Tiles = groupTiles,
                        Jokers = jokersUsed
                    };
                }
            }
        }
    }

    public Set ShuffleTiles()
    {
        var random = new Random();
        var n = Tiles.Count;

        for (var i = n - 1; i > 0; i--)
        {
            var j = random.Next(0, i + 1);
            (Tiles[i], Tiles[j]) = (Tiles[j], Tiles[i]);
        }

        return this;
    }

    public static IEnumerable<Set> GetBestSets(List<Tile> tiles, int n)
    {
        var combinations = GetCombinations(tiles, n);

        return combinations
            .Select(combination => new Set(combination)
            )
            .OrderByDescending(t => t.GetScore());
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