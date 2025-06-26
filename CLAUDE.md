# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

BlazorRummiSolve is a Rummikub game solver and visualizer consisting of three main projects:

- **BlazorRummiSolve**: Blazor web application for visualizing game states and solutions
- **RummiSolve**: Core game engine with multiple solver algorithms and strategies  
- **BlazorRummiSolve.Tests**: XUnit test suite for solver algorithms

## Common Commands

### Build and Run
```bash
# Build entire solution
dotnet build BlazorRummiSolve.sln

# Run Blazor web app
dotnet run --project BlazorRummiSolve/BlazorRummiSolve.csproj

# Run console solver benchmark
dotnet run --project RummiSolve/RummiSolve/RummiSolve.csproj
```

### Testing
```bash
# Run all tests
dotnet test BlazorRummiSolve.Tests/BlazorRummiSolve.Tests.csproj

# Run specific test class
dotnet test --filter "ClassName=BestScoreComplexSolverTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Architecture

### Core Game Engine (RummiSolve)
The game follows a layered solver architecture:

1. **Game/Player Layer**: `Game.cs` orchestrates turns, `Player.cs` uses strategies to solve
2. **Strategy Layer**: `ISolverStrategy` implementations coordinate multiple solvers
   - `ParallelSolverStrategy`: Runs combination and incremental solvers in parallel
   - `MeasuredSolverStrategy`: Tracks performance metrics
3. **Solver Layer**: Multiple algorithm implementations inheriting from `BaseSolver`
   - **Combinations**: `CombinationsSolver`, `BinaryBaseSolver` - enumerate valid tile combinations
   - **Incremental**: `IncrementalComplexSolver` - build solutions incrementally 
   - **Score-based**: `BestScoreComplexSolver` - optimize for best scoring moves

### Solver Hierarchy
- `ISolver` interface defines `SearchSolution()` contract
- `BaseSolver` provides tile management utilities (runs, groups, joker handling)
- `ComplexSolver` adds scoring and validation logic
- Concrete solvers implement specific algorithms (binary search, combinations, incremental)

### Blazor Frontend (BlazorRummiSolve)
- `GamePage.razor/.cs`: Main game interface with step-through visualization
- Component architecture: `TileComponent`, `SetComponent`, `SolutionComponent`
- Uses keyboard handling (Enter key) for game progression
- References RummiSolve project for game logic

### Key Domain Objects
- `Tile`: Game pieces with value, color, joker state
- `Set`: Collection of tiles (player rack or board sets)
- `Solution`: Complete board solution with validation
- `ValidSet`: Represents valid runs/groups for scoring
- `SolverResult`: Contains solution and performance metrics

## Development Notes

### Solver Strategy Pattern
New solvers should implement `ISolver` interface. The strategy pattern allows mixing different algorithms - parallel execution picks the fastest result.

### Testing Structure
Tests are organized by solver type in `BlazorRummiSolve.Tests/Solver/` with comprehensive coverage of edge cases and performance scenarios.

### Performance Considerations
The codebase uses BenchmarkDotNet for performance analysis. Solvers compete on speed, with timeout mechanisms for complex scenarios.