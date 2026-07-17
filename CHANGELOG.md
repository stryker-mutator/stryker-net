# Change Log

All notable changes will be documented in this file.

For historical release notes prior to the consolidated changelog, see the package-specific files:

* [Historical change log for stryker](/src/Stryker.Core/CHANGELOG.md)
* [Historical change log for dotnet-stryker](/src/Stryker.CLI/CHANGELOG.md)

<!-- changelog -->

# [4.16.0](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.15.0...dotnet-stryker@4.16.0) (2026-07-03)


### Bug Fixes

* **dashboard:** do not delete the dashboard report ([#3669](https://github.com/stryker-mutator/stryker-net/issues/3669)) ([62baf08](https://github.com/stryker-mutator/stryker-net/commit/62baf08074f20f1d1e108bebf1bb31fddbb7c4ac)), closes [#2563](https://github.com/stryker-mutator/stryker-net/issues/2563)


### Features

* **no-logo:** Skip printing logo with low verbosity ([#3647](https://github.com/stryker-mutator/stryker-net/issues/3647)) ([d8a1c04](https://github.com/stryker-mutator/stryker-net/commit/d8a1c04b6295b92545ce98d3f60fc50b8d01ddb4))
* **reporters:** Add GitLab Code Quality reporter ([#3660](https://github.com/stryker-mutator/stryker-net/issues/3660)) ([0d17830](https://github.com/stryker-mutator/stryker-net/commit/0d17830cc81d793ed59276ba610aaa5ce6c05123))

# [4.15.0](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.14.2...dotnet-stryker@4.15.0) (2026-06-22)


### Bug Fixes

* **mtp:** Add StackOverflowException handling in MTP testrunner ([#3650](https://github.com/stryker-mutator/stryker-net/issues/3650)) ([297d88c](https://github.com/stryker-mutator/stryker-net/commit/297d88cc935ab069bf95982043cb692bd7aea0b6))
* **mtp:** Kill test processes on interuption ([#3617](https://github.com/stryker-mutator/stryker-net/issues/3617)) ([7e61f27](https://github.com/stryker-mutator/stryker-net/commit/7e61f2794b0a2159249c00e12af623f669afee1a))


### Features

* **compilation:** Improve code generation support  ([#3611](https://github.com/stryker-mutator/stryker-net/issues/3611)) ([8e7f606](https://github.com/stryker-mutator/stryker-net/commit/8e7f606b776de4b314e1d3d2bec89cb860b68bca))
* Retrieve actual project's language version when not configured ([#3642](https://github.com/stryker-mutator/stryker-net/issues/3642)) ([c706801](https://github.com/stryker-mutator/stryker-net/commit/c70680138227697a0061bdc7dc9fd19060984f5e))

## [4.14.2](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.14.1...dotnet-stryker@4.14.2) (2026-05-17)


### Bug Fixes

* **mtp:** attribute error/timed-out/cancelled states as failures ([#3560](https://github.com/stryker-mutator/stryker-net/issues/3560)) ([58969ca](https://github.com/stryker-mutator/stryker-net/commit/58969ca79a816587945141383309adbe60de6778)), closes [#3094](https://github.com/stryker-mutator/stryker-net/issues/3094)
* **mtp:** Disable DiffEngine and the Verify clipboard feature  ([#3547](https://github.com/stryker-mutator/stryker-net/issues/3547)) ([f7b2f20](https://github.com/stryker-mutator/stryker-net/commit/f7b2f20a2e49ef9b7888d783b0162b4c3c0f4159))
* **mutating:** out var in lambdas triggering NRE ([#3575](https://github.com/stryker-mutator/stryker-net/issues/3575)) ([5ce07cc](https://github.com/stryker-mutator/stryker-net/commit/5ce07cc505e33e05e631c4999b645ffba09ec53e))
* **rollback:** Improve rollback for CS0165 and CS0177 compile errors  ([#3548](https://github.com/stryker-mutator/stryker-net/issues/3548)) ([b42be0e](https://github.com/stryker-mutator/stryker-net/commit/b42be0e82569b539512632d869f7f2dda259eb84))

## [4.14.1](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.14.0...dotnet-stryker@4.14.1) (2026-04-10)


### Bug Fixes

* **baseline:** S3 Provider Cleanup ([#3491](https://github.com/stryker-mutator/stryker-net/issues/3491)) ([4a40c42](https://github.com/stryker-mutator/stryker-net/commit/4a40c42ded7ffdd343f991388ddc4da111ee9d04))
* handle null Messages in TestRunAccumulator.Aggregate ([#3513](https://github.com/stryker-mutator/stryker-net/issues/3513)) ([58a89b7](https://github.com/stryker-mutator/stryker-net/commit/58a89b73236e53e3419bc91fa1f36402c69835c7)), closes [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510) [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510) [#3510](https://github.com/stryker-mutator/stryker-net/issues/3510)
