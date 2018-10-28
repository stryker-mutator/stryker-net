<a name="0.4.0"></a>
# [0.4.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.DotNetCoreCli@0.3.0...0.4.0) (2018-10-28)


### Bug Fixes

* **cli:** Return exit code 1 when score is below threshold break value ([#154](https://github.com/stryker-mutator/stryker-net/issues/154)) ([f4a8419](https://github.com/stryker-mutator/stryker-net/commit/f4a8419))
* **config:** Simplify CLI option creation ([#184](https://github.com/stryker-mutator/stryker-net/issues/184)) ([67d3b80](https://github.com/stryker-mutator/stryker-net/commit/67d3b80))


### Features

* **testing:** Add happyflow integration test ([#163](https://github.com/stryker-mutator/stryker-net/issues/163)) ([2b2f9ba](https://github.com/stryker-mutator/stryker-net/commit/2b2f9ba))



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