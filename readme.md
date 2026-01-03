PerfBenchmarks
--------------
A small .NET benchmark project using BenchmarkDotNet to measure performance of trading-related code paths.

Prerequisites
- .NET 10 SDK installed.
- Optional: Visual Studio or any .NET-capable IDE.

Project files
- [PerfBenchmarks.csproj](PerfBenchmarks.csproj)
- [Program.cs](Program.cs)
- [run.bat](run.bat)

Build
- Restore and build (Release recommended):
`dotnet build -c Release`

Run benchmarks
- From the repo root:
`dotnet run -c Release`
- Or on Windows:
`.\run.bat`

Benchmark output
- BenchmarkDotNet writes reports to `BenchmarkDotNet.Artifacts/results/`.
- Example report files:
  - [BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report.html](BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report.html)
  - [BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report-github.md](BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report-github.md)
  - [BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report.csv](BenchmarkDotNet.Artifacts/results/TradingBenchmarks-report.csv)
- Open the `.html` file in a browser for a formatted report.

Interpreting results
- Use the `.html` for human-friendly overview.
- Use the `.csv` for programmatic analysis or importing into spreadsheets.
- Use the `.md` as a quick summary suitable for GitHub.

Contributing
- Add small, deterministic benchmarks.
- Use `Release` builds for accurate measurements.
- Open issues or PRs for new benchmarks or fixes.

License
- No license file included. Add a `LICENSE` at the repo root if you want to specify terms.
