# MtpParallelAttributionMini

In-repo MTP fixture for [stryker-net#3832](https://github.com/stryker-mutator/stryker-net/issues/3832). Layout mirrors the published [stryker-3832-repro](https://github.com/dclark-olm/stryker-3832-repro) (multi-file document builder, **two test classes**, ~22 tests) without vendoring that repository.

## Layout

- **Library** (`MtpParallelAttributionMini.csproj`): `MiniReport.Build`, `MiniEscape`, builders (`MiniPageBuilder`, `MiniTableBuilder`, `Preamble`, …). Ported from the upstream repro sources (see `THIRD_PARTY_NOTICES.md`); dates use `DateOnly` instead of NodaTime.
- **Tests** (`MtpParallelAttributionMini.Tests/`): `MiniDocumentTests` (integration facts) and `MiniEscapeTests` (`[Theory]` escape rules). xUnit v3 + Microsoft.Testing.Platform (`OutputType` Exe).
- **Stryker:** run from the test project directory with `stryker-config.json` and `MtpParallelAttributionMini.slnx` (`test-runner`: `mtp`, default mutation level).

## Validation

`Stryker3832FastTests` (`Category=Stryker3832Fast`) in `integrationtest/Validation/ValidationProject/` compares kill counts and covered verdicts (`Killed + Survived`) at concurrency `1` vs `8`. A second test runs `stryker-config.isolated.json` (`perTestInIsolation`) and expects the same kill count as the default mode; `StaticOnlyHelper` (a value built only by a static initializer, tested by `StaticOnlyTests`) is what tells the two apart.

Additional configs `stryker-config.all.json` and `stryker-config.off.json` only assert **concurrency parity** (1 vs 8), not equality with `perTest`. In those modes MTP does not warm hosts or track static initializer usage, so the three string mutants in `StaticOnlyHelper.cs` typically **survive** while `perTest` kills them. See `docs/configuration.md` (Microsoft Test Platform / `coverage-analysis`).

## Gate status

A full run tests 323 mutants (67 killed, 256 survived) in about 40 s at concurrency 1 and about 25 s at concurrency 8, with 0 timeouts; the fast test asserts zero timeouts so a slow test session cannot pass as "killed".