# Change Log

All notable changes will be documented in this file.

For historical release notes prior to the consolidated changelog, see the package-specific files:

* [Historical change log for stryker](/src/Stryker.Core/CHANGELOG.md)
* [Historical change log for dotnet-stryker](/src/Stryker.CLI/CHANGELOG.md)

<!-- changelog -->

## [4.12.3](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.14.1...dotnet-stryker@4.12.3) (2026-05-17)


### Bug Fixes

* **mtp:** attribute error/timed-out/cancelled states as failures ([#3560](https://github.com/stryker-mutator/stryker-net/issues/3560)) ([58969ca](https://github.com/stryker-mutator/stryker-net/commit/58969ca79a816587945141383309adbe60de6778)), closes [#3094](https://github.com/stryker-mutator/stryker-net/issues/3094)
* **mtp:** Disable DiffEngine and the Verify clipboard feature  ([#3547](https://github.com/stryker-mutator/stryker-net/issues/3547)) ([f7b2f20](https://github.com/stryker-mutator/stryker-net/commit/f7b2f20a2e49ef9b7888d783b0162b4c3c0f4159))
* **mutating:** out var in lambdas triggering NRE ([#3575](https://github.com/stryker-mutator/stryker-net/issues/3575)) ([5ce07cc](https://github.com/stryker-mutator/stryker-net/commit/5ce07cc505e33e05e631c4999b645ffba09ec53e))
* **rollback:** Improve rollback for CS0165 and CS0177 compile errors  ([#3548](https://github.com/stryker-mutator/stryker-net/issues/3548)) ([b42be0e](https://github.com/stryker-mutator/stryker-net/commit/b42be0e82569b539512632d869f7f2dda259eb84))

## [4.14.1](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.14.0...dotnet-stryker@4.14.1) (2026-04-10)


### Bug Fixes

* **baseline:** S3 Provider Cleanup ([#3491](https://github.com/stryker-mutator/stryker-net/issues/3491)) ([4a40c42](https://github.com/stryker-mutator/stryker-net/commit/4a40c42ded7ffdd343f991388ddc4da111ee9d04))
* handle null Messages in TestRunAccumulator.Aggregate ([#3513](https://github.com/stryker-mutator/stryker-net/issues/3513)) ([58a89b7](https://github.com/stryker-mutator/stryker-net/commit/58a89b73236e53e3419bc91fa1f36402c69835c7)), closes [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510) [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510) [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510)
