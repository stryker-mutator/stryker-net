# Contributing to Stryker.NET
This is the contribution guide for Stryker.NET. Great to have you here! Here are a few ways you can help make this project better.

## Creating issues
Do you have an idea for a feature or have you found a bug? Please create an issue so we can talk about it!
If you found a bug, please run ```dotnet stryker --logFile``` and add the content of the log file to the issue.

## Running Stryker.NET locally
While developing on Stryker.NET we advise to work in [Visual Studio 2017](https://www.visualstudio.com/downloads/)

### Steps to run Stryker on Stryker:
*	Clone the repository `https://github.com/stryker-mutator/stryker-net.git`
*	Open `Stryker.CLI.sln`
*	On `Stryker.CLI` open `properties > Debug`
*	Create a new Debug profile
*	Set `Launch` as `Project` 
*	Set `WorkingDirectory` as your local installation dir, pointing to a UnitTest project `example: (C:\Repos\stryker-net\src\Stryker.Core\Stryker.Core.UnitTest)`
*	Run the program with the newly created Debug profile

## Adding new features
New features are welcome! Either as requests or proposals.

1.	Please create an issue first, so we know what to expect from you.
2.	Create a fork on your github account.
3.	When writing your code, please conform to the [Microsoft coding guidelines](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
4.	Please create or edit unit tests or integration tests.
5.	Run the tests and verify they pass
6. 	When creating commits, please conform the following template `<type>: <subject>\n\n[body]`
	* Type: feat, fix, docs, style, refactor, test, chore.
	* Subject and body: present tense (~changed~*change*, ~added~*add*) and include motivation and contrasts with previous behavior

## Community
Do you want to help? Great! These are a few things you can do:

* Evangelize mutation testing. Mutation testing is still relatively new, especially in .NET Core. Please help us get the word out there!
* Share your stories in blog posts an on social media. Please inform us about it! Did you use Stryker? Your feedback is very valuable to us. Good and bad! Please contact us and let us know what you think