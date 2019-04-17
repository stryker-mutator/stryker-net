Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!


<!-- TOC -->
- [Arithmetic Operators](#arithmetic-operators)
- [Equality Operators](#equality-operators)
- [Boolean Literals](#boolean-literals)
- [Assignment statements](#assignment-statements)
- [Unary Operators](#unary-operators)
- [Update Operators](#update-operators)
- [Checked Statements](#checked-statements)
- [Linq Methods](#linq-methods)
- [String Literals](#string-literals)
- [Conditions](#conditions)
<!-- /TOC -->

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
| `&&` | `||` | 
| `||` | `&&` |

## Boolean Literals
| Original | Mutated | 
| ------------- | ------------- | 
| `true`	| `false` |
| `false`	| `true` |
| `!`		| ` ` |

## Assignment Statements
| Original | Mutated | 
| ------------- | ------------- | 
|`+= `	| `-= ` |
|`-= `	| `+= ` |
|`*= `	| `/= ` |
|`/= `	| `*= ` |
|`%= `	| `*= ` |
|`<<=`  | `>>=` |
|`>>=`  | `<<=` |
|`&= `	| `|= ` |
|`|= `	| `&= ` |

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

## String Literals
| Original | Mutated |
| ------------- | ------------- | 
| `"foo"` | `""` |
|  `""` | `"Stryker was here!"` |
| `$"foo {bar}"` | `$""` |
| `@"foo"` | `@""` |

## Conditions
| Original | Mutated |
| ------------- | ------------- |
| `if(enumerator.MoveNext())` | `if(!enumerator.MoveNext())` |
| `while(enumerator.MoveNext())` | `while(!enumerator.MoveNext())` |