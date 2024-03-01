# [4.0.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.13.2...stryker@4.0.0) (2024-02-16)


### Features

* **reporters:** add support for real-time reporting in `DashboardReporter` ([#2563](https://github.com/stryker-mutator/stryker-net/issues/2563)) ([1ab3f29](https://github.com/stryker-mutator/stryker-net/commit/1ab3f293fabea735954edf15febcd4ea353ac1c5))
* **target framework:** Update dotnet target framework from 6 to 8 ([#2851](https://github.com/stryker-mutator/stryker-net/issues/2851)) ([9a978e6](https://github.com/stryker-mutator/stryker-net/commit/9a978e6dcfcd19b3cdce77322da13027d58e0651))



## [3.13.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.13.1...stryker@3.13.2) (2024-01-25)


### Bug Fixes

* **null-forgiving operator:** Mutation leak with SuppressNullableWarningSyntax constructs (!.) ([#2826](https://github.com/stryker-mutator/stryker-net/issues/2826)) ([469e3f6](https://github.com/stryker-mutator/stryker-net/commit/469e3f6fdb5f7cabefd32b50e34e9a03930b32dc))
* **real time reporting:** Whitespace in report data no longer removed ([#2821](https://github.com/stryker-mutator/stryker-net/issues/2821)) ([2cb16a9](https://github.com/stryker-mutator/stryker-net/commit/2cb16a9e89a6f14e0021ed01b96bbf07d3f25081))
* Stryker fails if project has conflicting settings regarding warnings ([#2828](https://github.com/stryker-mutator/stryker-net/issues/2828)) ([c2583e0](https://github.com/stryker-mutator/stryker-net/commit/c2583e04e9e298f934c301d4a729f96d134c7bc2))


### Reverts

* Revert "Publish" ([73a8177](https://github.com/stryker-mutator/stryker-net/commit/73a8177949157650100523dfe0368e75aabd56cf))



## [3.13.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.13.0...stryker@3.13.1) (2024-01-05)


### Bug Fixes

* Make sure to select valid IAnalyzerResult from Buildalyzer ([#2811](https://github.com/stryker-mutator/stryker-net/issues/2811)) ([70adc60](https://github.com/stryker-mutator/stryker-net/commit/70adc60caad193ed6942ee970ea5373003ade4e4)), closes [#1900](https://github.com/stryker-mutator/stryker-net/issues/1900)



# [3.13.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.12.0...stryker@3.13.0) (2023-12-22)


### Bug Fixes

* **ignore-methods:** Adjust IgnoredMethod filter to more consistent results ([#2689](https://github.com/stryker-mutator/stryker-net/issues/2689)) ([ceea82d](https://github.com/stryker-mutator/stryker-net/commit/ceea82d2e95f929455dee5595ca99300b4e0d544))


### Features

* Support compiler diagnostic options from msbuild ([#2777](https://github.com/stryker-mutator/stryker-net/issues/2777)) ([059f329](https://github.com/stryker-mutator/stryker-net/commit/059f32981c5a525289f65eacde92bac093a9546c)), closes [#2783](https://github.com/stryker-mutator/stryker-net/issues/2783)



# [3.12.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.11.1...stryker@3.12.0) (2023-11-22)



## [3.11.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.11.0...stryker@3.11.1) (2023-10-31)


### Bug Fixes

* Support recursive mutant removal ([#2739](https://github.com/stryker-mutator/stryker-net/issues/2739)) ([00621ec](https://github.com/stryker-mutator/stryker-net/commit/00621ec07d42540f821db39edcfc86e8094d4f91))
* Validation on Since enablement when Baseline is also enabled ([#2744](https://github.com/stryker-mutator/stryker-net/issues/2744)) ([dfbf510](https://github.com/stryker-mutator/stryker-net/commit/dfbf510f38ae14a300ee08f31233e1e67dc8abf2))



# [3.11.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.10.0...stryker@3.11.0) (2023-10-27)


### Bug Fixes

* Don't fail the test run if restoring test assemblies fails ([#2664](https://github.com/stryker-mutator/stryker-net/issues/2664)) ([0ba3f77](https://github.com/stryker-mutator/stryker-net/commit/0ba3f77d82ecc3e091112d6f2ebd98d28a336bd3))
* **ignore-methods null conditional:** Enable filtering null-conditional method invocations by name ([#2657](https://github.com/stryker-mutator/stryker-net/issues/2657)) ([16e2d16](https://github.com/stryker-mutator/stryker-net/commit/16e2d169c490174173dd52365defdaf334bb511e))
* **input validation:** Only validate baseline configuration when baseline is enabled ([#2729](https://github.com/stryker-mutator/stryker-net/issues/2729)) ([6a37ac3](https://github.com/stryker-mutator/stryker-net/commit/6a37ac3b392aeb8a293b45dc13281f2108bc7549))
* Remove usings from MutantControl ([#2694](https://github.com/stryker-mutator/stryker-net/issues/2694)) ([03f9913](https://github.com/stryker-mutator/stryker-net/commit/03f991307924b67d14791b7ee42d1f0f1c2c8717))
* Support case insensitive reference resolution ([#2719](https://github.com/stryker-mutator/stryker-net/issues/2719)) ([00f9a12](https://github.com/stryker-mutator/stryker-net/commit/00f9a126f8b15d1a504cbcd875a94e0789d00f01))
* Use solution file when it's available ([#2690](https://github.com/stryker-mutator/stryker-net/issues/2690)) ([4696956](https://github.com/stryker-mutator/stryker-net/commit/4696956393e22b924e9ba03361fdf51ee6dc30e5))


### Features

* **baseline:** Support large reports in azure file share ([#2588](https://github.com/stryker-mutator/stryker-net/issues/2588)) ([4d6dd37](https://github.com/stryker-mutator/stryker-net/commit/4d6dd37929afc8b1dc6153ebcd25f973ad630dbe))
* **baseline:** Validate mutual exclusivity of explicit --since and --with-baseline options ([#2723](https://github.com/stryker-mutator/stryker-net/issues/2723)) ([47b3739](https://github.com/stryker-mutator/stryker-net/commit/47b373995213bdb1c56e04099939b033a9987b05))
* **real-time reporting:** Queue mutants before client connects ([#2722](https://github.com/stryker-mutator/stryker-net/issues/2722)) ([e2ce014](https://github.com/stryker-mutator/stryker-net/commit/e2ce0149f1b175537d1c4ea366dd95cc69b396b5))
* Retry build with msbuild when dotnet build fails ([#2698](https://github.com/stryker-mutator/stryker-net/issues/2698)) ([b32cef2](https://github.com/stryker-mutator/stryker-net/commit/b32cef2b61589c9735d8d03ba5c1e0acdaa66da7))



# [3.10.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.9.2...stryker@3.10.0) (2023-07-28)


### Bug Fixes

* Improve Vstest support ([#2595](https://github.com/stryker-mutator/stryker-net/issues/2595)) ([04268c1](https://github.com/stryker-mutator/stryker-net/commit/04268c10366d82bff2a6d404bb2383481530ce8d))
* Increase time out before assuming VsTest is frozen ([#2576](https://github.com/stryker-mutator/stryker-net/issues/2576)) ([3069cb2](https://github.com/stryker-mutator/stryker-net/commit/3069cb267731d29dbced93f61cf8417457dfa90d))
* Small changes to configuration help text ([#2586](https://github.com/stryker-mutator/stryker-net/issues/2586)) ([c68c14e](https://github.com/stryker-mutator/stryker-net/commit/c68c14eb767705b418e189c13971cf58a5585647))


### Features

* **assembly reference resolving:** Support reference aliases ([#2604](https://github.com/stryker-mutator/stryker-net/issues/2604)) ([1cec405](https://github.com/stryker-mutator/stryker-net/commit/1cec405d01821a495b2115e10181b74db88b8296))
* **buildalyzer:** Provide msbuild log when buildalyzer fails analysis ([#2605](https://github.com/stryker-mutator/stryker-net/issues/2605)) ([2849911](https://github.com/stryker-mutator/stryker-net/commit/2849911b47e65d39b31e0b0f5cd5679a565969ff))
* **reporting:** Update mutation-testing-elements to 2.0.3 which adds a progress bar ([#2614](https://github.com/stryker-mutator/stryker-net/issues/2614)) ([dfe7559](https://github.com/stryker-mutator/stryker-net/commit/dfe7559bda0ebb2aa3306eb90059c3b5cedf6780))



## [3.9.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.9.1...stryker@3.9.2) (2023-06-09)


### Bug Fixes

* **report:** Use fullPath for file name in json and html report ([#2567](https://github.com/stryker-mutator/stryker-net/issues/2567)) ([353bde5](https://github.com/stryker-mutator/stryker-net/commit/353bde578a0b98c36e5ee0b9830c9cd3b6408cb5))



## [3.9.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.9.0...stryker@3.9.1) (2023-06-09)


### Bug Fixes

* exception when source files in more than one test project ([#2542](https://github.com/stryker-mutator/stryker-net/issues/2542)) ([e280f32](https://github.com/stryker-mutator/stryker-net/commit/e280f322bf63428dfd68c2bac9bf2d4f11a385a0))
* **testrunner:** fix scenario where vstest would hang if no tests were detected ([#2550](https://github.com/stryker-mutator/stryker-net/issues/2550)) ([e77dc26](https://github.com/stryker-mutator/stryker-net/commit/e77dc26d10525d995b98522c2c08bf37da978415))



# [3.9.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.8.2...stryker@3.9.0) (2023-05-12)


### Bug Fixes

* **reporting:** More reliable auto-open browser ([#2523](https://github.com/stryker-mutator/stryker-net/issues/2523)) ([6e5f386](https://github.com/stryker-mutator/stryker-net/commit/6e5f386a5cf1822bcddd2bc08e350156b5d0123a))


### Features

* **html report:** Update mutation testing elements to 2.0.1 ([#2514](https://github.com/stryker-mutator/stryker-net/issues/2514)) ([c116bd9](https://github.com/stryker-mutator/stryker-net/commit/c116bd9bc4c3dc030dc5025873810f2341945fa0))
* Record for each unit test which mutant they kill ([#2491](https://github.com/stryker-mutator/stryker-net/issues/2491)) ([da7cffd](https://github.com/stryker-mutator/stryker-net/commit/da7cffd244a9edd2855cffad3807948987abf18f))
* **solution mode:** Improve solution handling ([#2400](https://github.com/stryker-mutator/stryker-net/issues/2400)) ([6b48029](https://github.com/stryker-mutator/stryker-net/commit/6b480299fb00430c75054acc226341c17ecbafc3))



## [3.8.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.8.1...stryker@3.8.2) (2023-05-07)


### Bug Fixes

* **tests report:** Correctly parse test files with preprocessor symbols ([#2502](https://github.com/stryker-mutator/stryker-net/issues/2502)) ([86b4720](https://github.com/stryker-mutator/stryker-net/commit/86b4720028f17b83a2ba297bc6774a89d992332b))



## [3.8.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.8.0...stryker@3.8.1) (2023-05-05)


### Bug Fixes

* **baseline:** json reports with testfiles are now correctly deserialized ([#2498](https://github.com/stryker-mutator/stryker-net/issues/2498)) ([38793e1](https://github.com/stryker-mutator/stryker-net/commit/38793e16a4f8edd1be5766433c9443de62ff447d))



# [3.8.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.7.1...stryker@3.8.0) (2023-05-04)


### Bug Fixes

* **initial-build:** add quotes around buildCommand when necessary ([#2455](https://github.com/stryker-mutator/stryker-net/issues/2455)) ([7689a6e](https://github.com/stryker-mutator/stryker-net/commit/7689a6e0e622ecbfea855625706aa2e1f64be2f6))


### Features

* Add trace logging with vstest console messages ([#2490](https://github.com/stryker-mutator/stryker-net/issues/2490)) ([f072f1b](https://github.com/stryker-mutator/stryker-net/commit/f072f1b4a3bb1f4e78cbe52dcccaee47ef0a8c59))
* **compiling:** signing key is not required even if SignAssembly is…set to true ([#2457](https://github.com/stryker-mutator/stryker-net/issues/2457)) ([f6aa688](https://github.com/stryker-mutator/stryker-net/commit/f6aa68874a2b43db2c99f7cabd9da39444521da2))
* **reporting:** add support for realtime reporting ([#2469](https://github.com/stryker-mutator/stryker-net/issues/2469)) ([959ded0](https://github.com/stryker-mutator/stryker-net/commit/959ded0bd751d5d2b234eddfc8eca83c9a5bc75f)), closes [stryker-mutator/mutation-testing-elements#2434](https://github.com/stryker-mutator/mutation-testing-elements/issues/2434)
* **reporting:** Report on tests in mutation report ([#1850](https://github.com/stryker-mutator/stryker-net/issues/1850)) ([98f4e97](https://github.com/stryker-mutator/stryker-net/commit/98f4e97bb90fdc5f142c18eb34e51373d1f64868))



## [3.7.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.7.0...stryker@3.7.1) (2023-03-23)


### Bug Fixes

* **String mutations:** Generate empty string mutations parenthesized ([#2447](https://github.com/stryker-mutator/stryker-net/issues/2447)) ([0650cb1](https://github.com/stryker-mutator/stryker-net/commit/0650cb18a2082de58f94e5dedb5fe74a4f248bd6))



# [3.7.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.6.1...stryker@3.7.0) (2023-03-20)


### Bug Fixes

* **dashboard reporter:** Print the -o hint in actual cyan instead of the literal markup ([#2406](https://github.com/stryker-mutator/stryker-net/issues/2406)) ([e8cfa7d](https://github.com/stryker-mutator/stryker-net/commit/e8cfa7d7ecb2f9b590bda658798c999ebae9377e))
* **StringMutator:** don't mutate strings in Guid ctor ([#2428](https://github.com/stryker-mutator/stryker-net/issues/2428)) ([1baa6ca](https://github.com/stryker-mutator/stryker-net/commit/1baa6ca8054e15ccea7042ab6b1ce1d0221c5299))


### Features

* **MutantFilters:** Ignore Block mutations for blocks that already contain active mutants ([#2382](https://github.com/stryker-mutator/stryker-net/issues/2382)) ([d912c9b](https://github.com/stryker-mutator/stryker-net/commit/d912c9b6cf3d5e0703ea34440de3c667c8dc2f47))
* **string mutations:** Add string.IsNullOrEmpty and string.IsNullOrWhiteSpace mutations ([#2429](https://github.com/stryker-mutator/stryker-net/issues/2429)) ([47b6745](https://github.com/stryker-mutator/stryker-net/commit/47b67453f6410577b928959b7cee3d87e94b0896))
* Try loading embedded resources from on-disk module before generating ([#2433](https://github.com/stryker-mutator/stryker-net/issues/2433)) ([e919891](https://github.com/stryker-mutator/stryker-net/commit/e9198910741629020ae6f7dec6ebb8340c2205f1))



## [3.6.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.6.0...stryker@3.6.1) (2023-02-17)


### Features

* **embedded resources:** Support embedded resources outside the root of the project ([#2414](https://github.com/stryker-mutator/stryker-net/issues/2414)) ([59fa56f](https://github.com/stryker-mutator/stryker-net/commit/59fa56f343aac9c837c21c5d4f122aee26c9318a))



# [3.6.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.5.1...stryker@3.6.0) (2023-02-10)


### Bug Fixes

* **embedded resources generation:** Use buildalyzer to find root namespace and embedded resources ([#2404](https://github.com/stryker-mutator/stryker-net/issues/2404)) ([a4969ff](https://github.com/stryker-mutator/stryker-net/commit/a4969ff60debcb6a94a059ba2d2aa84cb54029ef))


### Features

* **html report:** Update mutation testing elements to 1.7.14 ([#2405](https://github.com/stryker-mutator/stryker-net/issues/2405)) ([98ad817](https://github.com/stryker-mutator/stryker-net/commit/98ad8178e61c1fad54c208e2712063634a0f6961))



## [3.5.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.5.0...stryker@3.5.1) (2023-02-06)


### Bug Fixes

* **dashboard reporter:** fix dashboard publishing when the project name is computed ([#2397](https://github.com/stryker-mutator/stryker-net/issues/2397)) ([7395b2b](https://github.com/stryker-mutator/stryker-net/commit/7395b2bc4dfd9db962e1ea1e9d13bc7e6235c252)), closes [#1710](https://github.com/stryker-mutator/stryker-net/issues/1710)



# [3.5.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.4.0...stryker@3.5.0) (2023-02-06)


### Bug Fixes

* **no test projects:** null-reference exception when no test projects are found ([#2377](https://github.com/stryker-mutator/stryker-net/issues/2377)) ([419a8a7](https://github.com/stryker-mutator/stryker-net/commit/419a8a746190d2e4262781525b59aef7791fa56f))
* **NullCoalescingExpressionMutator:** Don't generate certain mutants when right hand side is ThrowExpression ([#2387](https://github.com/stryker-mutator/stryker-net/issues/2387)) ([#2389](https://github.com/stryker-mutator/stryker-net/issues/2389)) ([e344382](https://github.com/stryker-mutator/stryker-net/commit/e34438259c26cf6b563c7a724b3f7bba14ddf768))


### Features

* **embedded resources:** Improve extraction of embedded resources ([#2383](https://github.com/stryker-mutator/stryker-net/issues/2383)) ([43c2918](https://github.com/stryker-mutator/stryker-net/commit/43c2918362a87be6f93b634df78c6ccb80bc6fbb))



# [3.4.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.3.0...stryker@3.4.0) (2022-12-23)


### Bug Fixes

* **nugetrestore:** use MsBuild option during nuget restore ([#2359](https://github.com/stryker-mutator/stryker-net/issues/2359)) ([892b69c](https://github.com/stryker-mutator/stryker-net/commit/892b69c1b0c9add9d4e35d8e65b21ac255510617))


### Features

* **pattern mutations:** Mutator for relational and logical patterns in is\switch expressions ([#2313](https://github.com/stryker-mutator/stryker-net/issues/2313)) ([70ded11](https://github.com/stryker-mutator/stryker-net/commit/70ded110c3dbf344386e012259c0d8a513ae9bbe))



# [3.3.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.2.0...stryker@3.3.0) (2022-12-09)


### Bug Fixes

* **GitDiffProvider:** remove stryker-config exclusion. ([#2338](https://github.com/stryker-mutator/stryker-net/issues/2338)) ([53286d3](https://github.com/stryker-mutator/stryker-net/commit/53286d34467d335175fd8660490525546cad1c14))
* **Solution context run:** Allow relative path for solution file  ([#2333](https://github.com/stryker-mutator/stryker-net/issues/2333)) ([01fbf8a](https://github.com/stryker-mutator/stryker-net/commit/01fbf8a0ac56aa9bfc007eaeab6e9e3d954410cd))


### Features

* **mutators:** Add new methods to LinqMutator ([#2329](https://github.com/stryker-mutator/stryker-net/issues/2329)) ([bdec359](https://github.com/stryker-mutator/stryker-net/commit/bdec359a835e139b6632fb5b2da3a27c5a116ffc))
* **mutators:** Add null-coalescing operator mutator ([#2284](https://github.com/stryker-mutator/stryker-net/issues/2284)) ([dc7c513](https://github.com/stryker-mutator/stryker-net/commit/dc7c513681559b31e4142e7fbbc1b9a7f271ff97))
* **solution context run:** discover solution file ([#2340](https://github.com/stryker-mutator/stryker-net/issues/2340)) ([52bc645](https://github.com/stryker-mutator/stryker-net/commit/52bc645d9f16cc4f397045dcb1bf21b87da73283))



# [3.2.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.1.0...stryker@3.2.0) (2022-11-21)


### Features

* Support csharp 11 ([#2318](https://github.com/stryker-mutator/stryker-net/issues/2318)) ([7b23fa5](https://github.com/stryker-mutator/stryker-net/commit/7b23fa55cd02e88342a18c96db9a071fd041dda0))
* Use available target framework if chosen target framework is not found ([#2322](https://github.com/stryker-mutator/stryker-net/issues/2322)) ([46435cd](https://github.com/stryker-mutator/stryker-net/commit/46435cd540cf51476aead5ecb0bb02fdc955f964))



# [3.1.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.0.1...stryker@3.1.0) (2022-11-11)


### Bug Fixes

* **assignment statement mutator:** Add trivia between operands ([#2276](https://github.com/stryker-mutator/stryker-net/issues/2276)) ([9b09b2d](https://github.com/stryker-mutator/stryker-net/commit/9b09b2d8c2b080e5fdc90130daa5dddf3cc380fe))
* **compilation:** Align source generator options to main compilation options ([#2301](https://github.com/stryker-mutator/stryker-net/issues/2301)) ([3442a8f](https://github.com/stryker-mutator/stryker-net/commit/3442a8fb8fec9d2a10ea6ae21f0f061955fc1a4d))
* **mutator description:** multiple mutator descriptions ([#2277](https://github.com/stryker-mutator/stryker-net/issues/2277)) ([f6d605b](https://github.com/stryker-mutator/stryker-net/commit/f6d605b627bc1cbe2d95e082dda761a7adccc7e3))
* **PrefixUnaryExpression mutator:** Handle mutations of prefixed increment/decrement ([#2285](https://github.com/stryker-mutator/stryker-net/issues/2285)) ([5de6109](https://github.com/stryker-mutator/stryker-net/commit/5de6109e6a4c2d7ac13240718a2e035b1a46a066))
* **Project filter:** Project filter should work on Macos ([#2212](https://github.com/stryker-mutator/stryker-net/issues/2212)) ([b283f6a](https://github.com/stryker-mutator/stryker-net/commit/b283f6af64abdacd38e56abdc136964ebe7cabf6))
* **reporting:** Correctly show whitespace in report ([#2289](https://github.com/stryker-mutator/stryker-net/issues/2289)) ([92d9e5a](https://github.com/stryker-mutator/stryker-net/commit/92d9e5afbef8b36a2aecb843ec487cbb30709490))
* **reporting:** Spaces in the output path are supported for the auto open report option ([#2264](https://github.com/stryker-mutator/stryker-net/issues/2264)) ([dd55cb2](https://github.com/stryker-mutator/stryker-net/commit/dd55cb29eebf2ccf5e8a85276159d8ce3587e5ec))
* rollback whole constructor if compile error mutation location cannot be found ([#2250](https://github.com/stryker-mutator/stryker-net/issues/2250)) ([a8069a7](https://github.com/stryker-mutator/stryker-net/commit/a8069a78f6e4a85cd3749a528b260925e04f8799))
* **since:** correctly classify changed files as source or test files ([#2256](https://github.com/stryker-mutator/stryker-net/issues/2256)) ([89f04fe](https://github.com/stryker-mutator/stryker-net/commit/89f04fe8b384e2ef2cadb3ed6a8fe64d1d94b700))
* **TestCaseFilter:** Use null instead of empty string for parameter ([#2254](https://github.com/stryker-mutator/stryker-net/issues/2254)) ([5b6f288](https://github.com/stryker-mutator/stryker-net/commit/5b6f288c9f14d1a842f439ff23e85a59c89867e8))


### Features

* **assignment mutator:** Mutate coalesce assignment operations ([#2274](https://github.com/stryker-mutator/stryker-net/issues/2274)) ([c733ffe](https://github.com/stryker-mutator/stryker-net/commit/c733ffe3e16641ab9ae75ebab8e1ce8790bc32b0))
* **InitializerMutator:** Add object initializer mutator ([#2259](https://github.com/stryker-mutator/stryker-net/issues/2259)) ([f5bd19b](https://github.com/stryker-mutator/stryker-net/commit/f5bd19b99f4d0084379965c84ce2e0ec32c9c858))
* **mutators:** Add Math methods mutator ([#2244](https://github.com/stryker-mutator/stryker-net/issues/2244)) ([34a87c9](https://github.com/stryker-mutator/stryker-net/commit/34a87c90e3f8c0b79a0cd71665e70893a9951618))
* **reporting:** Specify output path for reports and logs ([#2267](https://github.com/stryker-mutator/stryker-net/issues/2267)) ([bba21d3](https://github.com/stryker-mutator/stryker-net/commit/bba21d39dd2870ee2c118c2cb15bd6cb9b7849bd))
* **Run on complete solution:** Run stryker on all projects in a solution ([#2234](https://github.com/stryker-mutator/stryker-net/issues/2234)) ([1a0b274](https://github.com/stryker-mutator/stryker-net/commit/1a0b274616e8d29d9daedc4e606a25e9107f9fa4))



## [3.0.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@3.0.0...stryker@3.0.1) (2022-09-16)

* **html reporter:** Fix Spectre.Console link escaping by updating Spectre.Console ([#2108](https://github.com/stryker-mutator/stryker-net/issues/2108)) ([e207047](https://github.com/stryker-mutator/stryker-net/commit/e2070475911c92da619ebfbb648c9f430ab2a778))

# [3.0.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@2.2.0...stryker@3.0.0) (2022-09-07)


### Bug Fixes

* **azure-fileshare-baseline:** Use full SAS token format instead of making assumptions and transformations on the token ([#2149](https://github.com/stryker-mutator/stryker-net/issues/2149)) ([9c0694e](https://github.com/stryker-mutator/stryker-net/commit/9c0694e7dda088ed1ae36d8390703691dea8e98e))
* Stryker not properly flagging mutant as TimedOut ([#2143](https://github.com/stryker-mutator/stryker-net/issues/2143)) ([3299c18](https://github.com/stryker-mutator/stryker-net/commit/3299c182e6f3b8e5f5b3e6a62e3505edde097459))


### Features

* **initial test:** Configure stryker to fail when initial testrun fails ([#2151](https://github.com/stryker-mutator/stryker-net/issues/2151)) ([9c47a4b](https://github.com/stryker-mutator/stryker-net/commit/9c47a4b6f9f5d2c32478338ca33e6568e5ee55f3))


### BREAKING CHANGES

* **azure-fileshare-baseline:** SAS must contain `sv=` and `sig=` to be valid. SAS without sv= are no longer transformed to valid SAS.



# [2.2.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@2.1.2...stryker@2.2.0) (2022-08-22)


### Features

* **reporters:** Markdown File Summary Reporter ([#2138](https://github.com/stryker-mutator/stryker-net/issues/2138)) ([0ac8347](https://github.com/stryker-mutator/stryker-net/commit/0ac8347a219f80ca7d2cbaa16971780dbeb2f685))
* **testrunner:** Diagnose failed test discovery and provide fix hints ([#2139](https://github.com/stryker-mutator/stryker-net/issues/2139)) ([90971a2](https://github.com/stryker-mutator/stryker-net/commit/90971a290bff8eab5fcf3e4148cc440ebddf9f86))



## [2.1.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@2.1.1...stryker@2.1.2) (2022-08-05)


### Bug Fixes

* **exception logging:** Log vstest exceptions instead of swallowing ([#2123](https://github.com/stryker-mutator/stryker-net/issues/2123)) ([fc95813](https://github.com/stryker-mutator/stryker-net/commit/fc958136e813ca5d6eb56afbed009c5f5c91f802))



## [2.1.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@2.1.0...stryker@2.1.1) (2022-08-03)


### Bug Fixes

* **coverage analysis:** static mutants not marked as needing all tests ([#2121](https://github.com/stryker-mutator/stryker-net/issues/2121)) ([f120710](https://github.com/stryker-mutator/stryker-net/commit/f120710e6a29e66018e230e9df4bdfcab7e643e5))



# [2.1.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@2.0.0...stryker@2.1.0) (2022-07-26)


### Bug Fixes

* **baseline:** increase compatibility of azure file share provider with older SAS keys ([#2115](https://github.com/stryker-mutator/stryker-net/issues/2115)) ([7639793](https://github.com/stryker-mutator/stryker-net/commit/7639793b2ade9faf0f9c04e9dbf84860f03446b6)), closes [#2114](https://github.com/stryker-mutator/stryker-net/issues/2114)
* **Console print:** Escape paths with Spectre.Console to prevent Exception ([#2113](https://github.com/stryker-mutator/stryker-net/issues/2113)) ([11d627c](https://github.com/stryker-mutator/stryker-net/commit/11d627cee9c736d1dd63db4fb188c075caeb6a75))
* **Ignore mutants comments:** Stryker properly ignores mutants with comments on static fields ([#2088](https://github.com/stryker-mutator/stryker-net/issues/2088)) ([cb40c0a](https://github.com/stryker-mutator/stryker-net/commit/cb40c0a90fd41571ec8eb84cb729df95763f02b8))



# [2.0.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.5.3...stryker@2.0.0) (2022-06-01)


### Bug Fixes

* **initialisation:** Improve error message when no analyzer results are found ([#2037](https://github.com/stryker-mutator/stryker-net/issues/2037)) ([6ad8afb](https://github.com/stryker-mutator/stryker-net/commit/6ad8afb1d1a324a6c86e1cdba11ddf270b256efb))
* **initialisation:** Improve error messages when we fail to correctly analyze the project ([#2030](https://github.com/stryker-mutator/stryker-net/issues/2030)) ([305a713](https://github.com/stryker-mutator/stryker-net/commit/305a7139bfbcbda61e7492e93720314529b83a96))
* **reports:** Fix issue with relative paths that miss the root folder ([#2055](https://github.com/stryker-mutator/stryker-net/issues/2055)) ([4698513](https://github.com/stryker-mutator/stryker-net/commit/46985134192ade92d5e56dc2ce45681ff92e41c9))



## [1.5.3](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.5.2...stryker@1.5.3) (2022-05-03)


### Bug Fixes

* **baseline:** fixed issue where invalid header was used for creating azure fileshare file ([#2027](https://github.com/stryker-mutator/stryker-net/issues/2027)) ([7901c33](https://github.com/stryker-mutator/stryker-net/commit/7901c3353a2af27f5680255a49c75c7f94bdba09))
* **reporters:** support spaces in report pathname ([#2010](https://github.com/stryker-mutator/stryker-net/issues/2010)) ([1bc8afa](https://github.com/stryker-mutator/stryker-net/commit/1bc8afa92d403fcdadda03bcf4a0e303167e6b94))
* **since:** Add possibility to specify a git tag for the since.target option ([#1994](https://github.com/stryker-mutator/stryker-net/issues/1994)) ([12e3d9b](https://github.com/stryker-mutator/stryker-net/commit/12e3d9bc974fa9f7bc524ecfbe349360e0bfcec5))
* **testrunner:** Add default timespan for not executed duplicated xunit tests ([#2020](https://github.com/stryker-mutator/stryker-net/issues/2020)) ([2cbbfcd](https://github.com/stryker-mutator/stryker-net/commit/2cbbfcd3e750ae54c8d7f7f2c4724f1a29880dd7))



## [1.5.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.5.1...stryker@1.5.2) (2022-04-21)


### Bug Fixes

* **ignore-mutations:** Support mutator names as well as descriptions ([#2006](https://github.com/stryker-mutator/stryker-net/issues/2006)) ([f3fb5c6](https://github.com/stryker-mutator/stryker-net/commit/f3fb5c6139f6d656d43a2c2a271821254a039a66))
* **testrunner:** Properly handle execution time for xUnit theories ([#2004](https://github.com/stryker-mutator/stryker-net/issues/2004)) ([92aec17](https://github.com/stryker-mutator/stryker-net/commit/92aec175d9bb6b1aae4d0d4556422669bf8bfeb8))



## [1.5.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.5.0...stryker@1.5.1) (2022-04-12)


### Bug Fixes

* **baseline:** potential InvalidOperationException: Sequence contains no elements ([#1993](https://github.com/stryker-mutator/stryker-net/issues/1993)) ([f3260f4](https://github.com/stryker-mutator/stryker-net/commit/f3260f4d915b0622928b92a252eca7a81719adf8))
* **mutating:** Better support for mutating local function ([#1987](https://github.com/stryker-mutator/stryker-net/issues/1987)) ([6491298](https://github.com/stryker-mutator/stryker-net/commit/6491298a9466525d4465c9e35e06c8b393b7bbb3))
* properly restore storage context when leaving anonymous function. Fix some NREs and mutation loss ([#1980](https://github.com/stryker-mutator/stryker-net/issues/1980)) ([302c9d9](https://github.com/stryker-mutator/stryker-net/commit/302c9d93a72ac1af2b9b5bdd06fca97854b14e5b))
* **testrunner:** Improve handling of frozen VsTest runners ([#1977](https://github.com/stryker-mutator/stryker-net/issues/1977)) ([1d3310b](https://github.com/stryker-mutator/stryker-net/commit/1d3310b41f83ee467264602eece3821b36677d01))



# [1.5.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.4.2...stryker@1.5.0) (2022-03-15)


### Bug Fixes

* **filters:** ExcludeFromCodeCoverage not working when justification is given ([#1964](https://github.com/stryker-mutator/stryker-net/issues/1964)) ([a562149](https://github.com/stryker-mutator/stryker-net/commit/a56214959eae7c0b578f54c55970dc15c3891ffb)), closes [#1957](https://github.com/stryker-mutator/stryker-net/issues/1957)


### Features

* **reporting:** Make links clickable in terminals that support Ansi magic ([#1936](https://github.com/stryker-mutator/stryker-net/issues/1936)) ([54b9872](https://github.com/stryker-mutator/stryker-net/commit/54b98724d5651d993f7d27753b70369ef0a1a2ce))



## [1.4.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.4.1...stryker@1.4.2) (2022-03-05)


### Bug Fixes

* **regex mutations:** Revert inclusion of referenced projects because transitive dependencies are not included ([#1950](https://github.com/stryker-mutator/stryker-net/issues/1950)) ([69a80ee](https://github.com/stryker-mutator/stryker-net/commit/69a80ee75e082c1343a068ad6e3be4faf2edbd7a))



## [1.4.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.4.0...stryker@1.4.1) (2022-03-04)


### Bug Fixes

* **packaging:** Include referenced projects in nupkg ([#1947](https://github.com/stryker-mutator/stryker-net/issues/1947)) ([e7427ce](https://github.com/stryker-mutator/stryker-net/commit/e7427cee807df53388e6b009052b35b6a76881cf))
* **Rollback:** Null Reference Exception during mutation rollback ([#1940](https://github.com/stryker-mutator/stryker-net/issues/1940)) ([5b71ad0](https://github.com/stryker-mutator/stryker-net/commit/5b71ad07c0c2d173cb7a126d8d6a0e77825b67f8))



# [1.4.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.3.1...stryker@1.4.0) (2022-03-02)


### Bug Fixes

* **initialisation:** Make sure to select valid IAnalyzerResult from Buildalyzer ([#1900](https://github.com/stryker-mutator/stryker-net/issues/1900)) ([da2cd52](https://github.com/stryker-mutator/stryker-net/commit/da2cd52c20bc1d8a5863ce6f9eaf901d8d8fade7)), closes [#1899](https://github.com/stryker-mutator/stryker-net/issues/1899)
* **initialisation:** Use the NuGet.Frameworks package to parse target frameworks ([#1906](https://github.com/stryker-mutator/stryker-net/issues/1906)) ([556e1ae](https://github.com/stryker-mutator/stryker-net/commit/556e1aedd1b3179e6b082acf7b7081ea6d6286fc)), closes [/github.com/stryker-mutator/stryker-net/pull/1905/commits/6d9c0a95adfa50df4f78a800439321df7833a95b#diff-7ac954b3982a221de06b52bcdde548ea509a15bfc79fef82cf7b87071a223197](https://github.com//github.com/stryker-mutator/stryker-net/pull/1905/commits/6d9c0a95adfa50df4f78a800439321df7833a95b/issues/diff-7ac954b3982a221de06b52bcdde548ea509a15bfc79fef82cf7b87071a223197) [#1905](https://github.com/stryker-mutator/stryker-net/issues/1905)
* **mutation placing:** Support statement mutations in expression bodies lambdas ([#1905](https://github.com/stryker-mutator/stryker-net/issues/1905)) ([35c8374](https://github.com/stryker-mutator/stryker-net/commit/35c837480f1fbfcc6b7b2ce5bd808bc1b94af82b))
* **ProcessorCount:** Guard against `ProcessorCount` reporting values lower then `1` ([#1930](https://github.com/stryker-mutator/stryker-net/issues/1930)) ([c378d63](https://github.com/stryker-mutator/stryker-net/commit/c378d6300a936ac38d6592a537213746b30d69b9))
* **progressbar reporter:** fix NRE in the case of a mutant free world ([#1882](https://github.com/stryker-mutator/stryker-net/issues/1882)) ([249e204](https://github.com/stryker-mutator/stryker-net/commit/249e2042f0235cc7b54898b8f986fb901468b03b))
* **rollback:** Improve stability by rollbacking block mutations before all mutations are removed ([#1875](https://github.com/stryker-mutator/stryker-net/issues/1875)) ([2446ef0](https://github.com/stryker-mutator/stryker-net/commit/2446ef02847f687a63650d6a19647fffc937e70b))


### Features

* Allow specifying test projects as commandline arguments ([#1929](https://github.com/stryker-mutator/stryker-net/issues/1929)) ([072b456](https://github.com/stryker-mutator/stryker-net/commit/072b4568a8bc1180fa85f9691a75110783ce37cf))



# [1.3.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.3.0...stryker@1.3.1) (2022-01-21)


### Bug Fixes

* stackoverflow exceptions due to ignoredmethods filter ([#1879](https://github.com/stryker-mutator/stryker-net/issues/1879)) ([bdd8692](https://github.com/stryker-mutator/stryker-net/commit/bdd86920b613e85cf738fe00b7c885e4ec9ceee0))



# [1.3.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.2.2...stryker@1.3.0) (2022-01-20)


### Bug Fixes

* block mutator no longer mutates case section ([fe9ca13](https://github.com/stryker-mutator/stryker-net/commit/fe9ca1395ff95c5d773567c1211a13e23bd83a76))
* mutation testing for self-contained apps ([#1868](https://github.com/stryker-mutator/stryker-net/issues/1868)) ([9ce18d1](https://github.com/stryker-mutator/stryker-net/commit/9ce18d19b4c01e0581c2cca2149fe590dc2a2f0f))
* order of mutation roll back is now correct ([a4d2c1c](https://github.com/stryker-mutator/stryker-net/commit/a4d2c1cca0abe2f6ecb1aa1e83d4a41bf2964a79))


### Features

* Ignoredmethod filters now properly filters out method and block mutations ([fea6d6e](https://github.com/stryker-mutator/stryker-net/commit/fea6d6efd4d13cfc1da060b446c2159522e4f774))



# [1.2.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.2.1...stryker@1.2.2) (2022-01-11)


### Bug Fixes

* **Indexer methods:** Prevents rollback on indexer methods ([#1843](https://github.com/stryker-mutator/stryker-net/issues/1843)) ([ecb6df9](https://github.com/stryker-mutator/stryker-net/commit/ecb6df9750545b507313d0bffc5bee91bdb23f08))
* **Project filter:** Project filter works with both forward and backward slashes ([#1819](https://github.com/stryker-mutator/stryker-net/issues/1819)) ([6a6f302](https://github.com/stryker-mutator/stryker-net/commit/6a6f3026ccc258d3ae3fde63911511ac314efd93))
* **Reporters:** Stryker no longer throws exception on no mutants found ([#1833](https://github.com/stryker-mutator/stryker-net/issues/1833)) ([991497a](https://github.com/stryker-mutator/stryker-net/commit/991497a9b3fd7280a7d48d77caf29130049fdba1))
* **reporting:** Result status reason was filled with NotRun for mutants that were run ([#1851](https://github.com/stryker-mutator/stryker-net/issues/1851)) ([d8292b9](https://github.com/stryker-mutator/stryker-net/commit/d8292b951dc48a3a65b82e5b3b267c20603b1491))


### Reverts

* Revert "Add test" ([6cc5580](https://github.com/stryker-mutator/stryker-net/commit/6cc5580d25195eaaf022e8afe019e6f3cf816969))



# [1.2.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.2.0...stryker@1.2.1) (2021-12-15)


### Bug Fixes

* **Load preprocessor symbols:** Correctly load preprocessor symbols so they can be used ([#1836](https://github.com/stryker-mutator/stryker-net/issues/1836)) ([58bc927](https://github.com/stryker-mutator/stryker-net/commit/58bc927ab740db3294e16ce0af84ec0db28ab83d))



# [1.2.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.1.0...stryker@1.2.0) (2021-12-10)


### Bug Fixes

* **Assembly resolving:** Embedded resources resolving no longer uses Immediate mode. Fixes Xamarin support. ([#1816](https://github.com/stryker-mutator/stryker-net/issues/1816)) ([c1e5087](https://github.com/stryker-mutator/stryker-net/commit/c1e50877ed936353b3fede63b16520421f8a1a2f))


### Features

* **html report:** New diff view in HTML report ([#1818](https://github.com/stryker-mutator/stryker-net/issues/1818)) ([9b5ac6c](https://github.com/stryker-mutator/stryker-net/commit/9b5ac6c452895548d6c4bcb646d426441bd10549))
* **reporting:** Open dashboard report with --open-report ([#1795](https://github.com/stryker-mutator/stryker-net/issues/1795)) ([75cbec2](https://github.com/stryker-mutator/stryker-net/commit/75cbec233ef24d326db04c66eade384851964118))
* **reporting:** Report when a mutant is in a static context ([#1820](https://github.com/stryker-mutator/stryker-net/issues/1820)) ([f267b61](https://github.com/stryker-mutator/stryker-net/commit/f267b619b62019736c8a3b8bd3d1946bd60fefd2))



# [1.1.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.0.1...stryker@1.1.0) (2021-11-16)


### Bug Fixes

* **baseline:** Use regular fallback version as well as baseline fallback version ([#1800](https://github.com/stryker-mutator/stryker-net/issues/1800)) ([c65dd78](https://github.com/stryker-mutator/stryker-net/commit/c65dd788354767b6edae85e3eb87c8a06d522a40))
* **inline ignore mutations:** Trailing whitespace is ignored during disabled comment parsing ([#1786](https://github.com/stryker-mutator/stryker-net/issues/1786)) ([8d4c5b0](https://github.com/stryker-mutator/stryker-net/commit/8d4c5b0a738b847499451ec16b46206cd91d363e)), closes [#1776](https://github.com/stryker-mutator/stryker-net/issues/1776) [#1781](https://github.com/stryker-mutator/stryker-net/issues/1781)
* **options:** Auto enable baseline reporter when baseline is enabled ([#1799](https://github.com/stryker-mutator/stryker-net/issues/1799)) ([24131cc](https://github.com/stryker-mutator/stryker-net/commit/24131cc71b2341e49d50a4ba5f3f13ec2aef5293))
* **reporting:** Generate mutation reports even if there are no mutants to test ([982c30d](https://github.com/stryker-mutator/stryker-net/commit/982c30d7d28d7d0081cba66ca5fc37f34c49cc07))
* **since:** Mutants ignored from source code comment are no longer re-enabled by the since filter ([#1804](https://github.com/stryker-mutator/stryker-net/issues/1804)) ([b0f5fbb](https://github.com/stryker-mutator/stryker-net/commit/b0f5fbbe94ca019629f03e961c9220f6a0caa86e))


### Features

* **configuration:** Option to set report file name ([#1755](https://github.com/stryker-mutator/stryker-net/issues/1755)) ([2a1c564](https://github.com/stryker-mutator/stryker-net/commit/2a1c56439783a121c5d0962183f617554860e1da))
* **reporting:** Add option to automatically open the mutation report in the browser ([#1750](https://github.com/stryker-mutator/stryker-net/issues/1750)) ([6b2ec48](https://github.com/stryker-mutator/stryker-net/commit/6b2ec487c0e275a219ba64fabed2171b23e36b84))
* Support csharp10 syntaxes ([#1792](https://github.com/stryker-mutator/stryker-net/issues/1792)) ([a7d401b](https://github.com/stryker-mutator/stryker-net/commit/a7d401b781200965fcaf4b93950ed297635676ed))



# [1.0.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@1.0.0...stryker@1.0.1) (2021-11-02)


### Bug Fixes

* **block mutator:** Prevent false positives from block mutations ([#1759](https://github.com/stryker-mutator/stryker-net/issues/1759)) ([769ee79](https://github.com/stryker-mutator/stryker-net/commit/769ee797e0141d24e2901a472641c4c0abcd6bc3))
* **Ignore mutations:** Ignoring statement mutations is now possible ([#1764](https://github.com/stryker-mutator/stryker-net/issues/1764)) ([42a0a7e](https://github.com/stryker-mutator/stryker-net/commit/42a0a7e59a666fc99692e1eb736f0e3396032f22))
* **version undefined:** Remove version suffix "undefined" from stryker version ([#1765](https://github.com/stryker-mutator/stryker-net/issues/1765)) ([e4d4f9d](https://github.com/stryker-mutator/stryker-net/commit/e4d4f9dec7ff2ca92082a300a68733bcba043ad8))



# [1.0.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.11...stryker@1.0.0) (2021-10-30)


### Bug Fixes

* Dashboard baseline project name and version are not set ([#1710](https://github.com/stryker-mutator/stryker-net/issues/1710)) ([e2523e4](https://github.com/stryker-mutator/stryker-net/commit/e2523e4472d9abd68ae48c0951cded3a83f56d28))
* Disable copying to clipboard when using Verify framework ([#1709](https://github.com/stryker-mutator/stryker-net/issues/1709)) ([ccfb1a2](https://github.com/stryker-mutator/stryker-net/commit/ccfb1a2049a88bc75ffbab0a8fee78929ef6d0f6))
* Ignore methods input was not read from config file ([#1742](https://github.com/stryker-mutator/stryker-net/issues/1742)) ([4924379](https://github.com/stryker-mutator/stryker-net/commit/4924379db5ad532af8e222fd53e80529c7ed9a72))
* Improve errors messages when configuration contains invalid config keys ([#1707](https://github.com/stryker-mutator/stryker-net/issues/1707)) ([89d6379](https://github.com/stryker-mutator/stryker-net/commit/89d63792f4984c9ede7635a42cdb8bf0720c12d6)), closes [/github.com/microsoft/vstest/issues/2488#issuecomment-932036883](https://github.com//github.com/microsoft/vstest/issues/2488/issues/issuecomment-932036883)
* **initialization:** allow running stryker on the project under test from an unrelated working directory ([#1708](https://github.com/stryker-mutator/stryker-net/issues/1708)) ([b7884f8](https://github.com/stryker-mutator/stryker-net/commit/b7884f8622dae61bc27e9727a47dc15ea9b1086b)), closes [#1039](https://github.com/stryker-mutator/stryker-net/issues/1039)
* Loading Source Generators no longer leads to errors on loading unrelated Analyzers ([#1703](https://github.com/stryker-mutator/stryker-net/issues/1703)) ([60a3eaf](https://github.com/stryker-mutator/stryker-net/commit/60a3eafabb6b0ba2d21840c3720c595cb67105ff))
* **options:** change logic that determines if the dashboard is enable… ([#1688](https://github.com/stryker-mutator/stryker-net/issues/1688)) ([e7b1dfa](https://github.com/stryker-mutator/stryker-net/commit/e7b1dfa4b27655d7f2c32f62123134a286489c35))
* Set the project name option if input is supplied even if the dashboard reporter is not turned on ([6c982b2](https://github.com/stryker-mutator/stryker-net/commit/6c982b2f54ebf5d2f2f719fe75ef040da93bc05f))
* Set the project version option if input is supplied even if the dashboard reporter is not turned on ([9e5579f](https://github.com/stryker-mutator/stryker-net/commit/9e5579f98cbe16879aa37f8364a0eac61cd12904))


### Features

* **Allow failing tests:** Failing tests are now allowed during the initial testrun ([#1542](https://github.com/stryker-mutator/stryker-net/issues/1542)) ([2a26b2b](https://github.com/stryker-mutator/stryker-net/commit/2a26b2bd6098f0115005f24a52d3d5dbc9a7622c))
* **Block removal mutator:** Add block removal mutator ([#1717](https://github.com/stryker-mutator/stryker-net/issues/1717)) ([13ed1ab](https://github.com/stryker-mutator/stryker-net/commit/13ed1abe95bdfd296a4bc5b86a98796024ea4cd3))
* Cache and restore unmutated assembly so rebuild is not needed after mutation test ([#1516](https://github.com/stryker-mutator/stryker-net/issues/1516)) ([2987df5](https://github.com/stryker-mutator/stryker-net/commit/2987df53580f013b33dcb08c74863cc91d6cb236))
* **dashboard reporter:** Retrieve the project name and version from the project under test using source link ([#1663](https://github.com/stryker-mutator/stryker-net/issues/1663)) ([6eec110](https://github.com/stryker-mutator/stryker-net/commit/6eec110a57febfd16a026165bc2f7f2f7aad6e9d))
* **filtering:** Mutation can be controlled at the source code level ([#1583](https://github.com/stryker-mutator/stryker-net/issues/1583)) ([bd3fc4e](https://github.com/stryker-mutator/stryker-net/commit/bd3fc4e6b0500bb42224b54b95e08f1465b2fa09))
* **ignore-mutations:** Allow ignoring specific linq mutations ([#1660](https://github.com/stryker-mutator/stryker-net/issues/1660)) ([84d3995](https://github.com/stryker-mutator/stryker-net/commit/84d3995ff3bdda530922c165aaf8e19af8cd8701))
* **initial-build:** Allow custom msbuild path ([#1363](https://github.com/stryker-mutator/stryker-net/issues/1363)) ([64c99ee](https://github.com/stryker-mutator/stryker-net/commit/64c99ee4f6732632fd880c177392ac9e11c1316d))
* **multi-target:** Prepare project analyzer for multi-target support ([#1495](https://github.com/stryker-mutator/stryker-net/issues/1495)) ([9f443b9](https://github.com/stryker-mutator/stryker-net/commit/9f443b9e8e68f994d13d12ab98767bcae77cb4ef))
* Rework how stryker consumes options ([#1273](https://github.com/stryker-mutator/stryker-net/issues/1273)) ([35d8c24](https://github.com/stryker-mutator/stryker-net/commit/35d8c2450b8472c86efd6f7991dc30d0c9738c87))
* **statement mutator:** Add mutator that removes statements ([#1472](https://github.com/stryker-mutator/stryker-net/issues/1472)) ([feacbc9](https://github.com/stryker-mutator/stryker-net/commit/feacbc925ec9418728860431c1fdee682bd95249)), closes [#1470](https://github.com/stryker-mutator/stryker-net/issues/1470) [#1470](https://github.com/stryker-mutator/stryker-net/issues/1470) [#1470](https://github.com/stryker-mutator/stryker-net/issues/1470) [#1470](https://github.com/stryker-mutator/stryker-net/issues/1470)
* **String mutations:** Don't mutate strings as string literal when they contain regex ([#1746](https://github.com/stryker-mutator/stryker-net/issues/1746)) ([f6662d4](https://github.com/stryker-mutator/stryker-net/commit/f6662d43b5a589a2852e41d95bff49cc8b6c4bb9))
* **target-framework:** Add an option to specify target framework ([#1684](https://github.com/stryker-mutator/stryker-net/issues/1684)) ([83d2beb](https://github.com/stryker-mutator/stryker-net/commit/83d2bebe9a64f8c555e599b77f40da6b323ca95c))
* **testrunner:** Filter which unit tests are used during mutation test ([#1669](https://github.com/stryker-mutator/stryker-net/issues/1669)) ([7e74862](https://github.com/stryker-mutator/stryker-net/commit/7e74862e6eba2b9b9c19f4524d381bc5ebe35e30))
* update targetframework from netcoreapp3.1 to net5.0 ([#1462](https://github.com/stryker-mutator/stryker-net/issues/1462)) ([b02a0bb](https://github.com/stryker-mutator/stryker-net/commit/b02a0bb3559d6afa8f201f5b692b96db6c572d96))


### BREAKING CHANGES

* All options have been reworked. Your existing commandline and json config will most likely no longer work.

Co-authored-by: Caspar van Doornmalen <casparvandoornmalen@gmail.com>
Co-authored-by: Richard Werkman <Richard.Werkman@infosupport.com>
Co-authored-by: Richard Werkman <s1084402@student.windesheim.nl>
Co-authored-by: Sakari Bergen <s@beatwaves.net>
Co-authored-by: Peter Semkin <psfinaki@users.noreply.github.com>
Co-authored-by: Richard Werkman <Richard1158@gmail.com>



## [0.22.11](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.10...stryker@0.22.11) (2021-09-17)


### Bug Fixes

* downgrade some exceptions to reduce noise due to analyzes ([#1678](https://github.com/stryker-mutator/stryker-net/issues/1678)) ([1cba077](https://github.com/stryker-mutator/stryker-net/commit/1cba07748cb2121429b03dce9d59d8137702b309))

## [0.22.10](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.9...stryker@0.22.10) (2021-08-25)


### Bug Fixes

* Allow setting threshold break to 100 ([#1674](https://github.com/stryker-mutator/stryker-net/issues/1674)) ([4cd1945](https://github.com/stryker-mutator/stryker-net/commit/4cd1945621da08a9c6794ff9415096e68c306216))
* Return exit code 2 for threshold break violations ([#1673](https://github.com/stryker-mutator/stryker-net/issues/1673)) ([6da4e7d](https://github.com/stryker-mutator/stryker-net/commit/6da4e7d16e00aa90860e59a3875539c18bbbe757))


### Features

* **mutation engine:** Add full support for local functions ([#1664](https://github.com/stryker-mutator/stryker-net/issues/1664)) ([b689358](https://github.com/stryker-mutator/stryker-net/commit/b6893581edfbcffb5b752c262b391b19b4bf89ac))



## [0.22.10](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.9...stryker@0.22.10) (2021-08-25)


### Bug Fixes

* Allow setting threshold break to 100 ([#1674](https://github.com/stryker-mutator/stryker-net/issues/1674)) ([4cd1945](https://github.com/stryker-mutator/stryker-net/commit/4cd1945621da08a9c6794ff9415096e68c306216))
* Return exit code 2 for threshold break violations ([#1673](https://github.com/stryker-mutator/stryker-net/issues/1673)) ([6da4e7d](https://github.com/stryker-mutator/stryker-net/commit/6da4e7d16e00aa90860e59a3875539c18bbbe757))



## [0.22.9](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.8...stryker@0.22.9) (2021-08-03)


### Bug Fixes

* **Ignore methods:** Allow ignoring mutants of fully qualified method names ([#1635](https://github.com/stryker-mutator/stryker-net/issues/1635)) ([ddfc72f](https://github.com/stryker-mutator/stryker-net/commit/ddfc72f1c95f1302fb6f0bab7cf3e48b7b85b505))



## [0.22.8](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.7...stryker@0.22.8) (2021-07-26)


### Bug Fixes

* **Compiling:** Added support for C# source generators. ([#1617](https://github.com/stryker-mutator/stryker-net/issues/1617)) ([98c7669](https://github.com/stryker-mutator/stryker-net/commit/98c7669d8f643c2b51ca176952d6339fb3e80acf))



## [0.22.7](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.6...stryker@0.22.7) (2021-07-10)


### Bug Fixes

* **Diff feature:** Coverage analysis filter overwrites Ignored mutant statuses while it should ignore them ([#1586](https://github.com/stryker-mutator/stryker-net/issues/1586))

## [0.22.6](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.5...stryker@0.22.6) (2021-06-30)


### Bug Fixes

* **Conversion operator:** Stryker won't break anymore when using conversion operator ([#1613](https://github.com/stryker-mutator/stryker-net/issues/1613)) ([a09f2cc](https://github.com/stryker-mutator/stryker-net/commit/a09f2cc8c55f76ffcab8964e477334ff3ca75882))



## [0.22.5](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.4...stryker@0.22.5) (2021-06-22)


### Bug Fixes

* **Dotnet 5 support:** net5.0 version parsing ([#1592](https://github.com/stryker-mutator/stryker-net/issues/1592)) ([a92fa86](https://github.com/stryker-mutator/stryker-net/commit/a92fa86b65402cdcdb6a27cf1c55d2f0c8f0b684))
* **Ignore method:** Ignore method when not called on an object ([#1579](https://github.com/stryker-mutator/stryker-net/issues/1579)) ([eb87d07](https://github.com/stryker-mutator/stryker-net/commit/eb87d0702081848304cb679b6f1f9a2b85d3962a))



## [0.22.4](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.2...stryker@0.22.4) (2021-05-14)


### Bug Fixes

* Improve reliability of embedded resources resolving ([#1564](https://github.com/stryker-mutator/stryker-net/issues/1564)) ([6991b60](https://github.com/stryker-mutator/stryker-net/commit/6991b60d1206f1427cdf312a95c79d880402d0f5))
* **baseline:** Allow valid version strings like 2.0.0 ([#1514](https://github.com/stryker-mutator/stryker-net/issues/1514)) ([b740bba](https://github.com/stryker-mutator/stryker-net/commit/b740bba44fab7e6e4673743d1d46fcc45da28cd5))
* **baseline:** Azure fileshare uses projectname as output path if specified ([#1526](https://github.com/stryker-mutator/stryker-net/issues/1526)) ([de8940a](https://github.com/stryker-mutator/stryker-net/commit/de8940aba22489eb51ff063c0a2fd5a587a83679))
* **filtered mutants reporter:** OnMutantsCreated was executed before filtering mutants ([64b4ace](https://github.com/stryker-mutator/stryker-net/commit/64b4ace60b324156305e96af4be7f071ffd8c83a))
* **mutation orchestration:** rollback of helpers and faulty array initializers to prevent unrecoverable compile errors ([#1530](https://github.com/stryker-mutator/stryker-net/issues/1530)) ([2b6af15](https://github.com/stryker-mutator/stryker-net/commit/2b6af15a951a50d5271f8f58848d7b7e6b14097d))
* **rollback:** rollback mutation in expressions that cause build error(s) ([#1539](https://github.com/stryker-mutator/stryker-net/issues/1539)) ([83a8082](https://github.com/stryker-mutator/stryker-net/commit/83a80829edb23b1a816d1537927f05a098d1e284))
* Automatically disable DiffEngine when running tests ([#1523](https://github.com/stryker-mutator/stryker-net/issues/1523)) ([c37fafc](https://github.com/stryker-mutator/stryker-net/commit/c37fafc8b1ecf0a6ef119514b1bbdc1e225568ae))



## [0.22.3](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.2...stryker@0.22.3) (2021-04-17)


### Bug Fixes

* **baseline:** Allow valid version strings like 2.0.0 ([#1514](https://github.com/stryker-mutator/stryker-net/issues/1514)) ([b740bba](https://github.com/stryker-mutator/stryker-net/commit/b740bba44fab7e6e4673743d1d46fcc45da28cd5))



## [0.22.2](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.1...stryker@0.22.2) (2021-04-15)


### Bug Fixes

* **mutating:** Place yield return instead of yield return default(type) for yield break ([f791ad6](https://github.com/stryker-mutator/stryker-net/commit/f791ad6562ad162c3b067a7d02f6d3179303e90b))
* Default vstest extensions are no longer required on the filesystem ([#1500](https://github.com/stryker-mutator/stryker-net/issues/1500)) ([554bc38](https://github.com/stryker-mutator/stryker-net/commit/554bc38afc147983a23118b8b2b6128a842bbadb))



## [0.22.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.22.0...stryker@0.22.1) (2021-03-22)


### Bug Fixes

* **solution run:** Fix NPE on solution runs ([#1479](https://github.com/stryker-mutator/stryker-net/issues/1479)) ([5cdac27](https://github.com/stryker-mutator/stryker-net/commit/5cdac275167ebc6b5016535db22af2cae0c4377b))
* local declaration mutations are promoted from statement to block level mutations ([#1427](https://github.com/stryker-mutator/stryker-net/issues/1427)) ([c440de0](https://github.com/stryker-mutator/stryker-net/commit/c440de05aaf5c37270e60aa009c49a5742cde33e)), closes [#1368](https://github.com/stryker-mutator/stryker-net/issues/1368) [#1423](https://github.com/stryker-mutator/stryker-net/issues/1423)



# [0.22.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.21.1...stryker@0.22.0) (2021-03-05)


### Bug Fixes

* Support newer operating systems with git diff ([#1459](https://github.com/stryker-mutator/stryker-net/issues/1459)) ([48a1ae2](https://github.com/stryker-mutator/stryker-net/commit/48a1ae224b6c19bb954de4d69e86e17d277e7bfd))
* **Diff feature:** Diff feature could not be enabled ([#1458](https://github.com/stryker-mutator/stryker-net/issues/1458)) ([782302f](https://github.com/stryker-mutator/stryker-net/commit/782302fd79ac1c8cceb88e64cf0e934cbdda51e3))
* **mutation placing:** mutations may leak between body formed methods/properties ([#1389](https://github.com/stryker-mutator/stryker-net/issues/1389)) ([91dbb8b](https://github.com/stryker-mutator/stryker-net/commit/91dbb8b8e7cdee0b10b6481b3dd3169efc768c1f))
* **reporting:** report on mutations to test after filtering out mutations we should not test ([#1398](https://github.com/stryker-mutator/stryker-net/issues/1398)) ([1523a01](https://github.com/stryker-mutator/stryker-net/commit/1523a012bdfd9b9ea81f91442849f695518474c4))


### Features

* **Clickable file paths:** filepaths will be detected by supported terminals ([#1403](https://github.com/stryker-mutator/stryker-net/issues/1403)) ([f24d55c](https://github.com/stryker-mutator/stryker-net/commit/f24d55c5e7401175ca31d1ba33528438fe7406dd))
* **Filtered mutant reporter:** Move FilterMutations status logging to reporter so the logging can be turned off ([#1275](https://github.com/stryker-mutator/stryker-net/issues/1275)) ([68591ec](https://github.com/stryker-mutator/stryker-net/commit/68591ecd1ad293ef6a330f654bb32366059a5cf0))
* **json report:** Add absolute path to json report [#1154](https://github.com/stryker-mutator/stryker-net/issues/1154) ([#1267](https://github.com/stryker-mutator/stryker-net/issues/1267)) ([8b72975](https://github.com/stryker-mutator/stryker-net/commit/8b72975f7dc215998bdc222a29fd6126fe0ece28))
* **progressbar reporter:**  Switch to ShellProgressBar for console progress bar report ([#1286](https://github.com/stryker-mutator/stryker-net/issues/1286)) ([3fa0c68](https://github.com/stryker-mutator/stryker-net/commit/3fa0c687aa7fcf5be801aace2ef164844a0dade3))



## [0.21.1](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.21.0...stryker@0.21.1) (2021-01-23)


### Bug Fixes

* **MutantFilters:** Fix mutant filters were not being called ([#1378](https://github.com/stryker-mutator/stryker-net/issues/1378)) ([4314bad](https://github.com/stryker-mutator/stryker-net/commit/4314bada0d8aa5817ea71a265ecc1c7d519a8f13))



# [0.21.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.20.0...stryker@0.21.0) (2021-01-20)


### Bug Fixes

* **dashboard compare:** prevent NPE for compiler error mutants ([#1293](https://github.com/stryker-mutator/stryker-net/issues/1293)) ([7d4d023](https://github.com/stryker-mutator/stryker-net/commit/7d4d0231a9de4d137133d45814d9418cdff0ca73))
* **dashboard compare:** verify the name of target branch ([#1305](https://github.com/stryker-mutator/stryker-net/issues/1305)) ([528bff6](https://github.com/stryker-mutator/stryker-net/commit/528bff6be1fd6f2dcd62f2be6b94526cc66a3eb8)), closes [#1303](https://github.com/stryker-mutator/stryker-net/issues/1303)
* **diff:** improve the change test filter ([#1306](https://github.com/stryker-mutator/stryker-net/issues/1306)) ([eccb48f](https://github.com/stryker-mutator/stryker-net/commit/eccb48fe8278ebaf5dbbd0d0badc2453ae430893)), closes [#1304](https://github.com/stryker-mutator/stryker-net/issues/1304)
* **filter mutants:** do not pass Compile Error mutants to filters ([#1295](https://github.com/stryker-mutator/stryker-net/issues/1295)) ([#1298](https://github.com/stryker-mutator/stryker-net/issues/1298)) ([21197a4](https://github.com/stryker-mutator/stryker-net/commit/21197a4cdf56a97bbaa6e8af6eef1111f066d1ee))


### Features

* **initial-build:** Allow solutionpath for dotnet core ([#1362](https://github.com/stryker-mutator/stryker-net/issues/1362)) ([6a051c9](https://github.com/stryker-mutator/stryker-net/commit/6a051c9aa32daa8e8705ef9139310babc4db0b7b))
* add expression to body conversion for properties ([#1324](https://github.com/stryker-mutator/stryker-net/issues/1324)) ([be43d22](https://github.com/stryker-mutator/stryker-net/commit/be43d2274cc64fdace916cbc5dea2b8e455a5d0c)), closes [#1039](https://github.com/stryker-mutator/stryker-net/issues/1039) [#1348](https://github.com/stryker-mutator/stryker-net/issues/1348) [#1351](https://github.com/stryker-mutator/stryker-net/issues/1351) [#1350](https://github.com/stryker-mutator/stryker-net/issues/1350) [#1353](https://github.com/stryker-mutator/stryker-net/issues/1353) [#1352](https://github.com/stryker-mutator/stryker-net/issues/1352) [#1356](https://github.com/stryker-mutator/stryker-net/issues/1356) [#1355](https://github.com/stryker-mutator/stryker-net/issues/1355) [#1359](https://github.com/stryker-mutator/stryker-net/issues/1359) [#1361](https://github.com/stryker-mutator/stryker-net/issues/1361) [#1364](https://github.com/stryker-mutator/stryker-net/issues/1364)
* **Html reporter:** Enable dark mode for html report ([#1287](https://github.com/stryker-mutator/stryker-net/issues/1287)) ([d84072a](https://github.com/stryker-mutator/stryker-net/commit/d84072a39da6c4566e7bafc68a76eea86c9d9fa9))
* **initial-build:** Improve error message when initial build fails ([#1364](https://github.com/stryker-mutator/stryker-net/issues/1364)) ([91ae959](https://github.com/stryker-mutator/stryker-net/commit/91ae9599e8f06571737e43ca5b8844549008b0c1))
* **Multi project runs:** Multi project runs using solution file ([#1039](https://github.com/stryker-mutator/stryker-net/issues/1039)) ([e1b4bf3](https://github.com/stryker-mutator/stryker-net/commit/e1b4bf3b031fac3462196df553ac8a1f13392302))
* **mutation orchestration:** Improve mutation orchestration and rollback logic ([#1236](https://github.com/stryker-mutator/stryker-net/issues/1236)) ([ffc4860](https://github.com/stryker-mutator/stryker-net/commit/ffc4860a105fae836a4e98ba83cf0138d941e128))



# [0.20.0](https://github.com/stryker-mutator/stryker-net/compare/stryker@0.19.0...stryker@0.20.0) (2020-10-19)


### Bug Fixes

* **diff compare:** allow canonical branch name as diff target ([#1210](https://github.com/stryker-mutator/stryker-net/issues/1210)) ([0b9659f](https://github.com/stryker-mutator/stryker-net/commit/0b9659faee9af47584cfc53d807380a6edf48a02))
* **git diff:** exclude stryker generated files ([#1199](https://github.com/stryker-mutator/stryker-net/issues/1199)) ([4c1a749](https://github.com/stryker-mutator/stryker-net/commit/4c1a7495d8649d448fd280bc4f77ef59c14681b8))
* **git diff filter:** Gitignore was incorrectly placed causing strykeroutput to show in diff filter ([#1235](https://github.com/stryker-mutator/stryker-net/issues/1235)) ([fa88bda](https://github.com/stryker-mutator/stryker-net/commit/fa88bda5d30c0e844b67157c22688df0e4e86dca))
* **git diff filter:** Set mutants for unchanged files to Ignored if they have status NotRun ([#1243](https://github.com/stryker-mutator/stryker-net/issues/1243)) ([29219e4](https://github.com/stryker-mutator/stryker-net/commit/29219e4011c8df8681d16af64259793e6f9098b6))


### Features

* support .net5 and newer targetframeworks ([#1213](https://github.com/stryker-mutator/stryker-net/issues/1213)) ([92b3253](https://github.com/stryker-mutator/stryker-net/commit/92b32532ce502d7fd523bf9517ca09139e1dea0d))
* **clear text reporter:** Table report for clear text reporter ([#1242](https://github.com/stryker-mutator/stryker-net/issues/1242)) ([e19332d](https://github.com/stryker-mutator/stryker-net/commit/e19332d5822b2d51a6e959b442fe1e103a8e4de8))
* **cleartext reporter:** Improve directory structure of cleartext console report ([#1239](https://github.com/stryker-mutator/stryker-net/issues/1239)) ([5cb522b](https://github.com/stryker-mutator/stryker-net/commit/5cb522b7240a0052a6bd954937a8e576897b1863))
* **Dashboard Compare:** Add git diff file ignore ([#1206](https://github.com/stryker-mutator/stryker-net/issues/1206)) ([72d1473](https://github.com/stryker-mutator/stryker-net/commit/72d14731c8e2fb9147d665101b1ed75bebc03203))
* **Mutation levels:** Add mutation levels ([#987](https://github.com/stryker-mutator/stryker-net/issues/987)) ([2f0543e](https://github.com/stryker-mutator/stryker-net/commit/2f0543e9c86415c705eb89e90d2f9585f51cc03b))



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
