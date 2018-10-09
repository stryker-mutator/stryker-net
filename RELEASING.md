# Release procedure
Releasing a new version of the Stryker.NET packages can be done by following these steps:
1. Clone the repo or checkout (and pull) the master branch.
2. Look at the commits since the last release and determine the next version number. If we are on 0.11.0 and there are only bugfixes and refactorings: we go to 0.11.1. If there are also new features or breaking changes, we go to 0.12.0.
3. Set the version number in the `PackageVersion` variable in the azure-pipelines.yml file and in csproj files of the projects that have a `<Version>` property.
4. Edit the changelog for each package by mentioning every new feature, bugfix, refactoring and breaking change. Please look at existing changelog entries to figure out the correct format.
5. Commit your changes using the format below. (Replace package names and version names with whatever you're publishing). Make sure to have two empty lines between the `Publish` word and the changed package versions.
```
Publish


- StrykerMutator.Core@0.12.0
- StrykerMutator.DotNetCoreCli@0.12.0
```
6. Tag the last commit you made using the command: 
```
git tag -a StrykerMutator.Core@0.12.0 -m "StrykerMutator.Core@0.12.0"
``` 
(Replace package names and version names with whatever you're publishing) You can put multiple tags on a single commit. 

7. Push your commit and tags using `git push --follow-tags`.
8. Verify that the commit is on GitHub and the releases in GitHub have been made.
9. Wait for build on master to complete on Azure Pipelines and then start the Production environment on Azure Pipelines.
10. Approve the Production environment on Azure Pipelines (this approval prevents accidental releases).

It may take about 15 minutes for the new packages to show up on NuGet.org and in clients. New packages first have to be verified and then indexed before they can be used.
