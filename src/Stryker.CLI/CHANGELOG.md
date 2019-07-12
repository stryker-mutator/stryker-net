# 0.12.0 (2019-07-01)


### Bug Fixes

* **cli:** Return exit code 1 when score is below threshold break value ([#154](https://github.com/stryker-mutator/stryker-net/issues/154)) ([f4a8419](https://github.com/stryker-mutator/stryker-net/commit/f4a8419))
* **config:** change config file is no longer required, existence is determined automatically ([#102](https://github.com/stryker-mutator/stryker-net/issues/102)) ([e516e88](https://github.com/stryker-mutator/stryker-net/commit/e516e88))
* **config:** Change configuration example (timeout-ms) ([#108](https://github.com/stryker-mutator/stryker-net/issues/108)) ([8a8952c](https://github.com/stryker-mutator/stryker-net/commit/8a8952c))
* **config:** Simplify CLI option creation ([#184](https://github.com/stryker-mutator/stryker-net/issues/184)) ([67d3b80](https://github.com/stryker-mutator/stryker-net/commit/67d3b80))
* **dependency-resolve:** add new targets file with nugetID as name ([#112](https://github.com/stryker-mutator/stryker-net/issues/112)) ([331910d](https://github.com/stryker-mutator/stryker-net/commit/331910d))
* **Missing mutations:** Improve mutation algorithm to find more mutants ([#373](https://github.com/stryker-mutator/stryker-net/issues/373)) ([63ef355](https://github.com/stryker-mutator/stryker-net/commit/63ef355))
* **nuget:** update project info for NuGet pack ([#45](https://github.com/stryker-mutator/stryker-net/issues/45)) ([d23a485](https://github.com/stryker-mutator/stryker-net/commit/d23a485))
* **Reporter names:** Improve reporter names ([#504](https://github.com/stryker-mutator/stryker-net/issues/504)) ([3648cbe](https://github.com/stryker-mutator/stryker-net/commit/3648cbe))
* **Tuple bug:** ExpressionStatement placed as ConditionalExpression ([#280](https://github.com/stryker-mutator/stryker-net/issues/280)) ([f74f782](https://github.com/stryker-mutator/stryker-net/commit/f74f782))
* merge conflicts ([4bdc62f](https://github.com/stryker-mutator/stryker-net/commit/4bdc62f))
* merge conflicts ([31c8d11](https://github.com/stryker-mutator/stryker-net/commit/31c8d11))
* String interpolation on mutation threshold hit output was incorrect causing values to not be interpolated ([#226](https://github.com/stryker-mutator/stryker-net/issues/226)) ([b343d17](https://github.com/stryker-mutator/stryker-net/commit/b343d17))
* system.memory dependency issue by bumping dotnet core version to 2.1 ([#245](https://github.com/stryker-mutator/stryker-net/issues/245)) ([3b35d90](https://github.com/stryker-mutator/stryker-net/commit/3b35d90)), closes [#234](https://github.com/stryker-mutator/stryker-net/issues/234)


### Features

* **Better error handling:** More understandable error messages ([#169](https://github.com/stryker-mutator/stryker-net/issues/169)) ([9214fbd](https://github.com/stryker-mutator/stryker-net/commit/9214fbd))
* **Compiling:** False positive detection ([#481](https://github.com/stryker-mutator/stryker-net/issues/481)) ([0918e22](https://github.com/stryker-mutator/stryker-net/commit/0918e22))
* **Conditional mutant placer:** Add ability to place mutations in ConditionalExpressions ([#207](https://github.com/stryker-mutator/stryker-net/issues/207)) ([e3e0433](https://github.com/stryker-mutator/stryker-net/commit/e3e0433))
* **config:** add --max-concurrent-test-runners config setting ([#133](https://github.com/stryker-mutator/stryker-net/issues/133)) ([f5395ae](https://github.com/stryker-mutator/stryker-net/commit/f5395ae)), closes [#111](https://github.com/stryker-mutator/stryker-net/issues/111)
* **config:** Add configFile support ([#96](https://github.com/stryker-mutator/stryker-net/issues/96)) ([c018dd4](https://github.com/stryker-mutator/stryker-net/commit/c018dd4))
* **coverage analysis:** Implement coverage analysis with vstest ([#506](https://github.com/stryker-mutator/stryker-net/issues/506)) ([3858bbc](https://github.com/stryker-mutator/stryker-net/commit/3858bbc))
* **dev-mode:** Improve mutant rollback (Fix [#236](https://github.com/stryker-mutator/stryker-net/issues/236)) ([#257](https://github.com/stryker-mutator/stryker-net/issues/257)) ([2f6f459](https://github.com/stryker-mutator/stryker-net/commit/2f6f459))
* **Dotnet Framework support:** Add initial support for dotnet full framework ([#371](https://github.com/stryker-mutator/stryker-net/issues/371)) ([93ca8a5](https://github.com/stryker-mutator/stryker-net/commit/93ca8a5)), closes [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#192](https://github.com/stryker-mutator/stryker-net/issues/192) [#389](https://github.com/stryker-mutator/stryker-net/issues/389) [#390](https://github.com/stryker-mutator/stryker-net/issues/390) [#398](https://github.com/stryker-mutator/stryker-net/issues/398) [#391](https://github.com/stryker-mutator/stryker-net/issues/391) [#407](https://github.com/stryker-mutator/stryker-net/issues/407) [#372](https://github.com/stryker-mutator/stryker-net/issues/372) [#418](https://github.com/stryker-mutator/stryker-net/issues/418)
* **Excluded mutations:** Add option to exlude one or more mutators ([#253](https://github.com/stryker-mutator/stryker-net/issues/253)) ([7033969](https://github.com/stryker-mutator/stryker-net/commit/7033969))
* **exclusions:** Warn user on all mutations excluded ([#416](https://github.com/stryker-mutator/stryker-net/issues/416)) ([8c45c7d](https://github.com/stryker-mutator/stryker-net/commit/8c45c7d))
* **File exclusion:** Add option to exclude files from mutation run ([#196](https://github.com/stryker-mutator/stryker-net/issues/196)) ([596f907](https://github.com/stryker-mutator/stryker-net/commit/596f907))
* **First implementation:** initial commit ([d4e06ec](https://github.com/stryker-mutator/stryker-net/commit/d4e06ec))
* **html-reporter:** Implement html reporter using mutation report html elements ([#352](https://github.com/stryker-mutator/stryker-net/issues/352)) ([417a2b7](https://github.com/stryker-mutator/stryker-net/commit/417a2b7))
* **installation:** Package stryker runner as dotnet tool ([#193](https://github.com/stryker-mutator/stryker-net/issues/193)) ([a3fd4a4](https://github.com/stryker-mutator/stryker-net/commit/a3fd4a4))
* **integrationtest:** Run stryker from nuget in integration test ([#239](https://github.com/stryker-mutator/stryker-net/issues/239)) ([4b21514](https://github.com/stryker-mutator/stryker-net/commit/4b21514))
* **json-reporter:** Add ability to generate mutation report in json format ([#284](https://github.com/stryker-mutator/stryker-net/issues/284)) ([a5b59c2](https://github.com/stryker-mutator/stryker-net/commit/a5b59c2))
* **logging:** add logLevel validation ([#124](https://github.com/stryker-mutator/stryker-net/issues/124)) ([c5960ca](https://github.com/stryker-mutator/stryker-net/commit/c5960ca)), closes [#97](https://github.com/stryker-mutator/stryker-net/issues/97)
* **logging:** Print version number below logo ([#246](https://github.com/stryker-mutator/stryker-net/issues/246)) ([7dc13a6](https://github.com/stryker-mutator/stryker-net/commit/7dc13a6))
* **Reporter:** rename "RapportOnly" to "ReporrtOnly" ([#123](https://github.com/stryker-mutator/stryker-net/issues/123)) ([6be7fe6](https://github.com/stryker-mutator/stryker-net/commit/6be7fe6)), closes [#95](https://github.com/stryker-mutator/stryker-net/issues/95)
* **speed:** Add test run multithreading ([#107](https://github.com/stryker-mutator/stryker-net/issues/107)) ([6897cc2](https://github.com/stryker-mutator/stryker-net/commit/6897cc2))
* **testing:** Add happyflow integration test ([#163](https://github.com/stryker-mutator/stryker-net/issues/163)) ([2b2f9ba](https://github.com/stryker-mutator/stryker-net/commit/2b2f9ba))
* **testrunner:** Integrate with vstest testrunner on windows ([#319](https://github.com/stryker-mutator/stryker-net/issues/319)) ([4a1422a](https://github.com/stryker-mutator/stryker-net/commit/4a1422a))
* replace camel case arguments with dashed ([#100](https://github.com/stryker-mutator/stryker-net/issues/100)) ([0f0f0b4](https://github.com/stryker-mutator/stryker-net/commit/0f0f0b4))
* replace logFile with log-level-file [#99](https://github.com/stryker-mutator/stryker-net/issues/99) ([8237a25](https://github.com/stryker-mutator/stryker-net/commit/8237a25))
* Support for timeouts ([#25](https://github.com/stryker-mutator/stryker-net/issues/25)) ([eb56899](https://github.com/stryker-mutator/stryker-net/commit/eb56899))
* **thresholds:** Add threshold arguments for the CLI ([#140](https://github.com/stryker-mutator/stryker-net/issues/140)) ([ef93cb9](https://github.com/stryker-mutator/stryker-net/commit/ef93cb9))
* **update checks:** Add version updates check ([#612](https://github.com/stryker-mutator/stryker-net/issues/612)) ([a6c940a](https://github.com/stryker-mutator/stryker-net/commit/a6c940a))


### BREAKING CHANGES

* **Reporter:** The `"RapportOnly"` reporter is now called `"ReportOnly"`.



# [0.11.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.10.0...0.11.0) (2019-05-10)


### Bug Fixes

* **Reporter names:** Improve reporter names ([#504](https://github.com/stryker-mutator/stryker-net/issues/504)) ([3648cbe](https://github.com/stryker-mutator/stryker-net/commit/3648cbe))


### Features

* **Compiling:** False positive detection ([#481](https://github.com/stryker-mutator/stryker-net/issues/481)) ([0918e22](https://github.com/stryker-mutator/stryker-net/commit/0918e22))



# [0.10.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.9.0...0.10.0) (2019-04-05)


### Features

* **Dotnet Framework support:** Add initial support for dotnet full framework ([#371](https://github.com/stryker-mutator/stryker-net/issues/371)) ([93ca8a5](https://github.com/stryker-mutator/stryker-net/commit/93ca8a5)), closes [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#192](https://github.com/stryker-mutator/stryker-net/issues/192) [#389](https://github.com/stryker-mutator/stryker-net/issues/389) [#390](https://github.com/stryker-mutator/stryker-net/issues/390) [#398](https://github.com/stryker-mutator/stryker-net/issues/398) [#391](https://github.com/stryker-mutator/stryker-net/issues/391) [#407](https://github.com/stryker-mutator/stryker-net/issues/407) [#372](https://github.com/stryker-mutator/stryker-net/issues/372) [#418](https://github.com/stryker-mutator/stryker-net/issues/418)
* **exclusions:** Warn user on all mutations excluded ([#416](https://github.com/stryker-mutator/stryker-net/issues/416)) ([8c45c7d](https://github.com/stryker-mutator/stryker-net/commit/8c45c7d))



# [0.9.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.8.3...0.9.0) (2019-03-09)


### Bug Fixes

* **Missing mutations:** Improve mutation algorithm to find more mutants ([#373](https://github.com/stryker-mutator/stryker-net/issues/373)) ([63ef355](https://github.com/stryker-mutator/stryker-net/commit/63ef355))


### Features

* **html-reporter:** Implement html reporter using mutation report html elements ([#352](https://github.com/stryker-mutator/stryker-net/issues/352)) ([417a2b7](https://github.com/stryker-mutator/stryker-net/commit/417a2b7))
* **testrunner:** Integrate with vstest testrunner on windows ([#319](https://github.com/stryker-mutator/stryker-net/issues/319)) ([4a1422a](https://github.com/stryker-mutator/stryker-net/commit/4a1422a))



## [0.8.3](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.8.2...0.8.3) (2019-02-07)



## [0.8.2](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.8.1...0.8.2) (2019-01-28)



## [0.8.1](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.8.0...0.8.1) (2019-01-25)



# [0.8.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.7.0...0.8.0) (2019-01-11)


### Bug Fixes

* **Tuple bug:** ExpressionStatement placed as ConditionalExpression ([#280](https://github.com/stryker-mutator/stryker-net/issues/280)) ([f74f782](https://github.com/stryker-mutator/stryker-net/commit/f74f782))


### Features

* **dev-mode:** Improve mutant rollback (Fix [#236](https://github.com/stryker-mutator/stryker-net/issues/236)) ([#257](https://github.com/stryker-mutator/stryker-net/issues/257)) ([2f6f459](https://github.com/stryker-mutator/stryker-net/commit/2f6f459))
* **Excluded mutations:** Add option to exlude one or more mutators ([#253](https://github.com/stryker-mutator/stryker-net/issues/253)) ([7033969](https://github.com/stryker-mutator/stryker-net/commit/7033969))
* **File exclusion:** Add option to exclude files from mutation run ([#196](https://github.com/stryker-mutator/stryker-net/issues/196)) ([596f907](https://github.com/stryker-mutator/stryker-net/commit/596f907))
* **json-reporter:** Add ability to generate mutation report in json format ([#284](https://github.com/stryker-mutator/stryker-net/issues/284)) ([a5b59c2](https://github.com/stryker-mutator/stryker-net/commit/a5b59c2))



# [0.7.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.6.0...0.7.0) (2018-11-28)


### Features

* **Better error handling:** More understandable error messages ([#169](https://github.com/stryker-mutator/stryker-net/issues/169)) ([9214fbd](https://github.com/stryker-mutator/stryker-net/commit/9214fbd))



# [0.6.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.5.0...0.6.0) (2018-11-19)


### Bug Fixes

* String interpolation on mutation threshold hit output was incorrect causing values to not be interpolated ([#226](https://github.com/stryker-mutator/stryker-net/issues/226)) ([b343d17](https://github.com/stryker-mutator/stryker-net/commit/b343d17))
* system.memory dependency issue by bumping dotnet core version to 2.1 ([#245](https://github.com/stryker-mutator/stryker-net/issues/245)) ([3b35d90](https://github.com/stryker-mutator/stryker-net/commit/3b35d90)), closes [#234](https://github.com/stryker-mutator/stryker-net/issues/234)


### Features

* **Conditional mutant placer:** Add ability to place mutations in ConditionalExpressions ([#207](https://github.com/stryker-mutator/stryker-net/issues/207)) ([e3e0433](https://github.com/stryker-mutator/stryker-net/commit/e3e0433))
* **integrationtest:** Run stryker from nuget in integration test ([#239](https://github.com/stryker-mutator/stryker-net/issues/239)) ([4b21514](https://github.com/stryker-mutator/stryker-net/commit/4b21514))
* **logging:** Print version number below logo ([#246](https://github.com/stryker-mutator/stryker-net/issues/246)) ([7dc13a6](https://github.com/stryker-mutator/stryker-net/commit/7dc13a6))



# [0.5.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.4.0...0.5.0) (2018-11-03)



<a name="0.4.0"></a>
# [0.4.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.3.0...0.4.0) (2018-10-28)


### Bug Fixes

* **cli:** Return exit code 1 when score is below threshold break value ([#154](https://github.com/stryker-mutator/stryker-net/issues/154)) ([f4a8419](https://github.com/stryker-mutator/stryker-net/commit/f4a8419))
* **config:** Simplify CLI option creation ([#184](https://github.com/stryker-mutator/stryker-net/issues/184)) ([67d3b80](https://github.com/stryker-mutator/stryker-net/commit/67d3b80))


### Features

* **testing:** Add happyflow integration test ([#163](https://github.com/stryker-mutator/stryker-net/issues/163)) ([2b2f9ba](https://github.com/stryker-mutator/stryker-net/commit/2b2f9ba))
* **reporting:** Set default log level to Info and cleanup reporters ([#167](https://github.com/stryker-mutator/stryker-net/issues/167)) ([b378262](https://github.com/stryker-mutator/stryker-net/commit/b378262))
* **installation:** Package stryker runner as dotnet tool ([#193](https://github.com/stryker-mutator/stryker-net/issues/193)) ([a3fd4a4](https://github.com/stryker-mutator/stryker-net/commit/a3fd4a4))


<a name="0.3.0"></a>
## [0.3.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.2.0...StrykerMutator.DotNetCoreCli@0.3.0) (2018-10-06)


**Note:** Version bump only for package StrykerMutator.DotNetCoreCli

<a name="0.2.0"></a>
# [0.2.0](https://github.com/stryker-mutator/stryker-net/compare/80742de...StrykerMutator.DotNetCoreCli@0.2.0) (2018-10-05)


### Bug Fixes

* **dependency-resolve:** add new targets file with nugetID as name ([#112](https://github.com/stryker-mutator/stryker-net/issues/112)) ([331910d](https://github.com/stryker-mutator/stryker-net/commit/331910d)), closes [#47](https://github.com/stryker-mutator/stryker-net/issues/47)
* **Logging:** Ensure all references of --project are now --project-file ([#144](https://github.com/stryker-mutator/stryker-net/issues/144)) ([7379e5d](https://github.com/stryker-mutator/stryker-net/commit/7379e5d)), closes [#142](https://github.com/stryker-mutator/stryker-net/issues/142)


### Features

* **Reporter:** rename "RapportOnly" to "reportOnly" ([#123](https://github.com/stryker-mutator/stryker-net/issues/123)) ([6be7fe6](https://github.com/stryker-mutator/stryker-net/commit/6be7fe6)), closes [#95](https://github.com/stryker-mutator/stryker-net/issues/95)
* **Logging:** add logLevel validation ([#124](https://github.com/stryker-mutator/stryker-net/issues/124)) ([c5960ca](https://github.com/stryker-mutator/stryker-net/commit/c5960ca)), closes [#97](https://github.com/stryker-mutator/stryker-net/issues/97)
* replace camel case arguments with dashed ([#129](https://github.com/stryker-mutator/stryker-net/issues/129)) ([0f0f0b4](https://github.com/stryker-mutator/stryker-net/commit/0f0f0b4)), closes [#100](https://github.com/stryker-mutator/stryker-net/issues/100)
* replace logFile with log-level-file ([#132](https://github.com/stryker-mutator/stryker-net/issues/132)) ([8237a25](https://github.com/stryker-mutator/stryker-net/commit/8237a25)), closes [#99](https://github.com/stryker-mutator/stryker-net/issues/99)
* **Config:** add --max-concurrent-test-runners config setting ([#133](https://github.com/stryker-mutator/stryker-net/issues/133)) ([f5395ae](https://github.com/stryker-mutator/stryker-net/commit/f5395ae)), closes [#111](https://github.com/stryker-mutator/stryker-net/issues/111)
* **Threshold:** add threshold arguments for the CLI  ([#140](https://github.com/stryker-mutator/stryker-net/issues/140)) ([ef93cb9](https://github.com/stryker-mutator/stryker-net/commit/ef93cb9)), closes [#11](https://github.com/stryker-mutator/stryker-net/issues/11)


### BREAKING CHANGES:
* The `"RapportOnly"` reporter is now called `"ReportOnly"`.
* Renamed `"timeoutMS"` config property to `"timeout-ms"`
* Renamed `"logConsole"` config property to `"log-console"`
* Renamed `"logFile"` config property to `"log-level-file"`
* Renamed `"projectName"` config property to `"project-file"`
* Renamed `"projectName"` cli option to `"project-file"`
* Renamed `"configFilePath"` config property to `"config-file-path"`