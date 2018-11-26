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