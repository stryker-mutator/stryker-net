---
title: Current state
sidebar_position: 20
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/technical-reference/fsharp/current-state.md
---

The following have been added to secure Fsharp support in the future:

Code:
 * FsharpCompilingProcess.cs  
 *used to compile fsharp SyntaxTrees*
 * Fsharp variant of the ProjectComponents  
 *used to simulate a file structure in Stryker, and keep track of files in general*
 * FsharpProjectComponentsBuilder.cs  
 *used to instantiate the ProjectComponents for F#*
 * a set of classes to create Orchestrators for F# SyntaxTrees (ParsedInput)  
 *used to iterate over a given SyntaxTree*
 * FsharpMutationProcess.cs 
 *added to connect the different processes*
 * MutationTestProcess.cs 
 *used to start the FsharpMutationProcess.cs*
 
---

Packages:
* FSharp.Compiler.Service
* Microsoft.FSharp
* FSharp.Core

--- 

Other:
 * a .fs file to  use for the creation of tests for the F#-components
 * a test project written in F# to use as input for Stryker.net
 * a C# project use to test the process of iterating on a SyntaxTree in F#
 * general cleanup of existing code
