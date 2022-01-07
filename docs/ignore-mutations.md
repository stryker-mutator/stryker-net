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

It's also possible to filter mutants at the source code level using special comments. This filtering gives the most fine-grained level of control.

The syntax for the comments is: `Stryker [disable|restore][once][all| mutator list][: reason for disabling]`

`// Stryker disable all` Disables all mutants from that line on.

`// Stryker restore all` re-enables all mutants from that line on.

`// Stryker disable once all` will only disable mutants on the next line.

`// Stryker disable once Arithmetic,Update` will only disable Arithmetic and Update mutants on the next line

Example:

```csharp
var i = 0;
var y = 10;
// Stryker disable all : for explanatory reasons
i++; // won't be mutated
y++; // won't be mutated
// Stryker restore all
i--; // will be mutated
// Stryker disable once all
y--; // won't be mutated
i++; // will be mutated
// Stryker disable once Arithmetic
y++; // will be mutated
// Stryker disable once Arithmetic,Update
i--; // won't be mutated
```

_Note that this feature is scope aware. If you disable mutators inside a method, the scope will not leak outside the method, even if there is more code below._
