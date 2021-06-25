---
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/technical-reference/fsharp/Packages.md
---

During the development of Stryker.net for F# packages have been added to Stryker.net.
Here follow a description of those packages and how they are used in Stryker.net

---

### FSharp.Compiler.Service
* **FSharp.Compiler.SyntaxTree**
  * SynModuleDecl
    * Let
    * NestedModule
  * SynExpr
    * Match
    * LetOrUse
    * IfThenElse
  * SynConst
  * SynPat
  * ParsedInput

These classes describe the structure of the SyntaxTree for F#, the name for the SyntaxTree in F# is ParsedInput.
The other classes are used for the structure of said ParsedInput.

Walking over a ParsedInput: https://fsharp.github.io/FSharp.Compiler.Service/untypedtree.html  
Viewing the ParsedInput online: https://fsprojects.github.io/fantomas-tools/#/ast

* **FSharp.Compiler.SourceCodeServices**
  * FSharpChecker  
*The FSharpChecker contains a lot of logic needed for F# support, compiling for example. Currently it is used for: Generating FSharpProjectOptions and FSharpParseFileResults as well as compiling the F# SyntaxTrees*
  * FSharpErrorInfo  
*The error format for F# operations, used by the compiler and whilst generating the FSharpProjectOptions*
  * FSharpProjectOptions  
*Future-proofing needed for a lot of parts within the FSharpChecker, used to get FSharpParseFileResults*
  * FSharpParseFileResults  
*The parsing result of the function which creates an ParsedInput from sourcecode (string)*

* **FSharp.Compiler.Text**
  * SourceText  
  *The SourceText Class contains the sourcecode from a given F# file, used to generate the initial SyntaxTrees.*

---

### Microsoft.FSharp
* Microsoft.FSharp.Collections
  * FSharpList
  * ListModule.OfSeq  
  *Used to convert the standard C# List to FSharpList and back. FSharpList is expected by the functions from FSharp.Compiler.Service*

* Microsoft.FSharp.Control
  * FSharpAsync.RunSynchronously  
  *Some of the functions in FSharp.Compiler.Service are Async, we have to specify that the process comes from F# for it to work correctly, hence we use FSharpAsync.RunSynchronously*

---

### FSharp.Core
needed if we want the compile function to work. Since it checks for the local version of Fsharp.Compile.

---
