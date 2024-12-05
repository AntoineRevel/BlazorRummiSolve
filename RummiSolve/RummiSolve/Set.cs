namespace RummiSolve;

public class Set : ISet
{
    private bool _isSorted;
    private int _jokers; //Joker to => ? TODO
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

        if (_jokers > 0) Tiles.RemoveRange(Tiles.Count - _jokers, _jokers);
        var length = Tiles.Count;
        _usedTiles = new bool[length];

        if (length + _jokers <= 2) return length == 0 ? new Solution() : Solution.GetInvalidSolution();

        var solution = FindSolution(new Solution(), length, 0, _jokers);

        return solution.IsValid ? solution : Solution.GetInvalidSolution();
    }

    public Solution GetFirstSolution()
    {
        Sort();

        if (_jokers > 0) Tiles.RemoveRange(Tiles.Count - _jokers, _jokers);
        var length = Tiles.Count;

        if (length + _jokers <= 2) return length == 0 ? new Solution { IsValid = true } : Solution.GetInvalidSolution();

        _usedTiles = new bool[length];

        return FindFirstSolution(new Solution(), 0, 0, _jokers);
    }

    private Solution FindFirstSolution(Solution solution, int solutionScore, int startIndex,
        int availableJokers)
    {
        while (startIndex < _usedTiles.Length - 2)
        {
            startIndex = Array.FindIndex(_usedTiles, startIndex, used => !used);

            if (startIndex == -1) return solution;

            var solRun = TryFirstSet(GetRuns(startIndex, availableJokers), solution,
                solutionScore, startIndex,
                availableJokers);

            if (solRun.IsValid) return solRun;

            var solGroup = TryFirstSet(GetGroups(startIndex, availableJokers), solution,
                solutionScore,
                startIndex, availableJokers);

            if (solGroup.IsValid) return solGroup;

            startIndex++;
        }

        return solution;
    }

    private Solution TryFirstSet<TS>(IEnumerable<TS> sets, Solution solution,
        int solutionScore,
        int firstUnusedTileIndex, int availableJokers)
        where TS : ValidSet
    {
        var toDeleteMark = 0;
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, ref toDeleteMark, ref availableJokers);

            var newSolutionScore = solutionScore + set.GetScore();

            if (newSolutionScore >= 30)
                solution.IsValid = true;
            else
                solution = FindFirstSolution(solution, newSolutionScore, firstUnusedTileIndex,
                    availableJokers);

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


            MarkTilesAsUsed(set, false, ref toDeleteMark, ref availableJokers);
        }

        return solution;
    }
    
    private Solution FindSolution(Solution solution, int unusedTileCount, int firstUnusedTileIndex,
        int availableJokers)
    {
        firstUnusedTileIndex = Array.FindIndex(_usedTiles, firstUnusedTileIndex, used => !used);

        var solRun = TrySet(GetRuns(firstUnusedTileIndex, availableJokers),
            solution, unusedTileCount, firstUnusedTileIndex, availableJokers);

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(firstUnusedTileIndex, availableJokers),
            solution, unusedTileCount, firstUnusedTileIndex, availableJokers);

        return solGroup.IsValid ? solGroup : Solution.GetInvalidSolution();
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, int unusedTileCount,
        int firstUnusedTileIndex, int availableJokers)
        where TS : ValidSet
    {
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, ref unusedTileCount, ref availableJokers);

            var newSolution = solution;
            switch (unusedTileCount)
            {
                case 0 when availableJokers == 0:
                    solution.IsValid = true;
                    break;
                case > 2:
                    newSolution = FindSolution(solution, unusedTileCount, firstUnusedTileIndex,
                        availableJokers);
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

            MarkTilesAsUsed(set, false, ref unusedTileCount, ref availableJokers);
        }

        return solution;
    }


    private IEnumerable<Run> GetRuns(int tileIndex, int availableJokers) // why availablejoker and not _jokers
    {
        var color = Tiles[tileIndex].Color;
        var currentRun = new List<Tile> { Tiles[tileIndex] };
        var currentRunIndices = new List<int> { tileIndex };
        var jokersUsed = 0;
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
                currentRunIndices.Add(i);

                if (currentRun.Count >= 3)
                    yield return new Run
                    {
                        Tiles = [..currentRun],
                        Jokers = jokersUsed
                    };
            }

            if (availableJokers <= 0) yield break;

            if (currentRun[^1].Value != 13) currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));
            else if (currentRun[0].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));
            else jokersUsed--; //insert 2 Run 123J 4...TODO

            availableJokers -= 1;
            jokersUsed++;

            if (currentRun.Count >= 3)
                yield return new Run
                {
                    Tiles = [..currentRun],
                    Jokers = jokersUsed
                };
        }
    }

    private IEnumerable<Group> GetGroups(int firstTileIndex, int availableJokers)
    {
        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Value;
        var color = firstTile.Color;

        var sameNumberTiles = Tiles
            .Where((tile, index) => !_usedTiles[index] && tile.Value == number && tile.Color != color)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

        if (availableJokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { firstTile };
            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            for (var j = i + 1; j < sameNumberTiles.Count; j++)
                yield return new Group
                {
                    Tiles = [firstTile, sameNumberTiles[i], sameNumberTiles[j]]
                };
        }
        else
        {
            for (var jokersUsed = 0; jokersUsed <= availableJokers; jokersUsed++)
            for (var tilesUsed = 2; tilesUsed <= size + 1; tilesUsed++)
            {
                var totalTiles = tilesUsed + jokersUsed;
                if (totalTiles is < 3 or > 4) continue;

                var combinations = GetCombinations(sameNumberTiles, tilesUsed - 1);

                foreach (var combo in combinations)
                {
                    var groupTiles = new Tile[totalTiles];
                    groupTiles[0] = firstTile;

                    combo.CopyTo(0, groupTiles, 1, tilesUsed - 1);

                    for (var k = 0; k < jokersUsed; k++)
                        groupTiles[tilesUsed + k] = new Tile(number, isJoker: true);

                    yield return new Group
                    {
                        Tiles = groupTiles,
                        Jokers = jokersUsed
                    };
                }
            }
        }
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed, ref int unusedTile,
        ref int availableJokers)
    {
        foreach (var tile in set.Tiles)
        {
            if (tile.IsJoker)
            {
                availableJokers += isUsed ? -1 : 1;
                continue;
            }

            for (var i = 0; i < Tiles.Count; i++)
            {
                if (_usedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                _usedTiles[i] = isUsed;
                unusedTile += isUsed ? -1 : 1;
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