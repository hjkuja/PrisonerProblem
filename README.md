# PrisonerProblem

Piece of code to test the [100 prisoner problem](https://en.wikipedia.org/wiki/100_prisoners_problem)

Got inspiration from [this Veritasium video](https://youtu.be/iSNsgj1OCLA)

## Prerequisites

- .NET 10 SDK (pinned via `global.json`)

## Run

```bash
dotnet run --project PrisonerProblem/PrisonerProblem.csproj -c Release
```

## Options

- `--times <int>`: how many runs to simulate (default `1500`)
- `--prisoners <int>`: number of prisoners; must be divisible by 2 (default `100`)
- `--shufflePrisoners <bool>`: shuffle prisoner order too (default `true`)
- `--cores <int>`: max CPU cores to use (default `1`, capped to `Environment.ProcessorCount`)
- `--quiet`: disables per-run logging (recommended for performance)

Example (share 1500 runs across up to 8 cores):

```bash
dotnet run --project PrisonerProblem/PrisonerProblem.csproj -c Release -- --times 1500 --cores 8 --quiet
```
