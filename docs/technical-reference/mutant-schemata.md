---
title: Mutant schemata
sidebar_position: 30
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/technical-reference/mutant-schemata.md
---

Stryker.NET chose to work with mutant schemata. This created a number of challenges.

## Compile errors
Some mutations result in compile errors like the one below.

``` csharp
if (Environment.GetEnvironmentVariable("ActiveMutation") == "1") {
  return "hello " - "world"; // mutated code
} else {
  return "hello " + "world"; // original code
}
```

We chose to accept the fact that not all mutations can be compiled. So mutators don't have to take compile errors in account. This keeps the mutators as simple as possible.

The framework itself should handle the compile errors. 

This is done by rollbacking all mutations that result in compile errors. The mutant that linked to that piece of code gets the status `builderror`.

`compile` → `remove compile error codes` → `compile 2nd time`

Sometimes not all errors are returned by the compiler at the first try. That's why we repeat this process until we have compiling code. Usually 1-3 retries are needed. With Roslyn's incremental compilation these retries are fast.

## Scope
The scope of some variables can change by placing it inside an if statement. This results in compile errors.

``` csharp
if (Environment.GetEnvironmentVariable("ActiveMutation") == "1") {
  int i = 0; // mutated code
} else {
  int i = 99; // original code
}
return i;
```

This kind of errors can't be rollbacked because the location of the diagnostic error will be the return statement. The location of the actual code that causes the error will be somewhere else.

This can be solved by using conditional statements instead of if statements.

``` csharp
int i = Environment.GetEnvironmentVariable("ActiveMutation") == "1" ? 0 : 99;
return i;
```

What kind of placement should be used depends on the type of SyntaxNode the mutation is made in. There are some rules build into Stryker.net when to choose an if-statement and when to use a conditional statement.

## Constant values
A drawback of mutant schemata is that Stryker.NET cannot mutate constant values. 

For example:
``` cs
public enum Numbers
{
    One = 1,
    Two = (One + 1)
}
```

would be mutated into

``` cs
public enum Numbers
{
    One = 1,
    Two = (MutantControl.IsActive(0) ? (One - 1) : (One + 1))
}
```

This cannot compile since `MutantControl.IsActive(0)` is not a constant value. That is why we skip constant values from mutating.

We are researching ways to overcome this issue but have not yet found a way to do this.
