# Mutators
Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

<!-- TOC -->

- [Mutators](#mutators)
    - [Binary mutator](#binary-mutator)
    - [Boolean mutator](#boolean-mutator)
    - [PrefixUnaryStatements](#prefixunarystatements)
    - [PostfixUnaryStatements](#postfixunarystatements)
	- [Checked mutator](#checked-mutator)

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

## PrefixUnaryStatements
|    Original   |   Mutated  | 
| ------------- | ---------- | 
|  `!variable` 	| `variable` |
|  `-variable`  | `+variable`|
|  `+variable` 	| `-variable`|
|  `~variable` 	| `variable` |
|  `++variable` | `--variable` |
|  `--variable` | `++variable` |
`
## PostfixUnaryStatements
|    Original   |   Mutated  | 
| ------------- | ---------- | 
| `variable++`  | `variable--` |
| `variable--`  | `variable++` |

## Checked mutator
| Original | Mutated |
| ------------- | ------------- | 
| `checked(2 + 4)` | `2 + 4` |