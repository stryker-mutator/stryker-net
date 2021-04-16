---
title: Regex mutations
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/regex-mutators.md
---

Stryker supports a variety of regular expression mutators, which are listed below. Do you have a suggestion for a (new) mutator? Feel free to create an [issue](https://github.com/stryker-mutator/stryker-net/issues)!

## Common tokens
| Original | Mutated |
| ------------- | ------------- | 
| `[abc]` | `[^abc]` |
| `[^abc]` | `[abc]` |
| `\d` | `\D` |
| `\D` | `\d` |
| `\w` | `\W` |
| `\W` | `\w` |
| `\s` | `\S` |
| `\S` | `\s` |

## Anchors
| Original | Mutated |
| ------------- | ------------- | 
| `^abc` | `abc` |
| `abc$` | `abc` |
| `\Aabc` | `abc` |
| `abc\Z` | `abc` |
| `abc\z` | `abc` |
| `abc\b` | `abc` |
| `abc\B` | `abc` |
| `\Gabc` | `abc` |
| `abc*` | `abc` |
| `abc?` | `abc` |
| `abc+` | `abc` |

## Quantifiers
| Original | Mutated |
| ------------- | ------------- | 
| `abc{5}` | `abc` |
| `abc{5,}` | `abc` |
| `abc{5,8}` | `abc` |
| `abc{5,8}` | `abc{4,8}` |
| `abc{5,8}` | `abc{6,8}` |
| `abc{5,8}` | `abc{5,7}` |
| `abc{5,8}` | `abc{5,9}` |
| `abc{5,}` | `abc{4,}` | 
| `abc{5,}` | `abc{6,}` |

## Group constructs
| Original | Mutated |
| ------------- | ------------- | 
| `(?=abc)` | `(?!abc)` |
| `(?!abc)` | `(?=abc)` |
| `(?<=abc)` | `(?<!abc)` |
| `(?<!abc)` | `(?<=abc)` |
