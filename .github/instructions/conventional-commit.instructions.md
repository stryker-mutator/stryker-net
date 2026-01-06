---
description: 'Prompt and workflow for generating conventional commit messages and pull request titles using a structured XML format. Guides users to create standardized, descriptive commit messages and PR titles in line with the Conventional Commits specification, including instructions, examples, and validation.'
tools: ['run_in_terminal', 'get_terminal_output']
---

### Instructions

```xml
	<description>This file contains a prompt template for generating conventional commit messages and pull request titles. It provides instructions, examples, and formatting guidelines to help users write standardized, descriptive commit messages and PR titles in accordance with the Conventional Commits specification.</description>
	<note>
```

### Workflow

**Follow these steps:**

1. Run `git status` to review changed files.
2. Run `git diff` or `git diff --cached` to inspect changes.
3. Stage your changes with `git add <file>`.
4. Construct your commit message using the following XML structure.
5. After generating your commit message, Copilot will automatically run the following command in your integrated terminal (no confirmation needed):

```bash
git commit -m "type(scope): description"
```

6. Just execute this prompt and Copilot will handle the commit for you in the terminal.

### Commit Message Structure

```xml
<commit-message>
	<type>feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert</type>
	<scope>()</scope>
	<description>A short, imperative summary of the change</description>
	<body>(optional: more detailed explanation)</body>
	<footer>(optional: e.g. BREAKING CHANGE: details, or issue references)</footer>
</commit-message>
```

### Examples

```xml
<examples>
	<example>feat(parser): add ability to parse arrays</example>
	<example>fix(ui): correct button alignment</example>
	<example>docs: update README with usage instructions</example>
	<example>refactor: improve performance of data processing</example>
	<example>chore: update dependencies</example>
	<example>feat!: send email on registration (BREAKING CHANGE: email service required)</example>
</examples>
```

### Validation

```xml
<validation>
	<type>Must be one of the allowed types. See <reference>https://www.conventionalcommits.org/en/v1.0.0/#specification</reference></type>
	<scope>Optional, but recommended for clarity.</scope>
	<description>Required. Use the imperative mood (e.g., "add", not "added").</description>
	<body>Optional. Use for additional context.</body>
	<footer>Use for breaking changes or issue references.</footer>
</validation>
```

### Final Step

```xml
<final-step>
	<cmd>git commit -m "type(scope): description"</cmd>
	<note>Replace with your constructed message. Include body and footer if needed.</note>
</final-step>
```

## Pull Request Title Guidelines

**IMPORTANT**: When creating or updating pull requests in this repository, always format the PR title using the same Conventional Commits specification.

### PR Title Format

```xml
<pr-title>
	<format>type(scope): description</format>
	<type>feat|fix|docs|style|refactor|perf|test|build|ci|chore|revert</type>
	<scope>Optional but recommended - the affected area/component</scope>
	<description>Short, imperative summary (present tense)</description>
</pr-title>
```

### Why PR Titles Matter

This repository uses **squash merging**, which means:
- The PR title becomes the commit message in the main branch
- Properly formatted PR titles ensure a clean, consistent git history
- The changelog is automatically generated from these commit messages

### PR Title Examples

```xml
<pr-title-examples>
	<example>feat(mutators): add string mutator support</example>
	<example>fix(cli): resolve configuration parsing issue</example>
	<example>docs: update contributor guidelines</example>
	<example>refactor(core): improve mutation orchestrator performance</example>
	<example>test(integration): add tests for VB.NET projects</example>
	<example>ci: update Azure pipeline configuration</example>
	<example>feat(reporters)!: add JSON reporter (BREAKING CHANGE)</example>
</pr-title-examples>
```

### PR Title Validation Rules

- Use imperative mood: "add feature" not "added feature" or "adds feature"
- Keep it concise but descriptive
- No period at the end
- If breaking change, add `!` after type or include BREAKING CHANGE in PR description
- Type must be one of the allowed types listed above
