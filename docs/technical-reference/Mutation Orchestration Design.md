# Stryker’s crash course on C# syntax parsing and mutation


## Audience
This document’s purpose is to help developers who need or want to understand how Stryker.Net parses and mutates C# source code. The current design (as in V4.x) is the result of several years of experience and refinement and may be intimidating at first.

## Scope: what’s in what’s out
This document focuses on the source mutation step. That is how Stryker.Net generates a mutated version of a source file. It does not cover what happens before (project analysis, tests discovery, coverage capture…) nor what happens afterward (mutated project build, mutation tests, reporting…).
It describes the patterns and main classes, without detailing specifics that can be understood when reading the code itself or using code documentation.

# Roslyn
Stryker’s parsing and mutation logic relies extensively on Roslyn classes, so you need to be familiar with it if you want to understand or contribute to this part of Stryker.
Note that we made the creation of mutation engines (aka mutators) as easy as we could. In fact, this is arguably the easiest part of the overall mutation logic.

## Syntax tree
Stryker relies heavily on Roslyn for C# parsing. As such, a good understanding of it is important before digging further.
Roslyn precompiles any valid source file to an objects tree where each node is apart of  syntax construct. Each node stores and describes the associated source code, including trivia (whitespaces and comments). The tree structure captures the code hierarchy. So you have a syntax root node (the whole file), containing a namespace node  (namespace directive) which usually contains a class definition node which contains various members. The class likely contains at least one method definition, with a name, parameters and a body, made of statements. Statements contain a keyword and expressions and these contain sub expressions, potentially recursively as an expression can contain a lambda etc…
As such, this syntax tree is often deep (tens, even hundreds of levels).
Each node is described via the appropriate Roslyn class, such as `InvocationExpressionSyntax` for a function call, or `ForStatementSyntax` for a _for_ loop. [Sharplab](https://sharplab.io/) allows you to discover the syntax tree of any provided C# source file. I strongly advise you to try and use it. Quick warning: Sharplab displays the **SyntaxKind** of the syntax constructs, which may be slightly different from the actual class used to represent it; as a reminder, **SyntaxKind**  refines the type definition and enables to identify variants of some constructs, such as between `x++` and `x—-`.

### Syntax Node classes
There are hundreds of specific syntax nodes classes, as most C# constructs get their own class. So one can have specific access to each part of a for statement: initializers, condition, increments and statement (or block statement). This greatly simplifies reading, modifying or creating any syntax construct. 
Also, `SyntaxNode` can bear ‘annotations’. These are stored as pairs of strings (name and value) but these are not reflected (nor persisted) in the source code. These annotations are useful for multi pass processing of the source code.

The Roslyn class hierarchy is shallow, with most syntax nodes inheriting either from `StatementSyntax` or `ExpressionSyntax`. Those classes are great, but the bad news is that there is only one class inheritance tree. So what about syntax constructs that have multiple roles, such as a method, which is both a class member and an invokable function ?
Roslyn answer is: methods inherit from `BaseMethodDeclarationSyntax` and you are on your own for the invokable bit. 
In more plain terms, if some behavior is shared between some syntax constructs, this is probably not captured by Roslyn classes structures (using Interfaces would have been a good approach for that). On a similar note, naming is not guaranteed to be consistent across classes.

### A word of warning
One of the consequences is that one must extensively try syntax constructs to ensure all use cases are covered.
The nominal example for this is: there are at least 5 different classes that represent some callable code: `LocalFunctionStatement`, `AnonymousFunctionExpression` (which is derived in two sub classes), `PropertyDeclarationSyntax`, `AccessorDeclarationSyntax` and `BaseMethodDeclarationSyntax` (which is derived in several sub classes).  Most of them accept one or more parameters, may return a value and have either a classical body or an arrow expression body. But if you need/want to handle functions in general, you will have to duplicate your code for all these classes.
Another problem is worth mentioning: Roslyn documentation is lacking, as most of the doc is automatically generated from the signatures without any further comments or explanation. Last but not least, there is no way to know what kind of syntax construct each class represents (except guessing).

## Modifying the syntax tree
Roslyn syntax objects are immutable: you cannot alter any syntax node. **Modifying a syntax node means getting a new copy of this node with the modification**. While this is a good design philosophy for many scenarios, this is less than ideal for mutation, but I will discuss this later.
Syntax nodes can be modified via a set of `WithXXX` methods which allow to change one of the node’s properties at a time. These calls can be chained via a builder pattern: `node.WithXXX(...).WithY(...)...`.
Building syntax node can be done via the `SyntaxFactory` (static) class. But  modifying a node should be preferred to creating a new one (from scratch), as this is the surest way to keep as much information as possible. 
As there are no `WithoutXXX(...)` kind of methods, the proper way to remove some optional bit of a construct is to use `WithXXX(...)` with `null`  argument(s).

Warning: sometimes modifying syntax nodes may not give the expected result.

For example, one cannot change the type of a `BinaryExpressionSyntax` (which represents most arithmetic and binary operations, such as `+`, `/`, `&`….) simply by changing the operator via the `WithOperatorToken(...)`. You must create a new node via `SyntaxFactory.BinaryExpression(...)` (with the proper `SyntaxKind` value) and then set the adequate token.
Changing the token changes the text representation, but the **compiled version will retain the original operator!**

# Design Principles for mutation
Stryker project team strives to find a good balance between performance, concision and ease of maintenance. Having kept these goals in mind and having gone through design revisions, a few key principles emerged as important:
1. Mutation generators (a.k.a mutators) are implemented in dedicated classes
2. No coupling between mutation orchestration and the mutators logic. Mutators must only focus on generating mutations where possible, not how they will be injected
3. Minimize memory allocation while walking the syntax tree
4. Isolate per syntax construct logic in dedicated classes
5. Prevent accidental ‘mutation loss’ (more about this later)

To this list, I would add: throw everything in the air when facing the unexpected. That is, Stryker mutation code will fail early and throw when dealing with non supported situations. We are aware this means blocking some users, but a clear failing situation shortens the analysis time and reduces the impact. A defensive/tolerant approach would result in Stryker failing to generate mutants for unknown syntax structure. And this problem would remain undetected for several iterations as this would be lost in the mass of generated mutants.

# Stryker mutation design
Taking into account previously listed principles, we designed the following classes/class hierarchy:
1. **Mutators**: **Generates mutated version** of syntax node. Each of them implements a specific mutation strategy. They must implement [`IMutator`] and must be stateless (they can only store their configuration/options).
2. **SyntaxNode Orchestrators**: **Orchestrates the steps required for** the mutation of a specific syntax node kind. They must implement [`INodeMutator`], and must be stateless. They are responsible for walking through the syntax tree, and determining where mutation can be placed.```
3. [`MutationContext`]: **Keeps track of the current state of the mutation process**. It stores mutations (via [`MutationStore`]) until they are injected in the mutated syntax tree; it also tracks which mutators have been disabled via [Stryker comments] as well as other details.
4. [`CSharpNodeOrchestrator`]: **Main entry point**. While it was doing all the work in earlier version of Stryker, it is now responsible for building the needed orchestrators, mutators, the mutant placer and keeping track of created mutations.
5. [`MutantPlacer`]: **Serves** as a single entry point to **several helper functions**, including the ones for injecting mutations. Also in charge of providing the needed information for rollback logic.

While understanding how everything works together is non trivial, each class is focused on a clear responsibility, making it easier to fix any problem or adjusting/extending existing behavior. This design mostly leverages 2 traditional patterns: **strategy** (mutators, orchestrator) and **state** (MutationContext and MutationStore).
It is now time to dive deeper into each of those classes.

## Mutators
As noted previously, a mutator class is responsible for generating zero or more mutations of provided syntax nodes. Syntax nodes will be submitted to **all active mutators for consideration**, so mutators need to be efficient, at least on the filtering front (can they mutate this node or not?).

Mutator must implement the `IMutator` interface, as required for the mutation mechanism, but Stryker’s mutators actually inherit from `MutatorBase<T>`. Then mutators must:
1. Specify which kind of SyntaxNode they can mutate (`T` type parameters). `MutatorBase` will filter out  syntax nodes which are of a different type. Using `SyntaxNode` will ensure the mutator will see all nodes considered for mutation.
2. Implement `MutationLevel` property. This relates to fast configuration of mutators (see [website] ).
3. Implement `IEnumerable<Mutation> ApplyMutation(T node, SemanticModel model)` which returns a collection of mutations for the provide syntax node _node_.

You can look at [`StringEmptyMutator`] for an example of a mutator of average complexity. Implementations of `ApplyMutation` verify they can handle the provided SyntaxNode, then generate mutation(s) (for this node) often via `With...` methods to replace part of it. Then returns them via a `yield return` statement.

## Node Orchestrators
These classes bear most of the complexity. First and foremost, they prescribe the  mutation workflow, that is in which order perform the important steps: 

1. generate the mutation(s) (if any), 
2. mutate the child nodes (that is, perform this workflow for every child node)
3. inject mutations with the adequate control structure (`if` or ternary operator)
4. Return to the parent

This order is important because:
- it is the simplest way to deal with Roslyn immutability: if mutations were injected early in the workflow, the code would have to identify where to continue the syntax tree walking in the modified version.
- creating a modified version of a syntax node **results in having an orphaned syntax node**, I.e. outside any syntax tree (source file). One must reinject it via a reverse recursion, that is creating a new version of the parent node with the modified node and so on up to the root
- it keeps a (mostly) natural read order for mutation : mutations appear in the order they are created.
They provide an implementation for the major syntax structures (expressions, statements, blocks) but also implement specific strategies for the subtler syntax constructs.
As a reminder, they must be stateless, and this is a strong requirement. These classes must not store anything while walking the syntax tree. They should not assume this walking happens in any specific order. They are likely to be used recursively, depending on the currently traversed code.
We will describe the base class here, all orchestrators will be described at the end of this document as an attempt to keep it readable.

### `NodeSpecificOrchestrator<T, TU>`
This is the base class used by all other orchestrators. It inherits from `NodeOrchestratorBase` which deals with Stryker comments and is not described in this document for brevity.
It implements a standardized node mutation workflow. This workflow’s steps are implemented via virtual methods so that specific orchestrators can provide the adequate implementation

- `Mutate(...)`: this is the main method; it specifies the steps order:

	1.  Invokes `PrepareContext` to get a `MutationContext` instance in the adequate state.
	2.  Invokes `GenerateMutationForNode` to get the list of possible mutations (if any) for the current node.
	3.  Invokes `StoreMutations` so that generated mutations are queued for later injection
	4.  Invokes `OrchestrateChildrenMutation` to get the mutated children and prepare a mutated version of this node
	5.  Invokes `InjectMutations` to try inserting pending mutations (nothing happens if mutations cannot be injected here)
	6.  Finally, invokes `RestoreContext` to restore the adequate context when leaving this node. 
	
- `CanHandle(TNode)` : should return true if the orchestrator can handle the provided Node. Default implementation checks if node is of type `TNode`
- `InjectMutations(...)`: should inject pending mutations if the node can host mutation switching logic and return the resulting node. Default implementation does nothing.
-  `GenerateMutationsForNode(...)`: should generate all possible mutations for the provided node and return them. Default implementation forwards the call to the `MutationContext` instance. Child classes should rely on `MutationContext` to generate mutations as it uses configuration and Stryker comments to figure out which mutators to use. As such, the main reason to override this method is to prevent mutations to be generated.
- `StoreMutations(...)`: should store provided mutations at the appropriate (switching control level). Default implementation stores them at the current level. Child classes should override this method when they need to store some or all mutations at a higher control level.
- `OrchestrateChildrenMutations(...)`: should build a mutated version of the node integrating all children nodes’ mutations. Default implementation does that. Child classes should override this method when they need to control how child nodes are mutated and/or adjust the context before or during children mutation.
-  `PrepareContext(...)`: should update context according to the current mutation control and whether this is a static context or not. Most child classes override this method or inherit from a class overriding this. Default implementation does nothing
- `RestoreContext()`: should restore context. Child classes must override this if they override `PrepareContest`.
   

## Mutation context(fn)
`MutationContext` class’s job is to store the current state of the mutation orchestration.  Its responsibilities are:
1. Store mutations as they are created
2. Inject stored mutations appropriately when requested
3. Track static context while walking the syntax tree.

Below is the list of its methods, split between the ones that are useful for an orchestrator and the ones that are used by Stryker’s main logic (and are not needed when implementing an orchestrator)
### Context management
These methods are used in the `PrepareContext(...)`/`RestoreContext(...)` methods.
Orchestrators generally must specify the proper syntax level (note that a parent class may have done it). Useful methods for an orchestrator
- `EnterStatic(...)`: returns a new `MutationContext` instance to use inside a static context (fields/constructor)
- `Enter(...)`: returns a `MutationContext` instance to use for `SyntaxNode` of significant hierarchical level (member, block, statement, expression, member access) 
- `Leave()`: returns a `MutationContext` restored. Used when leaving a  `SyntaxNode` of significant hierarchical level (member, block, statement, expression, member access). When leaving a context level, any remaining mutations are transferred to the enclosing level (e.g. from expression to statement) so they can be injected. Any mutation that ultimately fails to be injected is flagged as compilation error state and logged accordingly.


Used by the main logic:
- `InStaticValue`: property,  `true` when inside a static constructor or initializer. This is used to identify static mutants.
- `MustInjectCoverageLogic`: property, `true` when Stryker needs to inject runtime static markers for proper static mutations detection (it depends on Stryker configuration)
- `FindHandler(...)`:  returns an `NodeOrchestrator` instance able to handle the provided `SyntaxNode` type.
### Mutations management
Orchestrator will use these methods to control at which syntax level mutations will be inserted and to inject mutations for compatible syntax nodes.

Methods useful for orchestrators:
- `AddMutations(...)` : store mutations for the current syntax level (default) or at a specific level. For example, mutating a syntax node containing a variable declaration should be done at the block level (i.e. creating mutants of the whole statement block), otherwise you will get compilation errors as a variable can only be declared once.
- `HasPendingMutations(...)`: property, `true` if there are any mutations that must be injected at the current level.
- `InjectMutations(...)`: inject mutations within the provided syntax node (either an expression, statement or block). There is a specific overload dedicated to the mutation of expression bodies, which must be converted to block statement bodies as they are mutated.

Used by main logic:
- `GenerateMutantsForNode(...)`: returns the list of `Mutants` for the provided node using the configured list of mutators.
- `FilterMutator(...)`: adjusts the list of active mutators ‘in flight’. This method is used when parsing Stryker Comments.
### Implementation details : `MutationStore`
An important part of mutation context is the `MutationStore` class, which is in charge of storing generated mutations, until they are injected.
It uses an internal stack of stores, where each entry matches a syntax level (member, block, statement, expression, memberAccess). Note that the stack may go deep thanks to local functions and lambdas/anonymous functions.
An entry is pushed on the stack when an orchestrator calls `MutationContext.Enter(...) ` and is popped when `MutationContext.Leave()`is called (the algorithm compresses empty levels to keep the stack as small as possible.
The store automatically promotes any pending mutations to the next higher level when leaving it, or flag them as compilation error if promotion is not possible.

It also implements injection methods that generate mutated versions of syntax nodes (expression, statement or block) enclosed in the appropriate mutation switching construct: ternary operator(s) for expressions and if statement(s) for statements and blocks.

## MutantPlacer class
`MutantPlacer` main responsibility is to ensure any code modification may be rolled back. As a reminder, Stryker assumes the mutation phase will result in compilation errors. When these are detected during compilation, Stryker will try to locate the code transformation that is the cause of this compilation error and undo it. Alas, this is more complex than simply ‘putting back the original code’. Using such a coarse approach would result in rolling back viable mutations due to the removal of any inner modifications.
Instead, Stryker relies on code injection engines that provide a method to revert the change(s) made.

### Injection engines
The `MutantPlacer` class implements the infrastructure for this logic, via a strategy pattern (again). Any class that modifies the source code must implement `IInstrumentCode` which exposes two members:
1. `string IntstrumentEngineId {get…}` which provides the (unique) engine identifier
2. `SyntaxNode RemoveInstrumentation(SyntaxNode)` which must revert any modifications done by the engine.
It must also be registered with the `MutantPlacer class` (via `MutantPlacer.RegisterEngine(...)`. This method returns a `SyntaxAnnotation` instance that must be attached to any node altered by the injection engine (the roll back logic uses these annotations to identify what can be rolled back and which engine is appropriate).

Note that there is no signature for the injection part, only the removal. The reason for this is twofold:
1. Engines are task specific and there is no identified situation where Stryker would benefit from a ‘generic’ injection mechanism
2. This allows method signature to better match the use case(s).
### Injection methods
The orchestration logic uses a shared `MutantPlacer` during the mutation process of one project. This instance stores the unique namespace name that is used by the mutation switching logic for this project. So injection methods related to mutation switching are instance methods, the others are often static.
`MutantPlacer` aggregates a few injection engines that are useful across the mutation logic. Those can be used via `MutantPlacer`’s methods.
#### Compilation helpers
Those engines inject code that are used to reduce the number of compilation errors. Indeed, some mutants alter the control flow which can lead to using non initialized variables, or failing to return a value. So Stryker injects statements to limit the risk of such an event.
- `AddEndingReturn(...)`: returns a copy of the provided `BlockSyntax` with a `return default(type)` statement added at the end. This method does not add a `return` if:
	- this is an iteration method (`yield` statement)
	- return type is `void` or a `Task` (for async method).
	- the last statement is a `return` or a `throw`
	- the block does not contain any `return`
	
	This method ensures mutations do not break the whole method and trigger roll back logic.
- `PlaceStaticContextMarker(...)`:  injects the logic used by Stryker to track _static context_ (i.e. code that is run during some static initialization). It relies on a `using` expression. This method supports blocks and expressions.
	This method must be used to mark any static construct, disregarding mutation.
- `InjectOutParametersInitialization(...)`: adds statements to initialize any out parameters to their default value.
#### Switching logic injectors
Those engines inject mutations and the switching logic (`if` or `?:`) to control them.
-  `PlaceStatementControlledMutations(...)`: build an `if` statement with the provided mutation as the `true` condition and the provided statement/block as the `false` condition. The `if` statement will be chained if there is more than one mutation.
- `PlaceExpressionControlledMutations`:  build a ternary operator (`?:`) expression with the provided mutation as the `true` condition and the provided expression as the `false` condition. The conditional expression will be chained if there is more than one mutation.
#### Rollback helpers
Those engines are used during to remove mutations (and helpers) that cause compilation errors.
- `RemoveMutant(...)`: returns a syntax node with the mutation removed.
- `RequiresRemovingChildMutations(...)`: returns true if the syntax node contains an injection that can not be removed without removing any child mutation/injection first.
- `FindAnnotations(...)`: returns information regarding injection in the provided syntax node, such as used engine and mutant id (if relevant)
## Node orchestrators
You can browse this list for figuring out how each syntax structure is mutated, which is useful when trying to understand why Stryker does not give the result you expected or fails.  Remember that Stryker tries out orchestrators in declaration order until one `CanHandle` returns true; so make sure that the most specific orchestrators are declared first.
Also, you may want to check `DoNotMutateOrchestrator`  when trying to understand why some syntax constructs are not mutated: this may be deliberate.
You can start by looking at `CSharpMutantOrchestrator.BuildOrchestratorList()` to see which orchestrators are used and their relative declaration order.

### Base classes
These classes are responsible for the general structure of the orchestration phases and/or are used as parent classes to other, more specific, orchestrators.
#### CsharpMutantOrchestrator
Not an orchestrator per se. This is the starting point for mutating C# source files. It aggregates the configured mutators and stores mutations are they are generated. It inherits from `BaseMutantOrchestrator<T,TU>` which implements the base logic needed by the overall workflow.
Note that there is an `FsharpMutantOrchestrator` class for a future support of F#.
- `MutantPlacer Placer`: this property stores the `MutantPlacer` instance that should be used for code injection
- `SyntaxNode Mutate(...)`: mutates an entire syntax tree
- `GetHandler(...)`: returns the appropriate orchestrator for a given syntax node
- `GenerateMutationsForNode(...)`: returns all mutants (as mutant instances) for a given node, according to configuration and context/Stryker comments).
#### NodeSpecificOrchestrator
`NodeSpecificOrchestrator<TNode, TBase>` is the base class of all orchestrators. It implements the workflow described earlier and defines several virtual methods that inheriting orchestrators can override to customize the workflow. It has been described earlier in this document.
#### ExpressionSpecificOrchestrator
`ExpressionSpecificOrchestrator<T>` handles syntax nodes that are part of an expression. Note that some syntax constructs inheriting from `ExpressionSyntax` are not considered as expressions from Stryker point of view. 
This class provides the following implementations
- `PrepareContext(...)`: declares a `MutationControl.Expression` context and override `RestoreContext()`
- `StoreMutations(...)`: stores mutation in the current context or next higher block context if the mutation contains a variable declaration.
- `InjectMutations(...)`: inject mutations controlled by conditional operator.
#### StatementSpecificOrchestrator
`StatementSpecificOrchestrator` handles `StatementSyntaxNodes` (and inheriting nodes).
This class provides the following implementations
- `PrepareContext(...)`: declares a `{MutationControl.Expression}` context .
- `RestoreContext()`: leaves the current context.
- `InjectMutations(...)`:  injects mutations controlled by if statements.
#### MutateAtStatementLevelOrchestrator
`MutateAtStatementLevelOrchestrator` class deals with (expression level) syntax constructs which mutations should be controlled at statement level, because they must be controlled via an `if` statement.
The constructor accepts a _predicate_ which is used for finer identification of SyntaxNodes.
This class implements:
- `CanHandle(...)`: forwards the request to the predicate (if any)
- `StoreMutations(...)`: stores mutation at the next higher statement level.

This class is used for:
- `Post/PrefixUnaryExpressionSyntax` (++/--): when part of a for statement or an expression statement
- `InitializerExpressionSyntax`: for non empty array declarations.
- `AssignmentExpressionSyntax` (x=value) as they  cannot be part of a ternary operation.

#### MemberDefinitionOrchestrator
`MemberDefinitionOrchestrator<T>` is a generic (base) class to help implement orchestrator for `SyntaxNode` that define class members. It implements:
- `PrepareContext(...)`: creates a `MutantControl.Member` context.
- `RestoreContext()`: leaves the current context

#### BaseFunctionOrchestrator
`BaseFunctionOrchestrator<T>` is an abstract base class helping orchestrate methods, functions (locals and anonymous) as well as accessors. This class will convert  from expression body to block body form if any mutation requires it. This class also implements `IInstrumentCode` and provides the adequate roll back logic for the compilation phase. It inherits from `MemberDefinitionOrchestrator<T>`.
Child classes must implement abstract methods that abstract some common operations:
- `ParameterList(T)`: must return the `ParameterListSyntax` instance listing all the provided method’s parameters. Returns an empty list if there are none.
- `ReturnType(T)`:  must return the `TypeSyntax` for the method’s returning type.  Should return `null` if the type cannot be determined (anonymous lambdas). One can use `RoslynHelper.VoidTypeSyntax()` if the method is void, explicitly or implicitly (constructors for example).
- `GetBodies(T)`: must return the body for the method. Return value is a tuple with the block body as first item and the expression body as the second. At least one of them should be null
- `SwitchToThisBodies(...)`: Must replace the method body with the provided body (either block or expression). This method is used to perform the expression to block body conversion and for roll back as well. Note that implementations assume one of the provided bodies is null (as a function cannot have both an expression body and block statement body;

Orchestrator’s overrides:
- `InjectMutations(...)`: if the body is in expression form and there are mutations that have not been injected (because they require statement or block level control), converts the body to block form and injects those mutations. If the body is in block form, injects out parameters initialization to default and adds an ending `return`  statement if it looks helpful.

Finally, this class implements an injection engine:
- `ConvertToBlockBody(...)` converts the method from expression form to block body form. Does nothing if the method has no expression body.
-  `RemoveInstrumentation(...)`: reverts the conversion. Note that this method requires that the body contains only a `return` or expression statement.
### Specialized orchestrators
These orchestrators are specific for certain syntax node types.
#### MemberAccessOrchestrator
`MemberAccessOrchestrator<T>` is used for ‘MemberAccess’ kind of expression, which is defined as a lower level than expression. This is because while those syntax constructs inherit from `ExpressionSyntax`, they cannot be replaced by other kinds of expression, which means that Stryker cannot use a conditional operator (`?:`) to mutate them in place; it must happen at a higher hierarchical level in the expression.
It is declared for `MemberAccessExpressionSyntax`, `MemberBindingExpressionSyntax` and `SimpleNameSyntax`.

This class implements:
- `CanHandle(...)`: returns true if the parent expression is either an invocation or member access expression.
- `PrepareContext(...)`: signals that this is a `MutantControl.MemberAccess` syntax level.
- `RestoreContext()`: leaves the current context.

#### DoNotMutateOrchestrator
`DoNotMutateOrchestrator<T>` is a generic class that be can used to ensure some type of `SyntaxNode` will not be mutated (including the node’s children). Its constructor accepts an optional predicate to confirm if a node should be mutated or not.
This class is used for several constructs:
- `AttributeListSyntax`: Attributes must be defined at compile time.
- `ParameterListSyntax`: Parameters and default values must be known at compile time.
- `EnumMemberSyntax`: Enumeration value must be known at compile time
-  `RecursivePatternSyntax`: Pattern syntax must be known at compile time.
-  `UsingDirectiveSyntax`: using directives are fixed and critical for compilation
- `FieldDeclarationSyntax`  (only const fields): cannot modify const fields at run time.
- `LocalDeclarationStatementSyntax` (only const): cannot modify constants at run time.

This class implements:
- `CanHandle(...)`: forwards the call to the predicate (if one was provided at construction), returns true otherwise.
- `Mutate(...)`: returns the unmodified syntax node.

#### InvocationExpressionOrchestrator
`InvocationExpressionOrchestrator` is used for methods and functions invocations (`xxxx(...)`).
This class implements:
- `PrepareContext(...)`: unless the invoked name is in the `MemberBindingExpression` form, I.e. starts with a dot `.` as those cannot be used in a condition expression. Then it declares a member access context.
- `RestoreContext()`: leaves the current context.
- `StoreMutations(...)`: Injects mutations at the current level, except if there are declarations in the arguments list. Then inject mutations at the next higher block level.
- `InjectMutation(...)`: it injects mutations normally in the current expression.

#### StaticFieldDeclarationOrchestrator
`StaicFieldDeclarationOrchestrator` is used for static fields in order to inject static marking logic used for coverage capture. It inherits from `MemberDefinitionOrchestrator`.
This class implements:
- `CanHandle(...)`: returns true for static fields with at least one initializer.
- `PrepareContext(...)`:  creates a static context.
- `InjectMutations(...)`: Actually injects mutations (call base implementation) then injects a static marker.

#### StaticConstructorOrchestrator
`StaticConstructorOrchestrator` is used for class constructors in order to inject static marking logic used for coverage capture. It inherits from `BasMethodOrchestrator`.
This class implements:
- `CanHandle(...)`: returns true for class constructors.
- `PrepareContext(...)`:  creates a static context.
- `InjectMutations(...)`: Actually injects mutations (call base implementation) then injects a static marker.

#### ExpressionBodiedPropertyOrchestrator
`ExpressionBodiedPropertyOrchestrator`  deals with properties expressed as simple expressions, such as 
`int MyProperty => 4;` 
or with static properties with an initializer, such as
`static int MyProperty  {get {...} set {...}} = MyInitialization(4);`
It inherits from `BaseFunctionOrchestrator` and implements required abstract methods.
This class implements:
- `OrchestrateChildrenMutation(...)`: ensure the node `Initializer` is mutated with a static context (if needed).
#### LocalFunctionOrchestrator
This class handles `LocalFunctionStatementSyntax`. It inherits from `BaseFunctionOrchestration` and implements all required abstract methods.
#### AnonymousFunctionExpressionOrchestrator
This class handles `AnonymousFunctionExpressionSyntax`. It inherits from `BaseFunctionOrchestration` and implements all required abstract methods.
#### BaseMethodDeclarationOrchestrator
This class handles `BaseMethodDeclarationSyntax`. It inherits from `BaseFunctionOrchestration` and implements all required abstract methods.
#### AccessorSyntaxOrchestrator
This class handles `AccessorDeclarationSyntax`. It inherits from `BaseFunctionOrchestration` and implements all required abstract methods.
#### LocalDeclarationOrchestrator
This class handles `LocalDeclarationStatement` (i.e. local variables declaration). It implements:
- `InjectMutations(...)`: it does not inject mutations, as they should be controlled at the scope level: using an `if` statement here would change the scope of the variable. As a consequence, mutations are controlled at the block level.

#### BlockOrchestrator
This class handles `BlockStatementSyntax` nodes. It implements:
- `PrepareContext(...)`: declares a **new** block context. Having a new block is used for scoped mutant filtering via Stryker comments
- `RestoreContext()`: restore the context
- `InjectMutations(...)`: injects mutations at the block level. That is the block is mutated and mutation switching is done via one or more `if` statements.
#### SyntaxNodeOrchestrator
This class handles any `SyntaxNode`. As such it provides the default behavior, that is no mutation. It implements:
- `GenerateMutationForNode(...)`: it does not try to generate mutations for this node and returns an empty list.

## Conclusion
Congratulations for reading this 5K words document to its end. I hope you found it useful, and most importantly, use it when working on this part of the project.
As usual, feedback and requests for clarifications are welcomed via the project GitHub repo.
