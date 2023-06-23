---
title: Mutations
sidebar_position: 40
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/mutations.md
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
| `is` | `is not` |
| `is not` | `is` |

## Logical Operators (_logical_)
| Original                  | Mutated                   |
|---------------------------|---------------------------|
| `&&`                      | <code>&#124;&#124;</code> |
| <code>&#124;&#124;</code> | `&&`                      |
| `^`                       | `==`                      |
| `and`                     | `or`                      |
| `or`                      | `and`                     |

## Boolean Literals (_boolean_)
| Original | Mutated |
| ------------- | ------------- |
| `true`	| `false` |
| `false`	| `true` |
| `!person.IsAdult()`		| `person.IsAdult()` |
| `if(person.IsAdult())` | `if(!person.IsAdult())` |
| `while(person.IsAdult())` | `while(!person.IsAdult())` |

## Assignment Statements (_assignment_)
| Original             | Mutated              |
|----------------------|----------------------|
| `+=`                 | `-=`                 |
| `-=`                 | `+=`                 |
| `*=`                 | `/=`                 |
| `/=`                 | `*=`                 |
| `%=`                 | `*=`                 |
| `<<=`                | `>>=`                |
| `>>=`                | `<<=`                |
| `&=`                 | <code>&#124;=</code> |
| `&=`                 | `^=`                 |
| <code>&#124;=</code> | `&=`                 |
| <code>&#124;=</code> | `^=`                 |
| `^=`                 | <code>&#124;=</code> |
| `^=`                 | `&=`                 |
| `??=`                | `=`                  |

## Initialization (_initializer_)
| Original | Mutated |
| ---------------------------------------- | ------------------------------------ |
|`new int[] { 1, 2 };`                     | `new int[] { };`                     |
|`int[] numbers = { 1, 2 };`               | `int[] numbers = { };`               |
|`new List<int> { 1, 2 };`                 | `new List<int> { };`                 |
|`new Collection<int> { 1, 2 };`           | `new Collection<int> { };`           |
|`new Dictionary<int, int> { { 1, 1 } };`  | `new Dictionary<int, int> { };`      |
|`new SomeClass { Foo = "Bar" };`          | `new SomeClass { };`                 |

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
| `MinBy()`            | `MaxBy()`             |
| `MaxBy()`            | `MinBy()`             |
| `SkipLast()`         | `TakeLast()`          |
| `TakeLast()`         | `SkipLast()`          |
| `Order()`            | `OrderDescending()`   |
| `OrderDescending()`  | `Order()`             |
| `UnionBy()`          | `IntersectBy()`       |
| `IntersectBy()`      | `UnionBy()`           |

## String Literals (_string_)
| Original | Mutated |
| ------------- | ------------- |
| `"foo"` | `""` |
| `""` | `"Stryker was here!"` |
| `$"foo {bar}"` | `$""` |
| `@"foo"` | `@""` |
| `string.Empty` | `"Stryker was here!"` |
| `string.IsNullOrEmpty(x)` | `(x != null)` |
| `string.IsNullOrEmpty(x)` | `(x != "")` |
| `string.IsNullOrWhiteSpace(x)` | `(x != null)` |
| `string.IsNullOrWhiteSpace(x)` | `(x != "")` |
| `string.IsNullOrWhiteSpace(x)` | `(x.Trim() != "")` |

## Bitwise Operators (_bitwise_)
| Original            | Mutated             |
|---------------------|---------------------|
| `<<`                | `>>`                |
| `>>`                | `<<`                |
| `&`                 | <code>&#124;</code> |
| <code>&#124;</code> | `&`                 |
| `a^b`               | `~(a^b)`            |

## Regular Expressions (_regex_)
For the full list of all available regex mutations, see the [regex mutator docs](./regex-mutations.md).

## Math Methods (_math_)
|      Original           |         Mutated         |
| ----------------------- | ----------------------- |
| `Acos()`                  | `Acosh()`                 |
| `Acos()`                  | `Asin()`                  |
| `Acos()`                  | `Atan()`                  |
| `Acosh()`                 | `Acos()`                  |
| `Acosh()`                 | `Asinh()`                 |
| `Acosh()`                 | `Atanh()`                 |
| `Asin()`                  | `Asinh()`                 |
| `Asin()`                  | `Acos()`                  |
| `Asin()`                  | `Atan()`                  |
| `Asinh()`                 | `Asin()`                  |
| `Asinh()`                 | `Acosh()`                 |
| `Asinh()`                 | `Atanh()`                 |
| `Atan()`                  | `Atanh()`                 |
| `Atan()`                  | `Acos()`                  |
| `Atan()`                  | `Asin()`                  |
| `Atanh()`                 | `Atan()`                  |
| `Atanh()`                 | `Acosh()`                 |
| `Atanh()`                 | `Asinh()`                 |
| `BitDecrement()`          | `BitIncrement()`          |
| `BitIncrement()`          | `BitDecrement()`          |
| `Ceiling()`               | `Floor()`                 |
| `Cos()`                   | `Cosh()`                  |
| `Cos()`                   | `Sin()`                   |
| `Cos()`                   | `Tan()`                   |
| `Cosh()`                  | `Cos()`                   |
| `Cosh()`                  | `Sinh()`                  |
| `Cosh()`                  | `Tanh()`                  |
| `Exp()`                   | `Log()`                   |
| `Floor()`                 | `Ceiling()`               |
| `Log()`                   | `Exp()`                   |
| `Log()`                   | `Pow()`                   |
| `MaxMagnitude()`          | `MinMagnitude()`          |
| `MinMagnitude()`          | `MaxMagnitude()`          |
| `Pow()`                   | `Log()`                   |
| `ReciprocalEstimate()`    | `ReciprocalSqrtEstimate()` |
| `ReciprocalSqrtEstimate()` | `ReciprocalEstimate()`   |
| `ReciprocalSqrtEstimate()` | `Sqrt()`                 |
| `Sin()`                   | `Sinh()`                  |
| `Sin()`                   | `Cos()`                   |
| `Sin()`                   | `Tan()`                   |
| `Sinh()`                  | `Sin()`                   |
| `Sinh()`                  | `Cosh()`                  |
| `Sinh()`                  | `Tanh()`                  |
| `Tan()`                   | `Tanh()`                  |
| `Tan()`                   | `Cos()`                   |
| `Tan()`                   | `Sin()`                   |
| `Tanh()`                  | `Tan()`                   |
| `Tanh()`                  | `Cosh()`                  |
| `Tanh()`                  | `Sinh()`                  |

## Null-coalescing Operators (_nullcoalescing_)
| Original            | Mutated             |
|---------------------|---------------------|
| `a ?? b`            | `b ?? a`                |
| `a ?? b`            | `a`                 |
| `a ?? b`            | `b`                 |
