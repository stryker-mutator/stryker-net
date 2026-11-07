---
status: accepted
---
# FCS dependency and packaging

Addresses: [NFR1](../designs/fsharp-support.md#nfr1), [NFR2](../designs/fsharp-support.md#nfr2)

## Problem
F# support was deleted because the pinned FCS preview dragged in a vulnerable
`System.Drawing.Common` (#3118), flagging every Stryker install. Hard to reverse: packaging
is a public contract (nuget layout), and a second scanner incident would end F# support
for good upstream.

## Options
| Option | Pros | Cons |
|---|---|---|
| A. Latest stable FCS inside Stryker.Core | simplest; one package | every C#-only user downloads FCS (~60 MB); any future FCS CVE flags everyone again — the exact #3118 failure mode |
| B. Separate `Stryker.Fsharp` assembly+package, loaded only for `.fsproj` sources | C# users never restore FCS; a future FCS CVE flags only F# users; answers the maintainers' removal reason directly | plugin-style loading seam to build; two packages to ship |
| C. No FCS: regex/line-based mutation | zero dependency | cannot find expression ranges reliably; dead end |

## Decision
Option B. Pin the latest stable FCS (43.9.x line) via central package management with a
lock file; CI gates on `dotnet list package --vulnerable --include-transitive`. The
F# pipeline lives in its own project referenced only through the existing DI seams, so the
load is conditional on project language.

## Consequences
Easier: upstream acceptance (removal reason is answered), scanner hygiene, independent FCS
upgrades. Harder: assembly-loading/DI wiring across a package boundary; release pipeline
ships one more package. Verify command is NFR1's.
