# Release procedure
Releasing a new version of the Stryker.NET packages can be done by following these steps:
1. Clone the repo or checkout (and pull) the master branch.
2. Look at the commits since the last release and determine the next version number. If we are on 0.11.0 and there are only bugfixes and refactorings: we go to 0.11.1. If there are also new features or breaking changes, we go to 0.12.0.
3. Run `npm run prepare-release` from the root of the repo and enter the new version number.
4. Verify that the commit is on GitHub and the releases in GitHub have been made.
5. Wait for build on master to complete on Azure Pipelines and then start the Production environment on Azure Pipelines.
6. Approve the Production environment on Azure Pipelines (this approval prevents accidental releases).

It may take about 15 minutes for the new packages to show up on NuGet.org and in clients. New packages first have to be verified and then indexed before they can be used.
