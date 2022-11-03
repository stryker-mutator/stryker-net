# Adding a new mutation strategy to Stryker
## An overview of the mutation process
After the initialization phase, during which Stryker.Net gathers information about the project, the mutation phase starts. The project’s source file are mutated separately using the [Roslyn API][1]. Each file’s **syntax tree** is recursively traversed and each discovered syntax node is submitted to the compatible mutators for them to generate mutations. Generated mutations are then buffered before being injected at the proper location (expression, statement, or block level) with the appropriate mutation switching logic (conditional operator or if statement).

When every file has been mutated, Stryker tries to build the mutated project, using a trial and error logic. When a compilation error happens on a mutation, this mutation is removed, if an error happens outside any mutations, Stryker preemptively removes all mutations from the current method (a.k.a. safe mode). Once compilation is successful, Stryker tests those mutations and generates one or more report.

## Key facts
These are the important information to bear in mind when designing a new mutator or working on improving an existing one:

1. Stryker.Net mutates the **syntax tree** and not the **raw source file**.
	1. This greatly simplifies the design and implementation of mutators
	2. On the other hand, one must not design a mutator based on text transformation logic.
> The syntax tree is an object representation of the source files where each syntax element (for example a statement) is described through object instances which classes match the syntax type.
> 
2. Stryker.Net **not does use the semantic model** of the syntax tree.
	1. This improves performance during mutation.
	2. On the other hand, one must not design a mutator relying on type information (e.g. Linq / Threading dedicated mutators).
> The semantic model is an improved version of the syntax tree which adds the type information to the syntax elements.
 3. Stryker.Net visits every syntax element; for example, a method invocation is first visited as a whole (e.g., `client.ChangeName(firstName, lastName)`), then  each element:object (`client`), method name (`ChangeName`) then each parameter(`firstname'` and `lastname`). _Note that if parameters are expressions, those are visited in a similar (and recursive) fashion._ 
4. Each file is mutated separately.
	1. One cannot design a mutator exploiting multiple files at once.
5. Stryker.Net takes care of roll backing in case of compilation errors
	1. Hence, mutators do not need to ensure mutation properly compiles
	2. On the other hand, mutator should avoid triggering ambiguous errors, as those lead to rolling back many mutations.

## Attributes of a good mutator
These are the attributes the Stryker’s project team will check before agreeing to a new mutator.
A mutator:

1. **Must generate mutations looking like possible errors**: the objective is not to generate as many mutants as possible, but to try **to reproduce potential human mistakes**.
2. **Should be fast**: Stryker visits every syntax elements recursively, meaning that each syntax item is visited several times. A slow mutator can seriously slow down the mutation process.
3. **Should generate buildable mutations**.: generated mutations should result in **compilable code in the majority of situations**.
4. **Should  generate survivors**: mutators **should avoid** generating **mutations that often raise exceptions** (e.g., changing the sign of an index, such as `array\[i\]` to `array\[-i\]`) are those will be killed by any test that execute these lines.
5. **Should generate killable mutations**: users must be able to devise a test that can kill the mutation. The main risk here is getting_semantically equivalent_ mutations, i.e., mutations that do change the behavior of the code and simply result in an alternate, but correct, implementation.
6. **Should be general**: mutators should be able to generate mutations for all projects, i.e., should not be specific to some rarely used constructions 5or type of projects. **Note**: in the future, Stryker.Net may support extra mutators via a plug-in like mechanism; those could be framework specific.

## How to code a mutator?
### Prerequisites
- You must have at least cursory knowledge of Roslyn APIs dedicated to syntax handling.
- You should verify your mutator will respect the previous list of attributes.
- You should have a look at unit test for existing mutators as it will help you write yours.
- You should as well examine the code for mutator(s) which is/are similar to the one you plan to write.
### General
Every mutator must implements the `IMutator`interface, with a single method `IEnumerable<Mutation> Mutate(SyntaxNode node, StrykerOptions options)`. The _Mutate_ method is called on every syntax element and the implementation must return an enumeration of generated mutations (or empty list if the mutator is not able to mutate the given _node_.

Adding your mutator means:
- adding a class that will implementation the mutator. It must implement `IMutator` and should inherit from `MutatorBase` (see below)
- adding an entry in the `Mutator enum`as an identifier for your mutations
- creating an instance of it in the `CsharpMutantOrchestrator`constructor.
You can create a mutator without inhering from `MutatorBase`, but there is little benefit doing so.

### MutatorBase\<T\>
The `MutatorBase<T>`provides you with:

- Automatic filtering of `SyntaxNode`. I.e. your mutator will only be presented `SyntaxNode`class(es) it can handle
- Typed mutation: your mutator will be submitted typed syntax elements and not `SyntaxNode`which type you need to discover.
- simplified configuration for the user: you can specify a `MutationLevel`value for your mutator so that mutation can be skipped if the configuration level is insufficient

### Implementing the mutation
You need to:

- add a new class that inherits from `MutatorBase<T>`and implements `IMutator`. Note that MutatorBase already implements the needed method so no need to actually implement anything.
- specify the expected class of node (above`T`) you are able to mutate (ex. `StatementSyntax`for full statement). If your mutator handles several classes, you can either pick the common ancestor to those classes (up to `SyntaxNode` if needs be), implement several classes, design a generic class or a combination of these.
- override the `MutationLevel` property and have it return the appropriate level. You should probably start with `Complete` or `Advanced`. This choice will have to be discussed with the project team, but it should happen when the mutator is stable
- override the `ApplyMutation<T>` method (where `T`is the type you choose as per the first item) and make it returns the one or mutations you are able to generate from the `T` syntax element you received as a parameter. You should look at existing implementation for further guidance


## Final words
- Do not hesitate to reach for help and assistance via GitHub discussions or the project Slack.
- Writing mutator is trivial for simple constructs but difficulty increases exponentially as syntax elements grow in size
- Invest in unit tests early. You need to be able to reproduce many situation
- Bear in mind that syntax constructs changed across C# versions, so make sure your mutator is compatible with various variants (e.g. expression body or block statement body). 
- It is ok to mutate new constructs (such as arrow expression) and leave the old constructs as is. The reverse is not true.
- Your mutator must return an empty list (or `yield break`) if it cannot generate any mutation.
- Mutator must not throw.
- Make sure to update the [mutations documentation](docs/mutations.md).

[Back to main contributing document](CONTRIBUTING.md).

[1]:	https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/get-started/syntax-analysis "Get started with syntax analysis"