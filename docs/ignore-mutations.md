---
title: Ignore mutations
sidebar_position: 35
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/ignore-mutations.md
---
# Ignore mutations

There are several ways of ignoring mutations in Stryker.NET. By using these options together you have fine grained control over which mutation should be tested and which should be ignored.

## Ignore mutations option

Every occurrence of a specific mutation type can be ignored using the [ignore mutations](https://stryker-mutator.io/docs/stryker-net/configuration#ignore-mutations-string) option.

``` json
"stryker-config": {
    "ignore-mutations": [
        "string"
    ]
}
```

## Ignore methods option

Specific method calls can be ignored using the [ignore methods](https://stryker-mutator.io/docs/stryker-net/configuration#ignore-methods-string) option.

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

Whole files can be ignored using the [mutate](https://stryker-mutator.io/docs/stryker-net/configuration#mutate-glob) option.

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
will only mark mutations impacting `SomeMEthod(x*5)` as ignored, keeping every other mutations in the expression.
- while multi line comments cannot be used anywhere within the code, Stryker may not be able to identiy which part of the code they refer to.
Meaning you may get what you expect. This is due to how Stryker and the Roslyn compiler interact. That is why we recommend using single line comments.

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

