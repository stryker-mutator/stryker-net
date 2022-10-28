---
title: Research
sidebar_position: 20
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/technical-reference/research.md
---

Stryker.NET wants to be a blazing fast mutation test framework. To achieve this some research has taken place before designing the framework.

## Criteria
The framework had to conform the following criteria:
* Be fast
* Be able to show the exact location of the mutations to the user
* The following mutations should be able to be made
  * Binary mutations
  * Boolean mutations
  * Logical mutations
  * Conditional mutations
  * Unary mutations
  * Return value mutations

## Options
By looking at other frameworks two options could be found regarding architectures:
* Mutating source code ([Stryker](https://stryker-mutator.io/))
* Mutating byte code ([PiTest](http://pitest.org/))

Later another architecture was found: mutant schemata (also dubbed mutation switching). This technique places all mutations inside if statements. Such an if statement could look like:
``` csharp
if(Environment.GetEnvironmentVariable("ActiveMutation") == "1") {
  i--; // mutated code
} else {
  i++; // original code
}
```

## Comparison
For each option a separate prototype has been created. The results showed the following pros and cons. 

### Mutating source code
Pros:
* Exact location can be shown to users.

Cons:
* Each mutation has to be compiled separate. So mutating is slow.

### Mutating byte code
Pros:
* Fast

Cons:
* Mutators are difficult to create.
* Exact location cannot be shown to users.

### Mutant schemata (mutation switching)
Pros:
* All mutants can be compiled at once, so mutating is fast.
* Exact location can be shown to users.
* Mutated assembly can be kept in memory during mutation testruns.
* Mutation coverage can be easily calculated.
* Testing multiple mutations in one testrun is possible.

Cons:
* Not all mutations are possible
  * Mutating constant values
  * Mutating method names
  * Mutating access modifiers
* Compile errors should not occur.
  * Since all mutations will compile at once, all mutations should compile correctly

## Conclusion
Mutant schemata works fastest and the exact location for every mutation can be shown to the user. Stryker.NET has chosen the path of mutant schemata. 
