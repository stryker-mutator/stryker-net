# Mutators
Stryker supports a variety of mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

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

## Checked mutator
| Original | Mutated |
| ------------- | ------------- | 
| `checked(2 + 4)` | `2 + 4` |
