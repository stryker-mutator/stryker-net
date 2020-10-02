# [0.19.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.18.0...stryker@0.19.0) (2020-09-04)


### Bug Fixes

* **Const mutation warning:** Don't mutate const local declaration ([#1183](https://github.com/stryker-mutator/stryker-net/issues/1183)) ([dde8e2b](https://github.com/stryker-mutator/stryker-net/commit/dde8e2b04351d5b6db5c55a673a89fe38d4b6218))
* **Mutation compile errors:** Redesign mutation logic ([#1172](https://github.com/stryker-mutator/stryker-net/issues/1172)) ([3ddb251](https://github.com/stryker-mutator/stryker-net/commit/3ddb25136b6eb20d6583651e1cb78f77b7ccc7b3))
* **Stackalloc arrays:** Mutate stackalloc arrays correctly ([#1133](https://github.com/stryker-mutator/stryker-net/issues/1133)) ([1a7416b](https://github.com/stryker-mutator/stryker-net/commit/1a7416b7961bb144d2b65be810530dfe745448f2))
* **SwitchExpressions:** Don't mutate constant value in SwitchExpression ([#1176](https://github.com/stryker-mutator/stryker-net/issues/1176)) ([e62c7d3](https://github.com/stryker-mutator/stryker-net/commit/e62c7d3d5306d12193f780b076433032cc46f88f))


### Features

* **Added linq mutations:** Added linq Union and intersect mutation. ([#1182](https://github.com/stryker-mutator/stryker-net/issues/1182)) ([edf7ebb](https://github.com/stryker-mutator/stryker-net/commit/edf7ebba3762f7798f9ca9657a7861e124dcea35))
* **dashboard compare:** Save mutation testing result in stryker dashboard to re-use in later run ([#1067](https://github.com/stryker-mutator/stryker-net/issues/1067)) ([c9a986d](https://github.com/stryker-mutator/stryker-net/commit/c9a986d39b56983aa10bc77706880ffd968cb03a))
* **dashboard compare:** test the mutants which are covered by unit tests in changed test files ([#1101](https://github.com/stryker-mutator/stryker-net/issues/1101)) ([e847896](https://github.com/stryker-mutator/stryker-net/commit/e847896228aad54809ad0db0c188ef0a5d7d7d9f))
* **Regex mutations:** Add mutations on regular expressions ([#1123](https://github.com/stryker-mutator/stryker-net/issues/1123)) ([45da048](https://github.com/stryker-mutator/stryker-net/commit/45da0484f9cde5cfd0c5cb93bcab068548d80ead))



# [0.18.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.17.1...stryker@0.18.0) (2020-05-07)


### Bug Fixes
* **Statics coverage analysis** Add statistics message ([#1000](https://github.com/stryker-mutator/stryker-net/issues/1000)) ([0ae3f16](https://github.com/stryker-mutator/stryker-net/commit/0ae3f1694e583fb6f05c4b0c23c96d7b5b1a6ebd)), closes [#999](https://github.com/stryker-mutator/stryker-net/issues/999)
* **Enum mutations warning:** Avoid mutating enum member declarations ([#1017](https://github.com/stryker-mutator/stryker-net/issues/1017)) ([abc51ce](https://github.com/stryker-mutator/stryker-net/commit/abc51ce4bd9c4e19e27fc86fa8d5d1db5ffe18e2))


### Features
* **Collection initializer mutator:** Add mutator for collections and arrays ([#1023](https://github.com/stryker-mutator/stryker-net/issues/1023)) ([863b56a](https://github.com/stryker-mutator/stryker-net/commit/863b56a09f0407e490d25efcdecce60e0187861e))
* **Compiling:** Support for project output types other than console app ([#1028](https://github.com/stryker-mutator/stryker-net/issues/1028)) ([c2d08ca](https://github.com/stryker-mutator/stryker-net/commit/c2d08ca29ab4d593a7da79c43f97f4781c3afdc4))
* **Core3.1:** Upgrade to dotnet core 3.1 ([#785](https://github.com/stryker-mutator/stryker-net/issues/785)) ([92283b5](https://github.com/stryker-mutator/stryker-net/commit/92283b5def0ffb10b74d0012d672905338deec14))


## [0.17.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.17.0...stryker@0.17.1) (2020-03-24)


### Bug Fixes

* **Dashboard reporter:** Show http response content in error message ([#997](https://github.com/stryker-mutator/stryker-net/issues/997)) ([9ab91b2](https://github.com/stryker-mutator/stryker-net/commit/9ab91b294ccefc0fb685300c7fc6fab50adede62))
* **Threshold break:** Threshold break calculation should use percentage ([#1004](https://github.com/stryker-mutator/stryker-net/issues/1004)) ([facd51a](https://github.com/stryker-mutator/stryker-net/commit/facd51a7f3340b3d8a59004f47b36bf6fce86c08))


# [0.17.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.16.1...stryker@0.17.0) (2020-03-21)


### Bug Fixes

* **Assignment expression mutator:** Add ExclusiveOrAssignment mutations to replace incorrect OrAssignment mutation ([#994](https://github.com/stryker-mutator/stryker-net/issues/994)) ([322cb1a](https://github.com/stryker-mutator/stryker-net/commit/322cb1a153c6593b48647577da4873221fed8c4d))
* **Cleartext reporter:** Re-add mutation score after a mutation testrun ([#993](https://github.com/stryker-mutator/stryker-net/issues/993)) ([e0878b1](https://github.com/stryker-mutator/stryker-net/commit/e0878b1a41505c6d62d768114238463121a96175))
* **While true bug:** Return default(T) for non-void methods ([#958](https://github.com/stryker-mutator/stryker-net/issues/958)) ([839d1f8](https://github.com/stryker-mutator/stryker-net/commit/839d1f842d3c1785d20e7de4b475cd752f7f48a0))

### Features

* **Coverage analysis:** Use coverage analysis to determine mutations that can be active at the same time ([#936](https://github.com/stryker-mutator/stryker-net/issues/936)) ([c0e5f35](https://github.com/stryker-mutator/stryker-net/commit/c0e5f359eae9164e3eeccc939d9ca3779eb73220)), closes [#760](https://github.com/stryker-mutator/stryker-net/issues/760) [#820](https://github.com/stryker-mutator/stryker-net/issues/820)
* **File resolving:** Skip auto generated code from mutation ([#995](https://github.com/stryker-mutator/stryker-net/issues/995)) ([2798e36](https://github.com/stryker-mutator/stryker-net/commit/2798e36af8edcf5e2542c48a388d34ebe4d6bd81))
* **Html reporter:** Update to mutation testing elements 1.3.0 ([#963](https://github.com/stryker-mutator/stryker-net/issues/963)) ([715c30b](https://github.com/stryker-mutator/stryker-net/commit/715c30b280567a24248e3899c0addce02522cadd))
* **Mutation exclusion:** Exclude mutants with ExcludeFromCodeCoverage attribute ([#964](https://github.com/stryker-mutator/stryker-net/issues/964)) ([aecc057](https://github.com/stryker-mutator/stryker-net/commit/aecc05735415ea03ad677bc71b9c837175b12e40))
* **Mutation placing:** Add support for mutating static expression bodied constructors ([#960](https://github.com/stryker-mutator/stryker-net/issues/960)) ([56a05e2](https://github.com/stryker-mutator/stryker-net/commit/56a05e291d5a94df8bc0cff237fb914dff8c58e7)), closes [#959](https://github.com/stryker-mutator/stryker-net/issues/959)


## [0.16.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.16.0...stryker@0.16.1) (2020-02-21)


### Bug Fixes

* **Project discovery:** discover the project under test correctly ([#957](https://github.com/stryker-mutator/stryker-net/issues/957)) ([c15880c](https://github.com/stryker-mutator/stryker-net/commit/c15880c967cd0daef73421b1b4c16dfb6cc99342))


# [0.16.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.15.0...stryker@0.16.0) (2020-02-08)


### Bug Fixes

* **Rollback:** Only crash out of stryker run after last rollback retry attempt  ([#930](https://github.com/stryker-mutator/stryker-net/issues/930)) ([73a4255](https://github.com/stryker-mutator/stryker-net/commit/73a4255a0b9791b1451b8cf4491525a6fe058cca)), closes [#929](https://github.com/stryker-mutator/stryker-net/issues/929)


### Features

* **Dotnet platform:** Enable roll-forward on major versions for CLI tool ([#786](https://github.com/stryker-mutator/stryker-net/issues/786)) ([ff78740](https://github.com/stryker-mutator/stryker-net/commit/ff78740df017f2692a22fe2c9fc128a4a272ee93))
* **Initialisation:** Fail gracefully on incorrect targetframework in csproj ([#872](https://github.com/stryker-mutator/stryker-net/issues/872)) ([aeac039](https://github.com/stryker-mutator/stryker-net/commit/aeac03907d1b90184f464803af69bd0aee6e274b)), closes [#681](https://github.com/stryker-mutator/stryker-net/issues/681)
* **Multiple test projects:** Add more than one test project support ([#830](https://github.com/stryker-mutator/stryker-net/issues/830)) ([54888af](https://github.com/stryker-mutator/stryker-net/commit/54888af2a046ee62f819c9f35769826a38a95f3f))
* use buildalyzer to identify source files ([#898](https://github.com/stryker-mutator/stryker-net/issues/898)) ([f2f219d](https://github.com/stryker-mutator/stryker-net/commit/f2f219db7165d488ca3cfb8f0a20c676059a66e6))
* **Parallelism:** Allow more parallel testrunners than logical processors despite performance impact ([#906](https://github.com/stryker-mutator/stryker-net/issues/906)) ([c1c6c7b](https://github.com/stryker-mutator/stryker-net/commit/c1c6c7b981186722e4a19a255a85aa5e1ef0bde4))
* **Reporters:** Enable Html reporter by default ([4e966c2](https://github.com/stryker-mutator/stryker-net/commit/4e966c249c6766995ca476f18e7905484d97ab11))


### Reverts

* Revert "build(deps): bump Microsoft.TestPlatform.Portable (#918)" ([8cd6141](https://github.com/stryker-mutator/stryker-net/commit/8cd614116189fc3dc3530eb12cd98cabb791acdd)), closes [#918](https://github.com/stryker-mutator/stryker-net/issues/918)


# [0.15.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.14.3...stryker@0.15.0) (2019-12-20)


### Bug Fixes

* **Mutating:** Don't negate IsPatternExpression and properly rollback Accessor methods ([#846](https://github.com/stryker-mutator/stryker-net/issues/846)) ([69d7a71](https://github.com/stryker-mutator/stryker-net/commit/69d7a7106d6f5d54ba6e983ef3c58d30523f9578)), closes [#820](https://github.com/stryker-mutator/stryker-net/issues/820)


### Features

* **Coverage analysis:** Implement defensive heuristics to cover when incorrect number of dynamic test cases are reported by testframework ([#852](https://github.com/stryker-mutator/stryker-net/issues/852)) ([470b08e](https://github.com/stryker-mutator/stryker-net/commit/470b08ee26afe3acbc2b96c91749b4245a6b13f6)), closes [#820](https://github.com/stryker-mutator/stryker-net/issues/820) [#843](https://github.com/stryker-mutator/stryker-net/issues/843) [#843](https://github.com/stryker-mutator/stryker-net/issues/843)
* **Dashboard reporter:** Add dashboard reporter ([#849](https://github.com/stryker-mutator/stryker-net/issues/849)) ([7764472](https://github.com/stryker-mutator/stryker-net/commit/7764472fccc14759ee1c12499ba5aeccc0ba450a))


### Reverts

* Revert "Publish" ([8e2897f](https://github.com/stryker-mutator/stryker-net/commit/8e2897fb533ada246b56a9b5f50dd1d174c0d5cc))



## [0.14.3](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.14.2...0.14.3) (2019-12-05)


### Bug Fixes

* **Compilation:** Compile with AssemblyIdentityComparer compilation option ([#828](https://github.com/stryker-mutator/stryker-net/issues/828)) ([6eb3c16](https://github.com/stryker-mutator/stryker-net/commit/6eb3c163a098299dbe0e052d3d1dd970a298948d))
* **Targetframeworkversion extraction:** Correctly extract aggregated framework versions ([#827](https://github.com/stryker-mutator/stryker-net/issues/827)) ([85bdc99](https://github.com/stryker-mutator/stryker-net/commit/85bdc99b100666442eb46c9cf3db4b3473368db0))



## [0.14.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.14.1...0.14.2) (2019-11-28)


### Bug Fixes

* **Vstest:** Correctly determine targetframework for vstest configuration ([#822](https://github.com/stryker-mutator/stryker-net/issues/822)) ([cd2c7ff](https://github.com/stryker-mutator/stryker-net/commit/cd2c7ff1e8fde4fd3c3167d0c4332be01927d255))



## [0.14.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.14.0...0.14.1) (2019-11-20)


### Bug Fixes

* **Coverage analysis:** The name 'AppDomain' does not exist in the current context ([#789](https://github.com/stryker-mutator/stryker-net/issues/789)) ([d53686c](https://github.com/stryker-mutator/stryker-net/commit/d53686c57b24076630c927d99d72f1e7c7f41e11))



# [0.14.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.13.0...0.14.0) (2019-11-15)


### Bug Fixes

* **Linq mutator:** mutate linq query containing conditional access operator ([#761](https://github.com/stryker-mutator/stryker-net/issues/761)) ([d018b97](https://github.com/stryker-mutator/stryker-net/commit/d018b97358f0a9df8e879aa2c2296883e45a0da7)), closes [#760](https://github.com/stryker-mutator/stryker-net/issues/760)
* **Logging:** Use non-generic non-reflection ILogger.Log to log startup logging ([#744](https://github.com/stryker-mutator/stryker-net/issues/744)) ([64449c1](https://github.com/stryker-mutator/stryker-net/commit/64449c10adefc34bf43e219b5d3568a3e4502f53))


### Features

* **Bitwise mutations:** Add bitwise mutator ([#755](https://github.com/stryker-mutator/stryker-net/issues/755)) ([c684268](https://github.com/stryker-mutator/stryker-net/commit/c684268de4547b98a2d68381d1643ad50392d98c))
* **Git diff:** Mutate only changed files based on git diff ([#708](https://github.com/stryker-mutator/stryker-net/issues/708)) ([34371c9](https://github.com/stryker-mutator/stryker-net/commit/34371c9bd47121f8c7458625718a229d8dfa0bee))
* **Negate mutator:** Add new negate mutations ([#773](https://github.com/stryker-mutator/stryker-net/issues/773)) ([8879654](https://github.com/stryker-mutator/stryker-net/commit/8879654f9170eaa276719ec3a3cefc6743009f2f))
* **Csharp8.0 nullable:** Compile with c# 8 nullable feature enabled ([#745](https://github.com/stryker-mutator/stryker-net/issues/745)) ([df11a97](https://github.com/stryker-mutator/stryker-net/commit/df11a978751752593c5b36bf4e3838913c59c03e))



# [0.13.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.12.0...0.13.0) (2019-09-06)


### Bug Fixes

* **AbortTestOnFail:** Make Abort test on fail option available from commandline ([#627](https://github.com/stryker-mutator/stryker-net/issues/627)) ([3008e33](https://github.com/stryker-mutator/stryker-net/commit/3008e33))
* **Signed assemblies:** Accept relative paths in signed assembly key ([#677](https://github.com/stryker-mutator/stryker-net/issues/677)) ([d8f1103](https://github.com/stryker-mutator/stryker-net/commit/d8f1103))
* **Vstest:** Change deployment of vstest package according to package structure change ([#685](https://github.com/stryker-mutator/stryker-net/issues/685)) ([41c759e](https://github.com/stryker-mutator/stryker-net/commit/41c759e))
* **VsTestRunner NPE:** Use ConcurrentBag with eventhandler instead of queue with monitor ([#629](https://github.com/stryker-mutator/stryker-net/issues/629)) ([1b1d60f](https://github.com/stryker-mutator/stryker-net/commit/1b1d60f))
* Handle warnings in compilation diagnostics ([#705](https://github.com/stryker-mutator/stryker-net/issues/705)) ([7863669](https://github.com/stryker-mutator/stryker-net/commit/7863669))


### Features

* **Default coverage analysis:** Coverage analysis default perTest ([#693](https://github.com/stryker-mutator/stryker-net/issues/693)) ([1b50795](https://github.com/stryker-mutator/stryker-net/commit/1b50795))
* **Html report:** Update html-elements to 1.1.1 ([#679](https://github.com/stryker-mutator/stryker-net/issues/679)) ([8377aad](https://github.com/stryker-mutator/stryker-net/commit/8377aad))
* **Ignore methods:** Allow users to specify methods that should be ignored when mutating their parameters ([#646](https://github.com/stryker-mutator/stryker-net/issues/646)) ([8b7d1fa](https://github.com/stryker-mutator/stryker-net/commit/8b7d1fa))
* **Language version:** Allow users to set c# language version used.([#568](https://github.com/stryker-mutator/stryker-net/issues/568)) ([a78040e](https://github.com/stryker-mutator/stryker-net/commit/a78040e)), closes [#557](https://github.com/stryker-mutator/stryker-net/issues/557)
* **MutateArgument:** Allow the user to specify which files to mutate. ([#662](https://github.com/stryker-mutator/stryker-net/issues/662)) ([155945d](https://github.com/stryker-mutator/stryker-net/commit/155945d)), closes [#1](https://github.com/stryker-mutator/stryker-net/issues/1) [#2](https://github.com/stryker-mutator/stryker-net/issues/2)
* **NoCoverage status:** Add NoCoverage result status to reporting ([#675](https://github.com/stryker-mutator/stryker-net/issues/675)) ([2a01fdd](https://github.com/stryker-mutator/stryker-net/commit/2a01fdd))
* **Specify test project:** Specify test project at any path relative to the working directory.  ([#588](https://github.com/stryker-mutator/stryker-net/issues/588)) ([5e916d2](https://github.com/stryker-mutator/stryker-net/commit/5e916d2))
* Fail gracefully when compile diagnostics indicate a general build error ([#649](https://github.com/stryker-mutator/stryker-net/issues/649)) ([8bab554](https://github.com/stryker-mutator/stryker-net/commit/8bab554))
* **Statics coverage analysis:** Identify 'static' mutants to make sure they are properly tested (issue [#623](https://github.com/stryker-mutator/stryker-net/issues/623)) ([#636](https://github.com/stryker-mutator/stryker-net/issues/636)) ([884e81d](https://github.com/stryker-mutator/stryker-net/commit/884e81d))
* **String mutator:** Mutate string.Empty to string literal "Stryker was here!" ([#653](https://github.com/stryker-mutator/stryker-net/issues/653)) ([d699204](https://github.com/stryker-mutator/stryker-net/commit/d699204))
* **Testcases discovery:** Remove unreliable totalnumberoftestsparser ([#566](https://github.com/stryker-mutator/stryker-net/issues/566)) ([4f54d3f](https://github.com/stryker-mutator/stryker-net/commit/4f54d3f))
* **Testrunner:** Set VsTest as default testrunner ([#617](https://github.com/stryker-mutator/stryker-net/issues/617)) ([c1c90d0](https://github.com/stryker-mutator/stryker-net/commit/c1c90d0))
* **Vstest.console exceptionhandling:** Restart vstest.console and retry testrun on translationlayer exception ([#676](https://github.com/stryker-mutator/stryker-net/issues/676)) ([17b3434](https://github.com/stryker-mutator/stryker-net/commit/17b3434))



# 0.12.0 (2019-07-01)


### Bug Fixes

* **Multiple nuget.exe paths:** Choose first nuget.exe path found.  ([#540](https://github.com/stryker-mutator/stryker-net/issues/540)) ([8635d6e](https://github.com/stryker-mutator/stryker-net/commit/8635d6e))
* **Shared projects:** Only include .projitems files in shared projects import ([#479](https://github.com/stryker-mutator/stryker-net/issues/479)) ([8f21ce1](https://github.com/stryker-mutator/stryker-net/commit/8f21ce1))
* **Total number of tests parser:** Improved parsing of total number of tests in test result. ([#556](https://github.com/stryker-mutator/stryker-net/issues/556)) ([0e35f1a](https://github.com/stryker-mutator/stryker-net/commit/0e35f1a))
* system.memory dependency issue by bumping dotnet core version to 2.1 ([#245](https://github.com/stryker-mutator/stryker-net/issues/245)) ([3b35d90](https://github.com/stryker-mutator/stryker-net/commit/3b35d90)), closes [#234](https://github.com/stryker-mutator/stryker-net/issues/234)

### Features

* **Additional timeout:** Decrease default additional timeout value to 5000 ([#567](https://github.com/stryker-mutator/stryker-net/issues/567)) ([f0903d7](https://github.com/stryker-mutator/stryker-net/commit/f0903d7))
* **Coverage analysis:** Implement coverage analysis with vstest ([#506](https://github.com/stryker-mutator/stryker-net/issues/506)) ([3858bbc](https://github.com/stryker-mutator/stryker-net/commit/3858bbc))
* **Update check:** Add version updates check ([#612](https://github.com/stryker-mutator/stryker-net/issues/612)) ([a6c940a](https://github.com/stryker-mutator/stryker-net/commit/a6c940a))
* Add assignment statement mutator ([955d871](https://github.com/stryker-mutator/stryker-net/commit/955d871))
* add PrefixUnaryMutator and PostfixUnaryMutator ([3c7242c](https://github.com/stryker-mutator/stryker-net/commit/3c7242c))
* Checked mutator ([375c040](https://github.com/stryker-mutator/stryker-net/commit/375c040))
* Support for timeouts ([#25](https://github.com/stryker-mutator/stryker-net/issues/25)) ([eb56899](https://github.com/stryker-mutator/stryker-net/commit/eb56899))


# [0.11.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.10.0...0.11.0) (2019-05-10)


### Bug Fixes

* **Project File Analyzer:** Improve solution path error message ([#460](https://github.com/stryker-mutator/stryker-net/issues/460)) ([ba5e4a2](https://github.com/stryker-mutator/stryker-net/commit/ba5e4a2))
* **Reporter names:** Improve reporter names ([#504](https://github.com/stryker-mutator/stryker-net/issues/504)) ([3648cbe](https://github.com/stryker-mutator/stryker-net/commit/3648cbe))
* **Rollback failed logging:** Improve error messages for rollback process ([#478](https://github.com/stryker-mutator/stryker-net/issues/478)) ([b1dd875](https://github.com/stryker-mutator/stryker-net/commit/b1dd875))
* **Shared projects:** Only include .projitems files in shared projects import ([#479](https://github.com/stryker-mutator/stryker-net/issues/479)) ([8f21ce1](https://github.com/stryker-mutator/stryker-net/commit/8f21ce1))


### Features

* **Compiling:** Compile assembly version information ([#482](https://github.com/stryker-mutator/stryker-net/issues/482)) ([f813f25](https://github.com/stryker-mutator/stryker-net/commit/f813f25))
* **Compiling:** False positive detection ([#481](https://github.com/stryker-mutator/stryker-net/issues/481)) ([0918e22](https://github.com/stryker-mutator/stryker-net/commit/0918e22))
* **Mutations:** negate mutator ([#451](https://github.com/stryker-mutator/stryker-net/issues/451)) ([a4630d0](https://github.com/stryker-mutator/stryker-net/commit/a4630d0))
* **Report json:** support html tags in source code and update html report to 1.0.5 ([#443](https://github.com/stryker-mutator/stryker-net/issues/443)) ([4b64821](https://github.com/stryker-mutator/stryker-net/commit/4b64821))
* **Rollback mutations:** Recompile 100 times ([#473](https://github.com/stryker-mutator/stryker-net/issues/473)) ([4530ff5](https://github.com/stryker-mutator/stryker-net/commit/4530ff5))
* **Xamarin support:** Run stryker.net on xamarin projects ([#488](https://github.com/stryker-mutator/stryker-net/issues/488)) ([d2b6010](https://github.com/stryker-mutator/stryker-net/commit/d2b6010))



# [0.10.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.9.0...0.10.0) (2019-04-05)


### Bug Fixes

* **Handeling statements:** ForStatements and ExpressionStatement mutated correctly ([#413](https://github.com/stryker-mutator/stryker-net/issues/413)) ([e1a654a](https://github.com/stryker-mutator/stryker-net/commit/e1a654a)), closes [#411](https://github.com/stryker-mutator/stryker-net/issues/411) [#412](https://github.com/stryker-mutator/stryker-net/issues/412)


### Features

* **Dotnet Framework support:** Add initial support for dotnet full framework ([#371](https://github.com/stryker-mutator/stryker-net/issues/371)) ([93ca8a5](https://github.com/stryker-mutator/stryker-net/commit/93ca8a5)), closes [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#28](https://github.com/stryker-mutator/stryker-net/issues/28) [#192](https://github.com/stryker-mutator/stryker-net/issues/192) [#389](https://github.com/stryker-mutator/stryker-net/issues/389) [#390](https://github.com/stryker-mutator/stryker-net/issues/390) [#398](https://github.com/stryker-mutator/stryker-net/issues/398) [#391](https://github.com/stryker-mutator/stryker-net/issues/391) [#407](https://github.com/stryker-mutator/stryker-net/issues/407) [#372](https://github.com/stryker-mutator/stryker-net/issues/372) [#418](https://github.com/stryker-mutator/stryker-net/issues/418)
* **Exclusions:** Warn user on all mutations excluded ([#416](https://github.com/stryker-mutator/stryker-net/issues/416)) ([8c45c7d](https://github.com/stryker-mutator/stryker-net/commit/8c45c7d))
* **Input file resolver:** Warn user when more than one csproj is found in working directory ([#426](https://github.com/stryker-mutator/stryker-net/issues/426)) ([7404fbd](https://github.com/stryker-mutator/stryker-net/commit/7404fbd))



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

* **Html-reporter:** Implement html reporter using mutation report html elements ([#352](https://github.com/stryker-mutator/stryker-net/issues/352)) ([417a2b7](https://github.com/stryker-mutator/stryker-net/commit/417a2b7))
* **Testrunner:** Integrate with vstest testrunner on windows ([#319](https://github.com/stryker-mutator/stryker-net/issues/319)) ([4a1422a](https://github.com/stryker-mutator/stryker-net/commit/4a1422a))



## [0.8.3](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.2...0.8.3) (2019-02-07)


### Bug Fixes

* **Mutating:** If statements are mutated nested ([#336](https://github.com/stryker-mutator/stryker-net/issues/336)) ([dbfe16f](https://github.com/stryker-mutator/stryker-net/commit/dbfe16f))



## [0.8.2](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.1...0.8.2) (2019-01-28)


### Bug Fixes

* **Local functions:** Local functions are mutated correctly ([#325](https://github.com/stryker-mutator/stryker-net/issues/325)) ([2670572](https://github.com/stryker-mutator/stryker-net/commit/2670572))
* **Rollback mutation:** Rollback two times instead of one in order to catch all compilation errors ([#323](https://github.com/stryker-mutator/stryker-net/issues/323)) ([00efe25](https://github.com/stryker-mutator/stryker-net/commit/00efe25))



## [0.8.1](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.8.0...0.8.1) (2019-01-25)


### Bug Fixes

* **C# 7.1 or greater:** Make Stryker compile all language features ([#311](https://github.com/stryker-mutator/stryker-net/issues/311)) ([25119e9](https://github.com/stryker-mutator/stryker-net/commit/25119e9))



# [0.8.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.7.0...0.8.0) (2019-01-11)


### Bug Fixes

* **Tuple bug:** ExpressionStatement placed as ConditionalExpression ([#280](https://github.com/stryker-mutator/stryker-net/issues/280)) ([f74f782](https://github.com/stryker-mutator/stryker-net/commit/f74f782))
* **UnauthorizedAccessException:** AppendTargetFrameworkToOutputPath csproj property is included ([#295](https://github.com/stryker-mutator/stryker-net/issues/295)) ([8a0b634](https://github.com/stryker-mutator/stryker-net/commit/8a0b634)), closes [#272](https://github.com/stryker-mutator/stryker-net/issues/272)


### Features

* **Dev mode:** Improve mutant rollback (Fix [#236](https://github.com/stryker-mutator/stryker-net/issues/236)) ([#257](https://github.com/stryker-mutator/stryker-net/issues/257)) ([2f6f459](https://github.com/stryker-mutator/stryker-net/commit/2f6f459))
* **Excluded mutations:** Add option to exlude one or more mutators ([#253](https://github.com/stryker-mutator/stryker-net/issues/253)) ([7033969](https://github.com/stryker-mutator/stryker-net/commit/7033969))
* **File exclusion:** Add option to exclude files from mutation run ([#196](https://github.com/stryker-mutator/stryker-net/issues/196)) ([596f907](https://github.com/stryker-mutator/stryker-net/commit/596f907))
* **Json reporter:** Add ability to generate mutation report in json format ([#284](https://github.com/stryker-mutator/stryker-net/issues/284)) ([a5b59c2](https://github.com/stryker-mutator/stryker-net/commit/a5b59c2))
* **No timeouts due to computer going in sleep mode:** Secure against sleep induced timeouts ([#249](https://github.com/stryker-mutator/stryker-net/issues/249)) ([69a0560](https://github.com/stryker-mutator/stryker-net/commit/69a0560))
* **Unsafe blocks:** recompile with unsafe=true ([#306](https://github.com/stryker-mutator/stryker-net/issues/306)) ([c9c913b](https://github.com/stryker-mutator/stryker-net/commit/c9c913b))



# [0.7.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.6.0...0.7.0) (2018-11-28)


### Bug Fixes

* **LinqMutator:** Linq mutator mutates only method invocations ([#252](https://github.com/stryker-mutator/stryker-net/issues/252)) ([5167997](https://github.com/stryker-mutator/stryker-net/commit/5167997))


### Features

* **Better error handling:** More understandable error messages ([#169](https://github.com/stryker-mutator/stryker-net/issues/169)) ([9214fbd](https://github.com/stryker-mutator/stryker-net/commit/9214fbd))
* **mutant placing:** Improve mutants performance ([#247](https://github.com/stryker-mutator/stryker-net/issues/247)) ([e2a6182](https://github.com/stryker-mutator/stryker-net/commit/e2a6182))
* **Shared projects:** Add support for shared projects ([#235](https://github.com/stryker-mutator/stryker-net/issues/235)) ([8304f2c](https://github.com/stryker-mutator/stryker-net/commit/8304f2c))



# [0.6.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.5.0...0.6.0) (2018-11-19)


### Bug Fixes

* **Logging:** Clean up oneline logger output by removing timestamp ([#227](https://github.com/stryker-mutator/stryker-net/issues/227)) ([769509e](https://github.com/stryker-mutator/stryker-net/commit/769509e))
* system.memory dependency issue by bumping dotnet core version to 2.1 ([#245](https://github.com/stryker-mutator/stryker-net/issues/245)) ([3b35d90](https://github.com/stryker-mutator/stryker-net/commit/3b35d90)), closes [#234](https://github.com/stryker-mutator/stryker-net/issues/234)


### Features

* **Conditional mutant placer:** Add ability to place mutations in ConditionalExpressions ([#207](https://github.com/stryker-mutator/stryker-net/issues/207)) ([e3e0433](https://github.com/stryker-mutator/stryker-net/commit/e3e0433))
* **integrationtest:** Run stryker from nuget in integration test ([#239](https://github.com/stryker-mutator/stryker-net/issues/239)) ([4b21514](https://github.com/stryker-mutator/stryker-net/commit/4b21514))



# [0.5.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.4.0...0.5.0) (2018-11-03)



<a name="0.4.0"></a>
# [0.4.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.3.0...0.4.0) (2018-10-28)


### Bug Fixes

* **Cli:** Return exit code 1 when score is below threshold break value ([#154](https://github.com/stryker-mutator/stryker-net/issues/154)) ([f4a8419](https://github.com/stryker-mutator/stryker-net/commit/f4a8419))
* **ConsoleReporter:** use 1-based line indexing ([#176](https://github.com/stryker-mutator/stryker-net/issues/176)) ([c302fe2](https://github.com/stryker-mutator/stryker-net/commit/c302fe2))
* **Mutating:** Make sure a broken mutation does not exist twice, so that we can roll back the mutation ([#145](https://github.com/stryker-mutator/stryker-net/issues/145)) ([#190](https://github.com/stryker-mutator/stryker-net/issues/190)) ([1f74cce](https://github.com/stryker-mutator/stryker-net/commit/1f74cce))
* **Test:** Run unit tests on all platforms ([#197](https://github.com/stryker-mutator/stryker-net/issues/197)) ([a4c27f0](https://github.com/stryker-mutator/stryker-net/commit/a4c27f0))


### Features

* **Logging:** Show number of tests found in initial run ([#138](https://github.com/stryker-mutator/stryker-net/issues/138)) ([57f5f08](https://github.com/stryker-mutator/stryker-net/commit/57f5f08))
* **Mutators:** Add Mutations for LINQ Expressions ([#185](https://github.com/stryker-mutator/stryker-net/issues/185)) ([5ae9d3a](https://github.com/stryker-mutator/stryker-net/commit/5ae9d3a))
* **Mutators:** Add string and interpolated string mutators ([#194](https://github.com/stryker-mutator/stryker-net/issues/194)) ([653f159](https://github.com/stryker-mutator/stryker-net/commit/653f159))
* **Testing:** Add happyflow integration test ([#163](https://github.com/stryker-mutator/stryker-net/issues/163)) ([2b2f9ba](https://github.com/stryker-mutator/stryker-net/commit/2b2f9ba))
* **Reporting:** Set default log level to Info and cleanup reporters ([#167](https://github.com/stryker-mutator/stryker-net/issues/167)) ([b378262](https://github.com/stryker-mutator/stryker-net/commit/b378262))



<a name="0.3.0"></a>
# [0.3.0](https://github.com/stryker-mutator/stryker-net/compare/StrykerMutator.Core@0.2.0...StrykerMutator.Core@0.3.0) (2018-10-06)


### Bug Fixes
* Ensure directories/files don't exist before creating them ([#157](https://github.com/stryker-mutator/stryker-net/issues/157)) ([d4d2497](https://github.com/stryker-mutator/stryker-net/commit/d4d2497)), closes [#155](https://github.com/stryker-mutator/stryker-net/issues/155)
* **Dependency-resolver:** Support backslash in ProjectReferences on Linux ([#149](https://github.com/stryker-mutator/stryker-net/issues/149)) ([223e841](https://github.com/stryker-mutator/stryker-net/commit/223e841)), closes [#120](https://github.com/stryker-mutator/stryker-net/issues/120)


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
