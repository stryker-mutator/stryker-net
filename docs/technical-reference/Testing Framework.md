# Testing Frameworks undocumented
This document captures the discoveries made about the various testing frameworks while working on Stryker. 
This can help understanding the design and logic of the mutation test classes.
This is a mandatory reading for anyone trying to understand and/or fix bugs related to coverage capture, analysis and testing phase.

To be specific, this document will mostly (and implicitly) refer to their VsTest adapters.

## Generalities
The [VsTest protocol](https://github.com/Microsoft/vstest-docs/blob/master/RFCs/0006-DataCollection-Protocol.md) is not
rich enough to cover every test framework feature. Therefore, each framework makes their own decisions on how to handle
some of their specificities. That is the reason why we can observe different behaviour and surprising results.

This affects:
- how tests are discovered
- how tests are described/identified
- how tests are filtered
- how tests are executed
- how test results are reported

## Identification
By default, VsTest generates unique identifiers stored as `Guid`. I do not know if test runners are able to provide
a specific implementation but in practice, this identifier is derived from the(test's) display's name hash code.

## xUnit

### Theories
xUnit's theory is a pattern where some test is executed multiple times with varying input data (like NUnit's TestCase). 
These test mesthods accept one or more parameters and bear one of this attributes: `InlineData`, `MemberData` or `ClassData`.
There are differences
between compile time theories (data is fixed at build time) and run time theories (data is discovered at run time).

#### Static theories
Static discoveries are seen and processed as different test cases.

#### Run time theories
Run time theories are discovered as one test, disregarding the number of underlying data set.
There is one test result per dataset, all associated with the same test.

Here is a summarized timeline of tests execution:
```
   xUnit runner calls data theory's data source to fetch each test case value(s)
TestCase start event
   xUnit run test with first set of data
   xUnit reports test results
TestCase end event
   xUnit run test with second set of data
   xUnit reports test reult
   xUnit run test with third set of data
   ...
   xUnit reports last test result
TestCase start event
   ...
```
The difficulty here is that a lot happens between `testcase end` and `testcase start` events. 
At coverage capture it is a problem, because there is a risk of spilling coverage information to the next tests: Stryker
can only capture coverage information on `testcase end` events, and only in association with the current running test.
So, every mutants covered between `testcase end` and `testcase start` will be associated with the **next** test.

And during execution phase, it is impossible to predict when the test will really be over, so it is diffult to
establish is the test was succesful.  

Also, if a mutation ends up changing a test case name - typically by changing the result of `ToTring()`, it will change the
test identifier so this testcase will only run when running **all tests** and can no longer be executed in isolation, as
Stryker can't anticipate the test name.

### NUnit

### TestCases
NUnit's TestCases is a pattern where some test is executed multiple times with varying input data (like xUnit's TestCase).
These test methods accept one or more parameters and bear one of these attributes (on top of the `[Test]` attribute):
 `TestCase` or `TestCaseSource`.

There are some differences
between `TestCase` (data is fixed at build time) and `TestCaseSource` (data is discovered at run time).

### TestCase
Basically, each `TestCase` is considered to be a specific test, and NUnit generates a name for each of those using the
`ToString()` method for each parameter (e.g: `Test(1,2)` for parameter 1 and 2).
Then each test is run separately and a test result is provided for each test.
**Caveat**: if the `ToString()` method is not overriden, multiple test cases will have the same name 
(eg: `Test(MyClass`). Then, there will be no way to distinguish between tests as well as properly associate test
results.

### TestCaseSource
Each test case is reported during the discovery phase, and NUnit generates a name for each of those using the
`ToString()` method for each parameter (e.g: `Test(1,2)` for parameter 1 and 2).
Then each test is run separately and a test result is provided for each test.
**Caveat**: if the `ToString()` method is not overriden, multiple test cases will have the same name 
(eg: `Test(MyClass`). Then, there will be no way to distinguish between tests as well as properly associate test
results.
When this happens, these appear as a single test resulting in multiple outcomes in Visual Studio.

Here is a summarized timeline of tests execution:
```
   NUnit runner calls the TestCaseSource and gets every test case.
TestCase start event
   NUnit run test with first set of data
   NUnit reports test reult
TestCase end event
TestCase start event
   NUnit run test with second set of data
   NUnit reports test reult
TestCase end event
   ...
   NUnit reports lasy test reult
TestCase end event
TestCase start event
   ...
```

The problems for Stryker are:
1. If test cases have the same name, we cannot distinguish between test cases, so it must be handled as a single test. This *single test* reports multiple test cases so Stryker needs to wait either for one failure or for each testcase
to succeed to establish the correct status. The good news is that discovery should provide the number of results to wait for.
2. Every mutation encountered by the `TestCaseSource` method will be reported as covered by the first test case,
whereas it is likely these mutations would need to be adequately associated with their realtive test case.
3. TestCaseSource is called **before** Testcase start event which means Stryker may set the active mutation too late. 
The only way to handle this is to force any concerned mutants to run in dedicated test sessions.

