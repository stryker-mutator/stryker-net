# Contributing to Stryker.NET
This is the contribution guide for Stryker.NET. Great to have you here! Here are a few ways you can help make this project better.

# Creating issues
Do you have an idea for a feature or have you found a bug? Please create an issue so we can talk about it!
If you found a bug, please run ```dotnet stryker --logFile``` and add the content of the log file to the issue.

# Running Stryker.NET locally
While developing on Stryker.NET we advise to work in [Visual Studio 2017](https://www.visualstudio.com/downloads/)

### Steps to run Stryker on Stryker:
- Clone the repository `https://github.com/stryker-mutator/stryker-net.git`
- Open `Stryker.CLI.sln`
- On `Stryker.CLI` open `properties > Debug`
- Create a new Debug profile
- Set `Launch` as `Project` 
- Set `WorkingDirectory` as your local installation dir, pointing to a UnitTest project `example: (C:\Repos\stryker-net\src\Stryker.Core\Stryker.Core.UnitTest)`
- Run using F5 or Ctrl+F5

# Community
Do you want to help? Great! These are a few things you can do:

* Evangelize mutation testing. Mutation testing is still relatively new, especially in JavaScript. Please help us get the word out there!
* Share your stories in blog posts an on social media. Please inform us about it! Did you use Stryker? Your feedback is very valuable to us. Good and bad! Please contact us and let us know what you think