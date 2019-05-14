# [0.11.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.10.0...0.11.0) (2019-05-10)


### Bug Fixes

* **Project File Analyzer:** Improve solution path error message ([#460](https://github.com/stryker-mutator/stryker-net/issues/460)) ([ba5e4a2](https://github.com/stryker-mutator/stryker-net/commit/ba5e4a2))
* **Reporter names:** Improve reporter names ([#504](https://github.com/stryker-mutator/stryker-net/issues/504)) ([3648cbe](https://github.com/stryker-mutator/stryker-net/commit/3648cbe))
* **Rollback failed logging:** Improve error messages for rollback process ([#478](https://github.com/stryker-mutator/stryker-net/issues/478)) ([b1dd875](https://github.com/stryker-mutator/stryker-net/commit/b1dd875))
* **shared projects:** Only include .projitems files in shared projects import ([#479](https://github.com/stryker-mutator/stryker-net/issues/479)) ([8f21ce1](https://github.com/stryker-mutator/stryker-net/commit/8f21ce1))


### Features

* **compiling:** Compile assembly version information ([#482](https://github.com/stryker-mutator/stryker-net/issues/482)) ([f813f25](https://github.com/stryker-mutator/stryker-net/commit/f813f25))
* **Compiling:** False positive detection ([#481](https://github.com/stryker-mutator/stryker-net/issues/481)) ([0918e22](https://github.com/stryker-mutator/stryker-net/commit/0918e22))
* **mutations:** negate mutator ([#451](https://github.com/stryker-mutator/stryker-net/issues/451)) ([a4630d0](https://github.com/stryker-mutator/stryker-net/commit/a4630d0))
* **report json:** support html tags in source code and update html report to 1.0.5 ([#443](https://github.com/stryker-mutator/stryker-net/issues/443)) ([4b64821](https://github.com/stryker-mutator/stryker-net/commit/4b64821))
* **rollback mutations:** Recompile 100 times ([#473](https://github.com/stryker-mutator/stryker-net/issues/473)) ([4530ff5](https://github.com/stryker-mutator/stryker-net/commit/4530ff5))
* **Xamarin support:** Run stryker.net on xamarin projects ([#488](https://github.com/stryker-mutator/stryker-net/issues/488)) ([d2b6010](https://github.com/stryker-mutator/stryker-net/commit/d2b6010))



# [0.10.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.9.0...0.10.0) (2019-04-05)


### Bug Fixes

* **Handeling statements:** ForStatements and ExpressionStatement mutated correctly ([#413](https://github.com/stryker-mutator/stryker-net/issues/413)) ([e1a654a](https://github.com/stryker-mutator/stryker-net/commit/e1a654a)), closes [#411](https://github.com/stryker-mutator/stryker-net/issues/411) [#412](https://github.com/stryker-mutator/stryker-net/issues/412)


### Features

* **Dotnet Framework support:** Add initial support for dotnet full framework ([#371](https://github.com/stryker-mutator/stryker-net/issues/371)) ([93ca8a5](https://github.com/stryker-mutator/stryker-net/commit/93ca8a5)), closes [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#192](https://github.com/stryker-mutator/stryker-net/issues/192) [#389](https://github.com/stryker-mutator/stryker-net/issues/389) [#390](https://github.com/stryker-mutator/stryker-net/issues/390) [#398](https://github.com/stryker-mutator/stryker-net/issues/398) [#391](https://github.com/stryker-mutator/stryker-net/issues/391) [#407](https://github.com/stryker-mutator/stryker-net/issues/407) [#372](https://github.com/stryker-mutator/stryker-net/issues/372) [#418](https://github.com/stryker-mutator/stryker-net/issues/418)
* **exclusions:** Warn user on all mutations excluded ([#416](https://github.com/stryker-mutator/stryker-net/issues/416)) ([8c45c7d](https://github.com/stryker-mutator/stryker-net/commit/8c45c7d))
* **input file resolver:** Warn user when more than one csproj is found in working directory ([#426](https://github.com/stryker-mutator/stryker-net/issues/426)) ([7404fbd](https://github.com/stryker-mutator/stryker-net/commit/7404fbd))



# [0.9.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.3...0.9.0) (2019-03-09)


### Bug Fixes

* **Declaration pattern:** Support DeclarationPattern ([#350](https://github.com/stryker-mutator/stryker-net/issues/350)) ([9490e90](https://github.com/stryker-mutator/stryker-net/commit/9490e90))
* **Double mutations:** On invocation expressions ([#397](https://github.com/stryker-mutator/stryker-net/issues/397)) ([c76e2c5](https://github.com/stryker-mutator/stryker-net/commit/c76e2c5))
* **json-reporter:** Update json report to be compliant with schema version 0.0.4 ([#347](https://github.com/stryker-mutator/stryker-net/issues/347)) ([9035407](https://github.com/stryker-mutator/stryker-net/commit/9035407))
* **linq mutation:** Remove Linq query removal mutation ([#341](https://github.com/stryker-mutator/stryker-net/issues/341)) ([2ef1964](https://github.com/stryker-mutator/stryker-net/commit/2ef1964))
* **Missing mutations:** Improve mutation algorithm to find more mutants ([#373](https://github.com/stryker-mutator/stryker-net/issues/373)) ([63ef355](https://github.com/stryker-mutator/stryker-net/commit/63ef355))
* **mutating:** Improved mutation algorithm ([#348](https://github.com/stryker-mutator/stryker-net/issues/348)) ([e844e34](https://github.com/stryker-mutator/stryker-net/commit/e844e34))
* **mutating:** Skip conditions containing a declaration from mutating ([#338](https://github.com/stryker-mutator/stryker-net/issues/338)) ([1e3a274](https://github.com/stryker-mutator/stryker-net/commit/1e3a274))


### Features

* **html-reporter:** Implement html reporter using mutation report html elements ([#352](https://github.com/stryker-mutator/stryker-net/issues/352)) ([417a2b7](https://github.com/stryker-mutator/stryker-net/commit/417a2b7))
* **testrunner:** Integrate with vstest testrunner on windows ([#319](https://github.com/stryker-mutator/stryker-net/issues/319)) ([4a1422a](https://github.com/stryker-mutator/stryker-net/commit/4a1422a))



## [0.8.3](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.2...0.8.3) (2019-02-07)


### Bug Fixes

* **mutating:** If statements are mutated nested ([#336](https://github.com/stryker-mutator/stryker-net/issues/336)) ([dbfe16f](https://github.com/stryker-mutator/stryker-net/commit/dbfe16f))



## [0.8.2](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.1...0.8.2) (2019-01-28)


### Bug Fixes

* **local functions:** Local functions are mutated correctly ([#325](https://github.com/stryker-mutator/stryker-net/issues/325)) ([2670572](https://github.com/stryker-mutator/stryker-net/commit/2670572))
* **rollback mutation:** Rollback two times instead of one in order to catch all compilation errors ([#323](https://github.com/stryker-mutator/stryker-net/issues/323)) ([00efe25](https://github.com/stryker-mutator/stryker-net/commit/00efe25))



## [0.8.1](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.0...0.8.1) (2019-01-25)


### Bug Fixes

* **C# 7.1 or greater:** Make Stryker compile all language features ([#311](https://github.com/stryker-mutator/stryker-net/issues/311)) ([25119e9](https://github.com/stryker-mutator/stryker-net/commit/25119e9))



# [0.8.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.7.0...0.8.0) (2019-01-11)


### Bug Fixes

* **Tuple bug:** ExpressionStatement placed as ConditionalExpression ([#280](https://github.com/stryker-mutator/stryker-net/issues/280)) ([f74f782](https://github.com/stryker-mutator/stryker-net/commit/f74f782))
* **UnauthorizedAccessException:** AppendTargetFrameworkToOutputPath csproj property is included ([#295](https://github.com/stryker-mutator/stryker-net/issues/295)) ([8a0b634](https://github.com/stryker-mutator/stryker-net/commit/8a0b634)), closes [#272](https://github.com/stryker-mutator/stryker-net/issues/272)


### Features

* **dev-mode:** Improve mutant rollback (Fix [#236](https://github.com/stryker-mutator/stryker-net/issues/236)) ([#257](https://github.com/stryker-mutator/stryker-net/issues/257)) ([2f6f459](https://github.com/stryker-mutator/stryker-net/commit/2f6f459))
* **Excluded mutations:** Add option to exlude one or more mutators ([#253](https://github.com/stryker-mutator/stryker-net/issues/253)) ([7033969](https://github.com/stryker-mutator/stryker-net/commit/7033969))
* **File exclusion:** Add option to exclude files from mutation run ([#196](https://github.com/stryker-mutator/stryker-net/issues/196)) ([596f907](https://github.com/stryker-mutator/stryker-net/commit/596f907))
* **json-reporter:** Add ability to generate mutation report in json format ([#284](https://github.com/stryker-mutator/stryker-net/issues/284)) ([a5b59c2](https://github.com/stryker-mutator/stryker-net/commit/a5b59c2))
* **No timeouts due to computer going in sleep mode:** Secure against sleep induced timeouts ([#249](https://github.com/stryker-mutator/stryker-net/issues/249)) ([69a0560](https://github.com/stryker-mutator/stryker-net/commit/69a0560))
* **unsafe blocks:** recompile with unsafe=true ([#306](https://github.com/stryker-mutator/stryker-net/issues/306)) ([c9c913b](https://github.com/stryker-mutator/stryker-net/commit/c9c913b))



# [0.7.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.6.0...0.7.0) (2018-11-28)


### Bug Fixes

* **LinqMutator:** Linq mutator mutates only method invocations ([#252](https://github.com/stryker-mutator/stryker-net/issues/252)) ([5167997](https://github.com/stryker-mutator/stryker-net/commit/5167997))


### Features

* **Better error handling:** More understandable error messages ([#169](https://github.com/stryker-mutator/stryker-net/issues/169)) ([9214fbd](https://github.com/stryker-mutator/stryker-net/commit/9214fbd))
* **mutant placing:** Improve mutants performance ([#247](https://github.com/stryker-mutator/stryker-net/issues/247)) ([e2a6182](https://github.com/stryker-mutator/stryker-net/commit/e2a6182))
* **Shared projects:** Add support for shared projects ([#235](https://github.com/stryker-mutator/stryker-net/issues/235)) ([8304f2c](https://github.com/stryker-mutator/stryker-net/commit/8304f2c))



# [0.6.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.5.0...0.6.0) (2018-11-19)


### Bug Fixes

* **logging:** Clean up oneline logger output by removing timestamp ([#227](https://github.com/stryker-mutator/stryker-net/issues/227)) ([769509e](https://github.com/stryker-mutator/stryker-net/commit/769509e))
* system.memory dependency issue by bumping dotnet core version to 2.1 ([#245](https://github.com/stryker-mutator/stryker-net/issues/245)) ([3b35d90](https://github.com/stryker-mutator/stryker-net/commit/3b35d90)), closes [#234](https://github.com/stryker-mutator/stryker-net/issues/234)


### Features

* **Conditional mutant placer:** Add ability to place mutations in ConditionalExpressions ([#207](https://github.com/stryker-mutator/stryker-net/issues/207)) ([e3e0433](https://github.com/stryker-mutator/stryker-net/commit/e3e0433))
* **integrationtest:** Run stryker from nuget in integration test ([#239](https://github.com/stryker-mutator/stryker-net/issues/239)) ([4b21514](https://github.com/stryker-mutator/stryker-net/commit/4b21514))



# [0.5.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.4.0...0.5.0) (2018-11-03)



<a name="0.4.0"></a>
# [0.4.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.3.0...0.4.0) (2018-10-28)


### Bug Fixes

* **cli:** Return exit code 1 when score is below threshold break value ([#154](https://github.com/stryker-mutator/stryker-net/issues/154)) ([f4a8419](https://github.com/stryker-mutator/stryker-net/commit/f4a8419))
* **ConsoleReporter:** use 1-based line indexing ([#176](https://github.com/stryker-mutator/stryker-net/issues/176)) ([c302fe2](https://github.com/stryker-mutator/stryker-net/commit/c302fe2))
* **mutating:** Make sure a broken mutation does not exist twice, so that we can roll back the mutation ([#145](https://github.com/stryker-mutator/stryker-net/issues/145)) ([#190](https://github.com/stryker-mutator/stryker-net/issues/190)) ([1f74cce](https://github.com/stryker-mutator/stryker-net/commit/1f74cce))
* **test:** Run unit tests on all platforms ([#197](https://github.com/stryker-mutator/stryker-net/issues/197)) ([a4c27f0](https://github.com/stryker-mutator/stryker-net/commit/a4c27f0))


### Features

* **logging:** Show number of tests found in initial run ([#138](https://github.com/stryker-mutator/stryker-net/issues/138)) ([57f5f08](https://github.com/stryker-mutator/stryker-net/commit/57f5f08))
* **mutators:** Add Mutations for LINQ Expressions ([#185](https://github.com/stryker-mutator/stryker-net/issues/185)) ([5ae9d3a](https://github.com/stryker-mutator/stryker-net/commit/5ae9d3a))
* **mutators:** Add string and interpolated string mutators ([#194](https://github.com/stryker-mutator/stryker-net/issues/194)) ([653f159](https://github.com/stryker-mutator/stryker-net/commit/653f159))
* **testing:** Add happyflow integration test ([#163](https://github.com/stryker-mutator/stryker-net/issues/163)) ([2b2f9ba](https://github.com/stryker-mutator/stryker-net/commit/2b2f9ba))
* **reporting:** Set default log level to Info and cleanup reporters ([#167](https://github.com/stryker-mutator/stryker-net/issues/167)) ([b378262](https://github.com/stryker-mutator/stryker-net/commit/b378262))



<a name="0.3.0"></a>
# [0.3.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.2.0...StrykerMutator.Core@0.3.0) (2018-10-06)


### Bug Fixes
* Ensure directories/files don't exist before creating them ([#157](https://github.com/stryker-mutator/stryker-net/issues/157)) ([d4d2497](https://github.com/stryker-mutator/stryker-net/commit/d4d2497)), closes [#155](https://github.com/stryker-mutator/stryker-net/issues/155)
* **dependency-resolver:** Support backslash in ProjectReferences on Linux ([#149](https://github.com/stryker-mutator/stryker-net/issues/149)) ([223e841](https://github.com/stryker-mutator/stryker-net/commit/223e841)), closes [#120](https://github.com/stryker-mutator/stryker-net/issues/120)


### Features

* **Build:** Add multiple framework support ([#147](https://github.com/stryker-mutator/stryker-net/issues/147)) ([9f5233a](https://github.com/stryker-mutator/stryker-net/commit/9f5233a)), closes [#130](https://github.com/stryker-mutator/stryker-net/issues/130)


<a name="0.2.0"></a>
# [0.2.0](https://github.com/stryker-mutator/stryker-net/compare/80742de...StrykerMutator.Core@0.2.0) (2018-10-05)


### Bug Fixes

* **TestRunner:** remove race condition during testing ([#139](https://github.com/stryker-mutator/stryker-net/issues/139)) ([7c2d476](https://github.com/stryker-mutator/stryker-net/commit/7c2d476)), closes [#135](https://github.com/stryker-mutator/stryker-net/issues/135)
* **Logging:** Ensure all references of --project are now --project-file ([#144](https://github.com/stryker-mutator/stryker-net/issues/144)) ([7379e5d](https://github.com/stryker-mutator/stryker-net/commit/7379e5d)), closes [#142](https://github.com/stryker-mutator/stryker-net/issues/142)


### Code Refactoring

* merge interfaces and implementation ([#126](https://github.com/stryker-mutator/stryker-net/issues/126)) ([b55988e](https://github.com/stryker-mutator/stryker-net/commit/b55988e)), closes [#8](https://github.com/stryker-mutator/stryker-net/issues/8)


### Features

* **Reporter:** rename "RapportOnly" to "reportOnly" ([#123](https://github.com/stryker-mutator/stryker-net/issues/123)) ([6be7fe6](https://github.com/stryker-mutator/stryker-net/commit/6be7fe6)), closes [#95](https://github.com/stryker-mutator/stryker-net/issues/95)
* **Logging:** add logLevel validation ([#124](https://github.com/stryker-mutator/stryker-net/issues/124)) ([c5960ca](https://github.com/stryker-mutator/stryker-net/commit/c5960ca)), closes [#97](https://github.com/stryker-mutator/stryker-net/issues/97)
* **Config:** add --max-concurrent-test-runners config setting ([#133](https://github.com/stryker-mutator/stryker-net/issues/133)) ([f5395ae](https://github.com/stryker-mutator/stryker-net/commit/f5395ae)), closes [#111](https://github.com/stryker-mutator/stryker-net/issues/111)
* **Threshold:** add threshold arguments for the CLI  ([#140](https://github.com/stryker-mutator/stryker-net/issues/140)) ([ef93cb9](https://github.com/stryker-mutator/stryker-net/commit/ef93cb9)), closes [#11](https://github.com/stryker-mutator/stryker-net/issues/11)


### BREAKING CHANGES:
* The `"RapportOnly"` reporter is now called `"ReportOnly"`.