# Stryker Mutation Report Parsing Skill

## Purpose

This skill provides quick reference commands and patterns for parsing Stryker mutation testing reports from the terminal. Stryker outputs reports in JSON format following the [Mutation Testing Results Schema](https://raw.githubusercontent.com/stryker-mutator/mutation-testing-elements/refs/heads/master/packages/report-schema/src/mutation-testing-report-schema.json).

## Report Location

Stryker writes output to a timestamped directory under `StrykerOutput/`:

```
StrykerOutput/
  2026-06-07.14-30-00/
    reports/
      mutation-report.json    ← JSON report (parse this)
      mutation-report.html    ← HTML report (view in browser)
```

The latest report is in the most recently modified directory:

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
cat "$LATEST/reports/mutation-report.json"
```

## Schema Overview

The report JSON has this top-level structure:

```json
{
  "schemaVersion": "2.0.0",
  "thresholds": { "high": 80, "low": 60 },
  "files": {
    "<absolute-file-path>": {
      "language": "cs",
      "source": "...",
      "mutants": [ ... ]
    }
  },
  "testFiles": { ... },
  "projectRoot": "...",
  "performance": { "setup": 100, "initialRun": 500, "mutation": 8000 }
}
```

### Mutant Status Values

Each mutant has a `status` field with one of these values:

| Status | Meaning |
|---|---|
| `Killed` | Test caught the mutation |
| `Survived` | No test caught the mutation (bad) |
| `NoCoverage` | Not covered by any test |
| `CompileError` | Mutation broke compilation |
| `RuntimeError` | Mutation caused a runtime error |
| `Timeout` | Testing the mutant timed out |
| `Ignored` | Intentionally skipped |
| `Pending` | Marked as pending |

### Mutant Fields

Each mutant object contains:

```json
{
  "id": "0",
  "mutatorName": "EqualityOperator",
  "status": "Survived",
  "replacement": "<=",
  "location": { "start": { "line": 10, "column": 8 }, "end": { "line": 10, "column": 9 } },
  "killedBy": [ "TestClassName.TestMethod" ],
  "coveredBy": [ "TestClassName.TestMethod" ],
  "statusReason": "..."
}
```

## Quick Terminal Commands

### Summary of all mutant statuses

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
cat "$LATEST/reports/mutation-report.json" | python3 -c "
import sys, json
data = json.load(sys.stdin)
counts = {}
for f in data['files'].values():
    for m in f['mutants']:
        s = m['status']
        counts[s] = counts.get(s, 0) + 1
for status, count in sorted(counts.items()):
    print(f'{status:15s} {count}')
"
```

### Mutation score (same as Stryker CLI output)

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
cat "$LATEST/reports/mutation-report.json" | python3 -c "
import sys, json
data = json.load(sys.stdin)
total = killed = 0
for f in data['files'].values():
    for m in f['mutants']:
        if m['status'] in ('Killed', 'Survived'):
            total += 1
            if m['status'] == 'Killed':
                killed += 1
score = (killed / total * 100) if total else 0
print(f'Mutation score: {score:.1f}% ({killed}/{total})')
"
```

### List all survived mutants

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
cat "$LATEST/reports/mutation-report.json" | python3 -c "
import sys, json
data = json.load(sys.stdin)
for path, fdata in data['files'].items():
    for m in fdata['mutants']:
        if m['status'] == 'Survived':
            loc = m.get('location', {})
            start = loc.get('start', {})
            line = start.get('line', '?')
            print(f'{path}:{line}  {m[\"mutatorName\"]}  {m[\"replacement\"]}')
"
```

### Find which test killed a specific mutant

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
MUTANT_ID=0
cat "$LATEST/reports/mutation-report.json" | python3 -c "
import sys, json, sys
data = json.load(sys.stdin)
target = int(sys.argv[1])
for path, fdata in data['files'].items():
    for m in fdata['mutants']:
        if int(m['id']) == target:
            tests = m.get('killedBy', [])
            if tests:
                print(f'Mutant {target} killed by:')
                for t in tests:
                    print(f'  - {t}')
            else:
                print(f'Mutant {target} has no killedBy data')
" "$MUTANT_ID"
```

### Filter survived mutants by file or mutator

```bash
LATEST=$(ls -td StrykerOutput/*/ | head -1)
# Filter by file path (partial match)
grep -o '"files": {[^}]*}' "$LATEST/reports/mutation-report.json" | grep "KilledMutants"

# Or use python for structured filtering:
cat "$LATEST/reports/mutation-report.json" | python3 -c "
import sys, json
data = json.load(sys.stdin)
for path, fdata in data['files'].items():
    if 'KilledMutants' not in path:
        continue
    for m in fdata['mutants']:
        if m['status'] == 'Survived':
            loc = m.get('location', {})
            start = loc.get('start', {})
            line = start.get('line', '?')
            print(f'{path}:{line}  {m[\"mutatorName\"]}  → {m[\"replacement\"]}')
"
```

## Tips

- **Always use `ls -td`** to find the latest report — Stryker creates a new timestamped directory each run.
- **`killedBy`** may be empty if the framework only tracks `coveredBy` (common with MTP).
- **`statusReason`** contains the failure message for killed mutants or the error for compile/runtime errors.
- **`location.start.line`** is 1-indexed (per the schema).
- For large reports, pipe through `head` or `grep` to avoid flooding the terminal.
- The HTML report (`mutation-report.html`) is human-readable and can be opened in a browser for interactive exploration.
