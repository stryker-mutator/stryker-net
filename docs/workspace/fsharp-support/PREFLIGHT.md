# fsharp-support — Pre-flight gate

Copy this file to docs/workspace/fsharp-support/PREFLIGHT.md at Phase 4c, tick every box on disk (the link lint included), then commit the plan.

**Per task**:
- [x] Types it creates or modifies reference the domain model
- [x] Constraints are extracted from ADRs, invariants, and transformation rules
- [x] Tests are defined — named, with expected behavior described (these become the failing tests)
- [x] Verify command is copy-pasteable and exits 0 on success
- [x] Acceptance criteria are pass/fail with no subjective language
- [x] Time-box is set (split if > 90 min)
- [x] Dependencies on other tasks are declared
- [x] Each task's named test enters through the public surface ([`tdd`](../../tdd/SKILL.md)), or its Slice is `prefactoring`, `expand`, `migrate`, `contract`, or `whole — <reason>`
- [x] Every planned domain type appears in the requirement traceability table
- [x] Task is `downhill` — no uncertainty remains
- [x] `Fit: closed` — a fresh subagent on a cheaper model finishes the task from its text alone; an `open` task goes back to Phase 3 or 4a before autopilot starts

**Constitution completeness**:
- [x] Domain model covers every planned domain type in diagram and traceability table
- [x] Every type in the traceability table maps to at least one FR or NFR
- [x] Every traceability row has a stability value (`internal` or `published`); each `published` row has a compatibility rule
- [x] Every NFR has a measure and a verify command (or an explicit `manual: <reason>`)
- [x] Transformations table covers every function that enforces a domain rule
- [x] Every failure-modes row that yields a rule appears as an error-path invariant in the transformations table
- [x] Every DESIGN.md section is filled or marked `N/A: <reason>` — no blank sections
- [x] No constraint is ambiguous enough that two reasonable agents would interpret it differently
- [x] Link lint green — every file reference in workspace docs is a clickable link:
  `` ! git grep --untracked -nE '(^|[^[])`([[:alnum:]._-]+/)*[[:alnum:]._-]+\.md(:[0-9]+([-,:][0-9]+)?)?`' -- 'docs/workspace/fsharp-support/*.md' ``
  Run from the repository root; `--untracked` scans the workspace before it is staged, so nothing is added to the index; green when the command prints nothing (exit 1 means no match); in cmd use double quotes around the pattern.

**Autopilot readiness**:
- [x] Build, test, and lint commands pass (green baseline) — run them now and confirm
- [x] Known-failing tests are explicitly listed with their reason
- [x] Every session has a `Skills` field — verify each skill name exists
- [x] Session checkpoints are defined and ordered
- [x] Total estimated time fits within the target session window (2–4H)

If any item fails, fix it before proceeding.

**Plan commit**: once the pre-flight gate passes, commit all workspace artifacts (`docs/workspace/fsharp-support/`) at once, in every context, without asking; the human's "go" is for Phase 5, not for this commit. Message:

```
docs(fsharp-support): plan ready for autopilot

Phases 1–4 complete. DESIGN.md, ADRs, and TASKS.md pass pre-flight gate.
```

This checkpoint preserves the plan before implementation begins. The plan is the contract — it must be committed before any code changes.
