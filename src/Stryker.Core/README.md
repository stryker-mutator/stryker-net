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
