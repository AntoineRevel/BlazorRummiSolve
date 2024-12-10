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

        var solution = FindSolution(new Solution(), length + _jokers, 0);

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
        while (startIndex < _usedTiles.Length - 2)
        {
            startIndex = Array.FindIndex(_usedTiles, startIndex, used => !used);

            var solRun = TryFirstSet(GetRuns(startIndex), solution,
                solutionScore, startIndex);

            if (solRun.IsValid) return solRun;

            var solGroup = TryFirstSet(GetGroups(startIndex), solution,
                solutionScore,
                startIndex);

            if (solGroup.IsValid) return solGroup;

            startIndex++;
        }

        return solution;
    }

    private Solution TryFirstSet<TS>(IEnumerable<TS> sets, Solution solution,
        int solutionScore,
        int firstUnusedTileIndex)
        where TS : ValidSet
    {
        var usedTilesCopy = _usedTiles.ToArray();
        var availableJokersCopy = _jokers;

        foreach (var set in sets)
        {
            var newSolutionScore = solutionScore + set.GetScore();

            if (newSolutionScore >= 30)
                solution.IsValid = true;
            else
                solution = FindFirstSolution(solution, newSolutionScore, firstUnusedTileIndex);

            if (solution.IsValid)
            {
                switch (set)
                {
                    case Run run:
                        solution.AddRun(run);
                        break;
                    case Group group:
                        solution.AddGroup(group);
                        break;
                }

                return solution;
            }

            _usedTiles = usedTilesCopy;
            _jokers = availableJokersCopy;
        }

        return solution;
    }

    private Solution FindSolution(Solution solution, int unusedTileCount, int firstUnusedTileIndex)
    {
        firstUnusedTileIndex = Array.FindIndex(_usedTiles, firstUnusedTileIndex, used => !used);

        var solRun = TrySet(GetRuns(firstUnusedTileIndex), solution, unusedTileCount, firstUnusedTileIndex);

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(firstUnusedTileIndex), solution, unusedTileCount, firstUnusedTileIndex);

        return solGroup.IsValid ? solGroup : Solution.GetInvalidSolution();
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int unusedTileCount, int firstUnusedTileIndex)
        where TS : ValidSet
    {
        _usedTiles[firstUnusedTileIndex] = true;
        var usedTilesCopy = _usedTiles.ToArray();
        var availableJokersCopy = _jokers;
        foreach (var set in sets)
        {
            unusedTileCount -= set.Tiles.Length;

            var newSolution = solution;
            switch (unusedTileCount)
            {
                case 0:
                    solution.IsValid = true;
                    break;
                case > 2:
                    newSolution = FindSolution(solution, unusedTileCount, firstUnusedTileIndex);
                    break;
            }

            if (newSolution.IsValid)
            {
                switch (set)
                {
                    case Run run:
                        solution.AddRun(run);
                        break;
                    case Group group:
                        solution.AddGroup(group);
                        break;
                }

                return solution;
            }

            _usedTiles = usedTilesCopy.ToArray();
            _jokers = availableJokersCopy;
            unusedTileCount += set.Tiles.Length;
        }

        _usedTiles[firstUnusedTileIndex] = false;

        return solution;
    }


    private IEnumerable<Run> GetRuns(int tileIndex)
    {
        var availableJokers = _jokers;
        var usedTilesCopy = _usedTiles.ToArray();
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
                usedTilesCopy[i] = true;

                if (currentRun.Count < 3) continue;
                _usedTiles = usedTilesCopy;
                var jokersUsed = _jokers - availableJokers;
                _jokers = availableJokers;
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

            _usedTiles = usedTilesCopy;
            var jokersUsedJ = _jokers - availableJokers;
            _jokers = availableJokers;
            yield return new Run
            {
                Tiles = [..currentRun],
                Jokers = jokersUsedJ
            };
        }
    }

    private IEnumerable<Group> GetGroups(int firstTileIndex)
    {
        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Value;
        var color = firstTile.Color;

        var sameNumberTiles = Tiles
            .Select((tile, index) => new { Tile = tile, Index = index })
            .Where(tuple => !_usedTiles[tuple.Index] && tuple.Tile.Value == number && tuple.Tile.Color != color)
            .DistinctBy(tuple => tuple.Tile.Color)
            .ToList();

        var size = sameNumberTiles.Count;

        if (_jokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { firstTile };
            foreach (var index in sameNumberTiles.Select(t => t.Index))
            {
                _usedTiles[index] = true;
            }

            groupTiles.AddRange(sameNumberTiles.Select(t => t.Tile));

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
            {
                _usedTiles[sameNumberTiles[i].Index] = true;
                _usedTiles[sameNumberTiles[j].Index] = true;

                yield return new Group
                {
                    Tiles = [firstTile, sameNumberTiles[i].Tile, sameNumberTiles[j].Tile]
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

                foreach (var comboTuple in combinations)
                {
                    var groupTiles = new Tile[totalTiles];
                    groupTiles[0] = firstTile;

                    for (var i = 0; i < tilesUsed - 1; i++)
                    {
                        groupTiles[i + 1] = comboTuple[i].Tile;
                        _usedTiles[comboTuple[i].Index] = true;
                    }

                    for (var k = 0; k < jokersUsed; k++) groupTiles[tilesUsed + k] = new Tile(number, isJoker: true);

                    _jokers -= jokersUsed;
                    
                    yield return new Group
                    {
                        Tiles = groupTiles,
                        Jokers = jokersUsed
                    };
                }
            }
        }
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed)
    {
        foreach (var tile in set.Tiles)
        {
            if (tile.IsJoker)
            {
                _jokers += isUsed ? -1 : 1;
                continue;
            }

            for (var i = 0; i < Tiles.Count; i++)
            {
                if (_usedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                _usedTiles[i] = isUsed;
                break;
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

    private static IEnumerable<List<T>> GetCombinations<T>(List<T> list, int length)
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