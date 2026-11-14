# Change Log

All notable changes will be documented in this file.

For historical release notes prior to the consolidated changelog, see the package-specific files:

* [Historical change log for stryker](/src/Stryker.Core/CHANGELOG.md)
* [Historical change log for dotnet-stryker](/src/Stryker.CLI/CHANGELOG.md)

<!-- changelog -->

# [5.1.0](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@5.0.0...dotnet-stryker@5.1.0) (2026-11-10)


### Bug Fixes

* **dashboard:** print report uri after uploading the final report ([#3939](https://github.com/stryker-mutator/stryker-net/issues/3939)) ([e92aa5b](https://github.com/stryker-mutator/stryker-net/commit/e92aa5bd1f9acc3bf9b4068062fc5ff6965c788d)), closes [#3227](https://github.com/stryker-mutator/stryker-net/issues/3227)
* **deps:** pin System.Security.Cryptography.Xml to a patched version ([#3846](https://github.com/stryker-mutator/stryker-net/issues/3846)) ([1ae2b3c](https://github.com/stryker-mutator/stryker-net/commit/1ae2b3c591a322d209cfdcd50bd2f10de20b8336)), closes [hi#severity](https://github.com/hi/issues/severity) [#3722](https://github.com/stryker-mutator/stryker-net/issues/3722) [#3835](https://github.com/stryker-mutator/stryker-net/issues/3835)
* **initialization:** Improve support for F# test projects ([#3809](https://github.com/stryker-mutator/stryker-net/issues/3809)) ([54ef1a3](https://github.com/stryker-mutator/stryker-net/commit/54ef1a33b730c7c385255cc404da3d53ccffe95c)), closes [#3804](https://github.com/stryker-mutator/stryker-net/issues/3804)
* **mtp:** Concurrency issue with multiple processes writing to the same mutant-control file ([#3724](https://github.com/stryker-mutator/stryker-net/issues/3724)) ([0dc7f04](https://github.com/stryker-mutator/stryker-net/commit/0dc7f045c89cd06c2b4b95c77ba96aa65a899ca7))
* **mtp:** drop parameter types from MTP test's fully qualified name in report ([#3955](https://github.com/stryker-mutator/stryker-net/issues/3955)) ([920397e](https://github.com/stryker-mutator/stryker-net/commit/920397e64b6f42d5094301875de4b0a3dfd06590)), closes [#3954](https://github.com/stryker-mutator/stryker-net/issues/3954)
* **mtp:** pass the test file location to the test description ([#3942](https://github.com/stryker-mutator/stryker-net/issues/3942)) ([2ebb6f2](https://github.com/stryker-mutator/stryker-net/commit/2ebb6f298ea3d80921397fed37d8f6496a78b69f)), closes [#3921](https://github.com/stryker-mutator/stryker-net/issues/3921)
* **mutating:** Support mutations with out variables in constructor initializers ([#3811](https://github.com/stryker-mutator/stryker-net/issues/3811)) ([b3cf83c](https://github.com/stryker-mutator/stryker-net/commit/b3cf83cad2738fe919e1ab5ac80fc1906c5c08cc)), closes [#3810](https://github.com/stryker-mutator/stryker-net/issues/3810)
* **regex-mutator:** support for interpolated string for regex mutator ([#3875](https://github.com/stryker-mutator/stryker-net/issues/3875)) ([f8bbbf2](https://github.com/stryker-mutator/stryker-net/commit/f8bbbf20d93ae45744b6add6925eb7315dcd511e)), closes [#3872](https://github.com/stryker-mutator/stryker-net/issues/3872)
* **rollback:** Handle compilation error induced by mutation erasing throw statements ([#3796](https://github.com/stryker-mutator/stryker-net/issues/3796)) ([37590bf](https://github.com/stryker-mutator/stryker-net/commit/37590bf8a863893a976a01e4a6992175db2f3cf4)), closes [#3783](https://github.com/stryker-mutator/stryker-net/issues/3783)
* **rollback:** Protect directives during roll back process ([#3817](https://github.com/stryker-mutator/stryker-net/issues/3817)) ([f74b4ce](https://github.com/stryker-mutator/stryker-net/commit/f74b4cef8e643952134a3e18d1415372eb7357b7)), closes [#3799](https://github.com/stryker-mutator/stryker-net/issues/3799)
* **since:** remove last '.git' from repo path instead of first ([#3982](https://github.com/stryker-mutator/stryker-net/issues/3982)) ([2ec2d68](https://github.com/stryker-mutator/stryker-net/commit/2ec2d68614600d191de828675a10f1d91048a7be)), closes [#3974](https://github.com/stryker-mutator/stryker-net/issues/3974)
* **test-runner:** distinguish discovery failures from empty test filters ([#3902](https://github.com/stryker-mutator/stryker-net/issues/3902)) ([81d392f](https://github.com/stryker-mutator/stryker-net/commit/81d392fd9a284935ea4c0f3a3aca940b2f9638e0)), closes [#3895](https://github.com/stryker-mutator/stryker-net/issues/3895)


### Features

* **cli:** expose test case filter option ([#3838](https://github.com/stryker-mutator/stryker-net/issues/3838)) ([98344e9](https://github.com/stryker-mutator/stryker-net/commit/98344e9660cd5281322d19ea712de8206e76aaf8)), closes [#3091](https://github.com/stryker-mutator/stryker-net/issues/3091)
* **compilation:** follow project C# language version ([#3839](https://github.com/stryker-mutator/stryker-net/issues/3839)) ([5abfbe4](https://github.com/stryker-mutator/stryker-net/commit/5abfbe4a3902a028601d056b06ef7e95bae3e7f1)), closes [#3662](https://github.com/stryker-mutator/stryker-net/issues/3662)
* **initialization:** Abort Stryker when using conflicting options ([#3807](https://github.com/stryker-mutator/stryker-net/issues/3807)) ([eaabae4](https://github.com/stryker-mutator/stryker-net/commit/eaabae406e0ed4b58fcf7eced99f72584345105a)), closes [#3795](https://github.com/stryker-mutator/stryker-net/issues/3795)
* **mtp:** apply test-case-filter in Stryker when the test app has no --filter ([#3961](https://github.com/stryker-mutator/stryker-net/issues/3961)) ([0508a9f](https://github.com/stryker-mutator/stryker-net/commit/0508a9fa8919510b4c282f6e0f755493b800f735)), closes [#3960](https://github.com/stryker-mutator/stryker-net/issues/3960)
* **MTP:** apply test-case-filter when using MTP testrunner ([#3848](https://github.com/stryker-mutator/stryker-net/issues/3848)) ([172799f](https://github.com/stryker-mutator/stryker-net/commit/172799f72e2abc46d03b907612b6556f2fe3a496)), closes [#3843](https://github.com/stryker-mutator/stryker-net/issues/3843) [#3843](https://github.com/stryker-mutator/stryker-net/issues/3843)
* **unity:** fail fast for unsupported test projects ([#3840](https://github.com/stryker-mutator/stryker-net/issues/3840)) ([9f1ced7](https://github.com/stryker-mutator/stryker-net/commit/9f1ced72edbdb01786015ac03305fd744e1baf16)), closes [#2381](https://github.com/stryker-mutator/stryker-net/issues/2381)


### Performance Improvements

* **test-runner:** materialize executed identifiers once per result ([#3929](https://github.com/stryker-mutator/stryker-net/issues/3929)) ([6a2513a](https://github.com/stryker-mutator/stryker-net/commit/6a2513a9e2e925594945038f938e5fc68a3c5aab))

# [5.0.0](https://github.com/stryker-mutator/stryker-net/compare/dotnet-stryker@4.16.0...dotnet-stryker@5.0.0) (2026-09-11)


### Bug Fixes

* MsTest not properly configured ([#3760](https://github.com/stryker-mutator/stryker-net/issues/3760)) ([18d0356](https://github.com/stryker-mutator/stryker-net/commit/18d03562cd6da321518bb8f5536921cb56b6188a))
* **MTP:** Flaky coverage results with MTP test runner ([#3696](https://github.com/stryker-mutator/stryker-net/issues/3696)) ([0f11451](https://github.com/stryker-mutator/stryker-net/commit/0f11451594681bd5426c57495e60b520bd9a68a3))
* **MTP:** MTP coverage per mutated assembly ([#3769](https://github.com/stryker-mutator/stryker-net/issues/3769)) ([eb94878](https://github.com/stryker-mutator/stryker-net/commit/eb94878a9763a7315b891d1d0bcf11f225b32e28)), closes [#3760](https://github.com/stryker-mutator/stryker-net/issues/3760) [#3760](https://github.com/stryker-mutator/stryker-net/issues/3760) [#3724](https://github.com/stryker-mutator/stryker-net/issues/3724)
* **reporters:** dispose reporter resources ([#3721](https://github.com/stryker-mutator/stryker-net/issues/3721)) ([45c74e5](https://github.com/stryker-mutator/stryker-net/commit/45c74e5fc39dcf996f29078d3c6f01b5b504b742))


### Features

* Add test information to html/json report for MTP test runner ([#3541](https://github.com/stryker-mutator/stryker-net/issues/3541)) ([5d497fa](https://github.com/stryker-mutator/stryker-net/commit/5d497fa1bd4e3da34661f505282c3017b3900eb8))
* **MTP:** Support `perTest` and `perTestInIsolation` coverage analysis ([#3752](https://github.com/stryker-mutator/stryker-net/issues/3752)) ([0705367](https://github.com/stryker-mutator/stryker-net/commit/070536793db9746fb9f922460993424ab29ecefb))
* Support solution configuration ([#3751](https://github.com/stryker-mutator/stryker-net/issues/3751)) ([b8bc307](https://github.com/stryker-mutator/stryker-net/commit/b8bc30719b640abe481d6b59393744a8edd44ac8))
* **timeouts:** Calculate timeout values based on actual mutant runtimes ([#3731](https://github.com/stryker-mutator/stryker-net/issues/3731)) ([4fa9ee7](https://github.com/stryker-mutator/stryker-net/commit/4fa9ee790e40d7bb4684c2b055d8c9b713642a48))

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
