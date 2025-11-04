# BlazorRummiSolve Tests

This directory contains comprehensive tests for all Rummikub solvers.

## Test Structure

### Test Files

- **AllSolversTests.cs** - Tests all standard solvers (with board) against all scenarios
    - 6 solvers × 18 scenarios = 108 tests
    - Solvers: CombinationsSolver, ParallelCombinationsSolver, BestScoreComplexSolver, IncrementalComplexSolver,
      IncrementalScoreFieldComplexSolver, IncrementalComplexSolverTileAndSc

- **AllFirstSolversTests.cs** - Tests all first-turn solvers (no board) against first-turn scenarios
    - 3 solvers × 7 scenarios = 21 tests
    - Solvers: CombinationsFirstSolver, BestScoreFirstBaseSolver, IncrementalFirstBaseSolver

- **CommonTestCases.cs** - 18 test scenarios for standard solvers
- **CommonTestFirstCases.cs** - 7 test scenarios for first-turn solvers
- **SolverTestHelpers.cs** - Shared validation logic (no code duplication)
- **DebugSingleScenarioTests.cs** - Helper tests for debugging specific scenarios

### Running Tests

```bash
# Run all tests
dotnet test BlazorRummiSolve.Tests/BlazorRummiSolve.Tests.csproj

# Run specific test class
dotnet test --filter "FullyQualifiedName~AllSolversTests"
dotnet test --filter "FullyQualifiedName~AllFirstSolversTests"

# Run with detailed output (shows diagnostic info for each test)
dotnet test --logger "console;verbosity=detailed"
```

## Test Output

Each test outputs diagnostic information including:

- ✓ Solver name and test name
- IsValid status
- Number of tiles to play
- Number of jokers to play
- Score
- Source solver
- List of tiles played (with colors: R=Red, B=Black, Bl=Blue, M=Mango, J=Joker)

### Example Output

```
✓ CombinationsSolver - Valid
  IsValid: True
  Tiles to play: 3
  Jokers to play: 0
  Score: 0
  Source: CombinationsSolver
  Tiles played: 10Bl, 10R, 10B
```

To see this output, run tests with verbose logging:

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Adding New Test Scenarios

### For Standard Solvers (with board)

Add to `CommonTestCases.cs`:

```csharp
new(
    "MyNewTest",
    new Set([  // Board
        new Tile(1, TileColor.Red),
        new Tile(2, TileColor.Red),
        new Tile(3, TileColor.Red)
    ]),
    new Set([  // Player
        new Tile(4, TileColor.Red)
    ]),
    new ExpectedResult(
        true,  // IsValid
        [  // TilesToPlay
            new Tile(4, TileColor.Red)
        ],
        0  // JokerToPlay
    )
)
```

### For First-Turn Solvers (no board)

Add to `CommonTestFirstCases.cs`:

```csharp
new(
    "MyNewFirstTest",
    new Set([  // Player only
        new Tile(10),
        new Tile(10, TileColor.Red),
        new Tile(10, TileColor.Black)
    ]),
    0,  // JokerCount
    new CommonTestCases.ExpectedResult(
        true,
        [
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ],
        0
    )
)
```

## Adding New Solvers

### For Standard Solvers

In `AllSolversTests.cs`, add to the `Solvers` array:

```csharp
("MyNewSolver", MyNewSolver.Create)
```

### For First-Turn Solvers

In `AllFirstSolversTests.cs`, add to the `Solvers` array:

```csharp
("MyNewFirstSolver", playerSet => MyNewFirstSolver.Create(playerSet))
```

## Debugging Tests

Use `DebugSingleScenarioTests.cs` to debug specific scenarios:

1. **DebugSpecificScenario()** - Test any scenario from CommonTestCases with any solver
2. **DebugCustomScenario()** - Create a custom scenario for specific testing

Simply set breakpoints and run the test in debug mode.

## Test Architecture

- **Order-insensitive comparison**: Tiles can be returned in any order
- **Multiplicity checking**: Validates correct number of each unique tile
- **Shared validation logic**: `SolverTestHelpers.AssertSolverResult()` used by all tests
- **No code duplication**: Same validation logic for standard and first-turn solvers
