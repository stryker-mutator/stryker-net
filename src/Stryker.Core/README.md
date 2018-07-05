# Mutators
Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

<!-- TOC -->

- [Mutators](#mutators)
    - [Binary mutator](#binary-mutator)
    - [Boolean mutator](#boolean-mutator)
    - [Assignment mutator](#assignment-mutator)
    - [PrefixUnaryStatements](#prefixunarystatements)
    - [PostfixUnaryStatements](#postfixunarystatements)

<!-- /TOC -->

## Binary mutator
| Original | Mutated | 
| ------------- | ------------- | 
| `+` | `-` |
| `-` | `+` |
| `*` | `/` |
| `/` | `*` |
| `%` | `*` |
| `>` | `<` |
| `>` | `=>` |
| `=>` | `<` |
| `=>` | `>` |
| `<` | `>` |
| `<` | `=<` |
| `=<` | `>` |
| `=<` | `<` |
| `==` | `!=` |
| `!=` | `==` |
| `&&` | `\|\|`
| `\|\|` | `&&`

## Boolean mutator
| Original | Mutated | 
| ------------- | ------------- | 
| `true` | `false` |
| `false` | `true` |

## Assignment mutator
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

## PrefixUnaryStatements
|    Original   |   Mutated  | 
| ------------- | ---------- | 
|  `!variable` 	| `variable` |
|  `-variable`  | `+variable`|
|  `+variable` 	| `-variable`|
|  `~variable` 	| `variable` |
|  `++variable` | `--variable` |
|  `--variable` | `++variable` |

## PostfixUnaryStatements
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `variable++`  | `variable--` |
| `variable--`  | `variable++` |

