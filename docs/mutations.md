---
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/mutators.md
---

Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

## Arithmetic Operators
| Original | Mutated | 
| ------------- | ------------- | 
| `+` | `-` |
| `-` | `+` |
| `*` | `/` |
| `/` | `*` |
| `%` | `*` |

## Equality Operators
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

## Logical Operators
| Original | Mutated | 
| ------------- | ------------- | 
| `&&` | `\|\|` | 
| `\|\|` | `&&` |
| `^` | `==` |

## Boolean Literals
| Original | Mutated | 
| ------------- | ------------- | 
| `true`	| `false` |
| `false`	| `true` |
| `!person.IsAdult()`		| `person.IsAdult()` |
| `if(person.IsAdult())` | `if(!person.IsAdult())` |
| `while(person.IsAdult())` | `while(!person.IsAdult())` |

## Assignment Statements
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

## Collection initialization
| Original | Mutated | 
| ---------------------------------------- | ------------------------------------ |
|`new int[] { 1, 2 };`                     | `new int[] { };`                     |
|`int[] numbers = { 1, 2 };`               | `int[] numbers = { };`               |
|`new List<int> { 1, 2 };`                 | `new List<int> { };`                 |
|`new Collection<int> { 1, 2 };`           | `new Collection<int> { };`           |
|`new Dictionary<int, int> { { 1, 1 } };`  | `new Dictionary<int, int> { };`      |

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
| `++variable`	| `--variable` |
| `--variable`	| `++variable` |

## Checked Statements
| Original | Mutated |
| ------------- | ------------- | 
| `checked(2 + 4)` | `2 + 4` |

## Linq Methods
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
| `Sum()`               | `Max()`*               |
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

`* Sum() was mutated to Count() in previous versions; this change reduces compilation errors.`

## String Literals and Constants
| Original | Mutated |
| ------------- | ------------- | 
| `"foo"` | `""` |
| `""` | `"Stryker was here!"` |
| `$"foo {bar}"` | `$""` |
| `@"foo"` | `@""` |
| `string.Empty` | `"Stryker was here!"` |

## Bitwise Operators
| Original | Mutated |
| ------------- | ------------- | 
| `<<` | `>>` |
| `>>` | `<<` |
| `&` | `\|` |
| `\|` | `&` |
| `a^b` | `~(a^b)` |

## Regular Expressions
For the full list of all available regex mutations, see the [regex mutator docs](./regex-mutations.md).
