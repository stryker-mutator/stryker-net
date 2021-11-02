---
title: Mutators
sidebar_position: 40
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/mutators.md
---

Stryker supports a variety of mutators, which are listed below. In parentheses the names of correspondent mutations are specified, which you might need for the `exclude-mutations` section of the configuration.

Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

## Arithmetic Operators (_arithmetic_)
| Original | Mutated | 
| ------------- | ------------- | 
| `+` | `-` |
| `-` | `+` |
| `*` | `/` |
| `/` | `*` |
| `%` | `*` |

## Equality Operators (_equality_)
| Original | Mutated | 
| ------------- | ------------- |
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

## Logical Operators (_logical_)
| Original | Mutated | 
| ------------- | ------------- | 
| `&&` | `\|\|` | 
| `\|\|` | `&&` |
| `^` | `==` |

## Boolean Literals (_boolean_)
| Original | Mutated | 
| ------------- | ------------- | 
| `true`	| `false` |
| `false`	| `true` |
| `!person.IsAdult()`		| `person.IsAdult()` |
| `if(person.IsAdult())` | `if(!person.IsAdult())` |
| `while(person.IsAdult())` | `while(!person.IsAdult())` |

## Assignment Statements (_assignment_)
| Original | Mutated | 
| ------------- | ------------- | 
|`+= ` | `-= ` |
|`-= ` | `+= ` |
|`*= ` | `/= ` |
|`/= ` | `*= ` |
|`%= ` | `*= ` |
|`<<=` | `>>=` |
|`>>=` | `<<=` |
|`&= ` | `\|= `|
|`&= ` | `^= ` |
|`\|= `| `&= ` |
|`\|= `| `^= ` |
|`^= ` | `\|= `|
|`^= ` | `&= ` |

## Collection initialization (_initializer_)
| Original | Mutated | 
| ---------------------------------------- | ------------------------------------ |
|`new int[] { 1, 2 };`                     | `new int[] { };`                     |
|`int[] numbers = { 1, 2 };`               | `int[] numbers = { };`               |
|`new List<int> { 1, 2 };`                 | `new List<int> { };`                 |
|`new Collection<int> { 1, 2 };`           | `new Collection<int> { };`           |
|`new Dictionary<int, int> { { 1, 1 } };`  | `new Dictionary<int, int> { };`      |

## Removal mutators (_statement_, _block_)
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `void Function() { Age++; }`	| `void Function() {} (block emptied)`|
| `int Function() { Age++; return Age; }` 	| `void Function() { return default; } (block emptied)`|
| `return;` 	| `removed` |
| `return value;` 	| `removed` |
| `break;` 	| `removed` |
| `continue;` 	| `removed` |
| `goto;` 	| `removed` |
| `throw;` 	| `removed` |
| `throw exception;` 	| `removed` |
| `yield return value;` 	| `removed` |
| `yield break;` 	| `removed` |
| `MyMethodCall();` 	| `removed` |

## Unary Operators (_unary_)
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `-variable`	| `+variable`|
| `+variable` 	| `-variable`|
| `~variable` 	| `variable` |

## Update Operators (_update_)
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `variable++`	| `variable--` |
| `variable--`	| `variable++` |
| `++variable`	| `--variable` |
| `--variable`	| `++variable` |

## Checked Statements (_checked_)
| Original | Mutated |
| ------------- | ------------- | 
| `checked(2 + 4)` | `2 + 4` |

## Linq Methods (_linq_)
|      Original         |       Mutated         |
| --------------------- | --------------------- |
| `SingleOrDefault()`  | `Single()`             |
| `Single()`           | `SingleOrDefault()`    |
| `FirstOrDefault()`   | `First()`              |
| `First()`             | `FirstOrDefault()`    |
| `Last()`              | `First()`             |
| `All()`               | `Any()`               |
| `Any()`               | `All()`               |
| `Skip()`              | `Take()`              |
| `Take()`              | `Skip()`              |
| `SkipWhile()`        | `TakeWhile()`        |
| `TakeWhile()`        | `SkipWhile()`        |
| `Min()`               | `Max()`               |
| `Max()`               | `Min()`               |
| `Sum()`               | `Max()`               |
| `Count()`             | `Sum()`               |
| `Average()`           | `Min()`               |
| `OrderBy()`           | `OrderByDescending()` |
| `OrderByDescending()` | `OrderBy()`           |
| `ThenBy()`            | `ThenByDescending()`  |
| `ThenByDescending()`  | `ThenBy()`            |
| `Reverse()`           | `AsEnumerable()`     |
| `AsEnumerable()`     | `Reverse()`           |
| `Union()`            | `Intersect()`         |
| `Intersect()`        | `Union()`             |
| `Concat()`           | `Except()`            |
| `Except()`           | `Concat()`            |

## String Literals and Constants (_string_)
| Original | Mutated |
| ------------- | ------------- | 
| `"foo"` | `""` |
| `""` | `"Stryker was here!"` |
| `$"foo {bar}"` | `$""` |
| `@"foo"` | `@""` |
| `string.Empty` | `"Stryker was here!"` |

## Bitwise Operators (_bitwise_)
| Original | Mutated |
| ------------- | ------------- | 
| `<<` | `>>` |
| `>>` | `<<` |
| `&` | `\|` |
| `\|` | `&` |
| `a^b` | `~(a^b)` |

## Regular Expressions (_regex_)
For the full list of all available regex mutators, see the [regex mutator docs](./regex-mutators.md).
