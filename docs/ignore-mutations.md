---
title: Ignore mutations
sidebar_position: 35
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/ignore-mutations.md
---
# Ignore mutations

There are several ways of ignoring mutations in Stryker.NET. By using these options together you have fine grained control over which mutation should be tested and which should be ignored.

## Ignore mutations option

Every occurrence of a specific mutation type can be ignored using the [ignore mutations](./configuration.md#ignore-mutations-string) option.

``` json
"stryker-config": {
    "ignore-mutations": [
        "string"
    ]
}
```

## Ignore methods option

Specific method calls can be ignored using the [ignore methods](./configuration.md#ignore-methods-string) option.

``` json
"stryker-config": {
    "ignore-methods": [
        "*Log", // Ignores all methods ending with Log
        "Console.Write*", // Ignores all methods starting with Write in the class Console
        "*Exception.ctor" // Ignores all exception constructors
    ]
}
```

_Note that this only ignores mutation inside the method calls, not the method declaration._

## Mutate option

Whole files can be ignored using the [mutate](./configuration.md#mutate-glob) option.

``` json
"stryker-config": {
    "mutate": ["!**/*.Generated.cs"]
}
```

Or on command line: `-m "!**/*.Generated.cs"`

## Stryker comments

### Overview
It's also possible to filter mutants at the source code level using special comments. This filtering gives the most fine-grained level of control.

The syntax for the comments is: `Stryker [disable|restore][once][all| mutator list][: reason for disabling]`

`// Stryker disable all` Marks all mutants as ignored from that line on.

`// Stryker restore all` re-enables all mutants from that line on.

`// Stryker disable once all` will mark mutants as ignored for next line only.

`// Stryker disable all: we do not want to test this` will mark mutations on the next line as ignored with comment: 'we do not want to test this'.

`// Stryker disable once Arithmetic,Update` will only disable Arithmetic and Update mutants on the next line.

For the complete list of the types of mutators you may ignore, check out the [mutations](./mutations.md) page. For each mutator, use the name in parentheses. For example: to ignore `Boolean Literals (boolean)`, comment `// Stryker disable once Boolean`.

### Why ignore specific mutations?

Reasons for ignoring some mutations:

- The mutation always causes a timeout, ignoring it will speed up Stryker in future runs
- The mutation is invalid in this specific case
- The mutation can't be killed by any reasonable unit test

Be carefull to not ignore too many mutations, the goal isn't to reach 100% score, but to improve your actual coverage.

### Technical considerations
**Format and placement**:
Stryker supports both single (//) and multi-line (/\* \*/) comments. We recommend using single-line comments on dedicated lines for clarity and scope control.
You should use /\* \*/ format only when constrained so by code style rules.
**Scope and limitations**:
- 'once` means that ALL mutations within the next SYNTAX CONSTRUCT will be mark as ignored. This usually means the next statement.
But if used at the beginning of a block, it will apply to ALL MUTATIONS WITHIN THAT BLOCK. Same for a whole method or even a class.
- 'once' can also be used within an expression if you want fine grained control. Eg
```csharp
x = x + 
// Stryker disable once Arithmetic
SomeMethod(x*5)
* SomeOtherMethod(x/2);
```
will only mark mutations impacting `SomeMethod(x*5)` as ignored, keeping every other mutations in the expression.
- while multi line comments can be used anywhere within the code, Stryker may not be able to identify which part of the code they refer to.
Meaning you may get what you expect. This is due to how Stryker and the Roslyn compiler interact. That is why we recommend using single line comments.

### Automatically ignore timeouts

Mutants that cause a timeout make a mutation run a lot slower. Instead of adding comments by hand, you can let Stryker do it with the [auto-ignore-timeouts](./configuration.md#auto-ignore-timeouts-flag) flag:

```shell
dotnet stryker --auto-ignore-timeouts
```

When the run is complete, Stryker adds a Stryker comment for every mutant with the status `Timeout`:

```csharp
// Stryker disable once Arithmetic: Auto-ignored by Stryker (--auto-ignore-timeouts), the mutation caused a timeout
while (i < Compute(i + 1))
{
    ...
}
```

How it works:

- Comments are added using the syntax tree, directly above the closest statement or member declaration containing the mutation. This guarantees the comment is placed on a valid location and keeps existing comments and formatting intact.
- Only the mutator types that timed out are disabled (`once` applies to the whole statement, so other mutants of the same types in that statement are ignored too).
- After the files are changed, Stryker builds the project to verify the result. If the build fails, the files are restored to their original content and an error is logged.
- Files that changed on disk since the analysis are not touched.
- The next run skips these mutants, and they show up as `Ignored` in the report.

Things to keep in mind:

- A timeout is not always caused by the mutation. A heavily loaded machine can also cause timeouts. Review the added comments (for example in a pull request) and remove those that hide a mutant you do want to test.
- Ignoring a timeout hides a mutant that probably would have been detected. Fix the underlying issue when possible, for example by making the test suite faster or the loop condition testable.
- Remove a comment, or use `// Stryker restore`, to test the mutant again.

### Examples

```csharp
var i = 0;
var y = 10;
// Stryker disable all : for explanatory reasons
i++; // won't be mutated
y++; // won't be mutated
// Stryker restore all
i--; // will be mutated
i++; // will be mutated
// Stryker disable once Arithmetic
y++; // will be mutated
// Stryker disable once Arithmetic,Update
i--; // won't be mutated
```
