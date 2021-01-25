---
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/fsharp/Classes.md
---

FsharpProjectComponentsBuilder	
	no mutanthelpers injection
	no auto generated code detection
	FindProjectFilesScanningProjectFolders (the backup for if buildalyzer doesn't work) has not been tested, and is considered WIP
FsharpCoreOrchestrator
	basic workings
FsharpMutationProcess
	lastcompiled flag
	-> reason that the orchestrator works with FSharpList<SynModuleOrNamespace> != ParsedInput
