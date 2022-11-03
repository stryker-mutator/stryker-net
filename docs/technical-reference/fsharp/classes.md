---
title: Classes
sidebar_position: 10
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/technical-reference/fsharp/classes.md
---

The Following Classes need extra context to explain the current workings.
Not every feature of C# has been ported to F#.

### FsharpProjectComponentsBuilder

The current ProjectComponentsBuilder for F# does **not** have support for the following:
   * mutanthelpers injection
   * auto generated code detection

The ```FindProjectFilesScanningProjectFolders``` method (the backup for if buildalyzer doesn't work) has not been tested.
This was done because the ```FindProjectFilesScanningProjectFolders``` has been [deprecated] so any development would be a waste.

---

### FsharpCoreOrchestrator

The Orchestrators works in a similar fashion to those in the CsharpMutantOrchestrator.
This process starts with a method: ```Mutate(FSharpList<SynModuleOrNamespace>)``` inside the ```FsharpMutantOrchestrator```.
The ```FsharpMutantOrchestrator``` is only used as a starting point, after that the ```FsharpCoreOrchestrator``` takes over.

Inside the 'Core' is the actual process. 
This works by defining a Mutate() method which handles the iteration over a specific part of a SyntaxTree. (SynExpr for example)

Inside the Mutate method (contained in ```FsharpCoreOrchestrator```) we look for a specific Orchestrator in a Dictionary belonging to the type that the Mutate function handles. (the dictionary gets filled like this: ```_fsharpMutationsSynExpr.Add(typeof(SynExpr.Match), new MatchOrchestrator());```)
Once a specific orchestrator class has been found we initialize it and use it to iterate further.
The Orchestrator itself defines how to iterate further.

---

### FsharpMutationProcess

**Context:**

Whilst implementing the Compile function we ran into some limitations. 
There is a flag (lastcompiled) that should be set on the last file in the compilation sequence. 
During our testing this flag remained unset and the compiler failed.

**Effect:**

To solve this the flag get hardcoded on the file named ```Program.fs```,
this is because ```Program.fs``` is the last file in the project used to test the compile method.

This is also the reason the orchestrator works with ```FSharpList<SynModuleOrNamespace>``` and not ```ParsedInput```.
Because we need to set the flag on the ParsedInput.
Therefore making it easier to use the ```FSharpList<SynModuleOrNamespace>``` until we ave set the flag.
