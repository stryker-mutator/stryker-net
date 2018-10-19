# Mutators
Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

<!-- TOC -->

- [Mutators](#mutators)
    - [Binary Operators](#binary-operators)
    - [Boolean Substitutions](#boolean-substitutions)
    - [Assignment Mutator](#assignment-mutator)
    - [Unary Operators](#unary-operators)
    - [Update Operators](#update-operators)
	- [Checked Mutator](#checked-mutator)
    - [LINQ Mutator](#linq-mutator)
    - [String Mutator](#string-mutator)
    - [Interpolated String Mutator](#interpolated-string-mutator)

<!-- /TOC -->

## Binary Operators
| Original | Mutated | 
| ------------- | ------------- | 
| `+` | `-` |
| `-` | `+` |
| `*` | `/` |
| `/` | `*` |
| `%` | `*` |
| `>` | `<` |
| `>` | `>=` |
| `>=` | `<` |
| `>=` | `>` |
| `<` | `>` |
| `<` | `<=` |
| `<=` | `>` |
| `<=` | `<` |
| `==` | `!=` |
| `!=` | `==` |
| `&&` | `\|\|`
| `\|\|` | `&&`

## Boolean Substitutions
| Original | Mutated | 
| ------------- | ------------- | 
| `true`	| `false` |
| `false`	| `true` |
| `!`		| ` ` |

## Assignment Mutator
| Original | Mutated | 
| ------------- | ------------- | 
|`+= `	| `-= ` |
|`-= `	| `+= ` |
|`*= `	| `/= ` |
|`/= `	| `*= ` |
|`%= `	| `*= ` |
|`<<=`  | `>>=` |
|`>>=`  | `<<=` |
|`&= `	| `\|= ` |
|`\|= `	| `&= ` |

## Unary Operators
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `-variable`	| `+variable`|
| `+variable` 	| `-variable`|
| `~variable` 	| `variable` |

## Update Operators
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `variable++`	| `variable--` |
| `variable--`	| `variable++` |
| `-variable`	| `+variable`|
| `+variable` 	| `-variable`|
| `~variable` 	| `variable` |
| `++variable`	| `--variable` |
| `--variable`	| `++variable` |

## Checked Mutator
| Original | Mutated |
| ------------- | ------------- | 
| `checked(2 + 4)` | `2 + 4` |

## LINQ Mutator
|      Original         |       Mutated         |
| --------------------- | --------------------- |
| `Distinct()`          | ` `                   |
| `Reverse()`           | ` `                   |
| `OrderBy()`           | ` `                   |
| `OrderByDescending()` | ` `                   |
| `SingleOrDefault()`   | `FirstOrDefault()`    |
| `FirstOrDefault()`    | `SingleOrDefault()`   |
| `First()`             | `Last()`              |
| `Last()`              | `First()`             |
| `All()`               | `Any()`               |
| `Any()`               | `All()`               |
| `Skip()`              | `Take()`              |
| `Take()`              | `Skip()`              |
| `SkipWhile()`         | `TakeWhile()`         |
| `TakeWhile()`         | `SkipWhile()`         |
| `Min()`               | `Max()`               |
| `Max()`               | `Min()`               |
| `Sum()`               | `Count()`             |
| `Count()`             | `Sum()`               |

## String Mutator
| Original | Mutated |
| ------------- | ------------- | 
| `"foo"` (non-empty string) | `""` (empty string) |
|  `""` (empty string) | `"Stryker was here!"` |

## Interpolated String Mutator
| Original | Mutated |
| ------------- | ------------- | 
| `$"foo {bar}"` (string interpolation) | `$""` |
