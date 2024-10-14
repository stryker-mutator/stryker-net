---
title: Regex mutations
sidebar_position: 50
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/regex-mutations.md
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

## Character class mutators

### CharClassChildRemoval

Remove a child of a character class.

| Original | Mutated |
|----------|---------|
| `[abc]`  | `[bc]`  |
| `[abc]`  | `[ac]`  |
| `[abc]`  | `[ab]`  |

### CharClassAnyChar

Change a character class to a character class which matches any character.

| Original | Mutated  |
|----------|----------|
| `[abc]`  | `[\w\W]` |

### CharClassRangeModification

Change the high and low of a range by one in both directions if possible.

| Original | Mutated |
|----------|---------|
| `[b-y]`  | `[a-y]` |
| `[b-y]`  | `[c-y]` |
| `[b-y]`  | `[b-z]` |
| `[b-y]`  | `[b-x]` |

## Predefined character class mutators

### PredefCharClassNullification

Remove the backslash from a predefined character class such as `\w`.

| Original | Mutated |
|----------|---------|
| `\d`     | `d`     |
| `\D`     | `D`     |
| `\s`     | `s`     |
| `\S`     | `S`     |
| `\w`     | `w`     |
| `\W`     | `W`     |

### PredefCharClassAnyChar

Change a predefined character class to a character class containing the predefined one and its
negation.

| Original | Mutated  |
|----------|----------|
| `\d`     | `[\d\D]` |
| `\D`     | `[\D\d]` |
| `\s`     | `[\s\S]` |
| `\S`     | `[\S\s]` |
| `\w`     | `[\w\W]` |
| `\W`     | `[\W\w]` |

### UnicodeCharClassNegation

Flips the sign of a Unicode character class.

| Original    | Mutated     |
|-------------|-------------|
| `\p{Alpha}` | `\P{Alpha}` |
| `\P{Alpha}` | `\p{Alpha}` |

## Quantifier mutators

### QuantifierShortModification

Treat the shorthand quantifiers (`?`, `*`, `+`) as their corresponding range quantifier
variant (`{0,1}`, `{0,}`, `{1,}`), and applies other mutations to the new node.

| Original | Mutated    |
|----------|------------|
| `abc?`   | `abc{1,1}` |
| `abc?`   | `abc{0,0}` |
| `abc?`   | `abc{0,2}` |
| `abc*`   | `abc{1,}`  |
| `abc+`   | `abc{0,}`  |
| `abc+`   | `abc{2,}`  |

### QuantifierReluctantAddition

Change greedy quantifiers to reluctant quantifiers.

| Original    | Mutated      |
|-------------|--------------|
| `abc?`      | `abc??`      |
| `abc*`      | `abc*?`      |
| `abc+`      | `abc+?`      |
| `abc{9}`    | `abc{9}?`    |
| `abc{9,}`   | `abc{9,}?`   |
| `abc{9,13}` | `abc{9,13}?` |

## Group-related construct mutators

### GroupToNCGroup

Change a normal group to a non-capturing group.

| Original | Mutated   |
|----------|-----------|
| `(abc)`  | `(?:abc)` |