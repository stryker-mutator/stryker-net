---
title: Installing Stryker in pipelines
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/Stryker-in-pipeline.md
---

When running stryker in your pipeline there are some things to take into consideration

Due to the way dotnet core global tools are installed on the system a regular `dotnet tool install -g` is often not effective in pipelines.

Instead use the `--tool-path` to install stryker in a local folder or use the project level install of dotnet core 3.0+

Example for installing in azure devops:

```yaml
- task: DotNetCoreCLI@2
    displayName: 'Install dotnet-stryker'
    inputs:
      command: custom
      custom: tool
      arguments: install dotnet-stryker --tool-path $(Agent.BuildDirectory)/tools
```

And then running this locally installed tool:

```yaml
- task: Powershell@2
    displayName: 'Run dotnet-stryker'
    inputs:
      workingDirectory: <test-project-folder-here>
      targetType: 'inline'
      pwsh: true
      script: $(Agent.BuildDirectory)/tools/dotnet-stryker
```

## Configuring dashboard compare in pull requests
Dashboard compare is very useful when running stryker in pipelines because it cuts down on the total runtime by only testing mutations that have changed compared to for example master
The following minimal steps are needed to use dashboard compare

1. Enable --dashboard-compare 
1. Choose a storage provider (Dashboard for public projects or Azure File Share for private projects)
1. Set up authentication for the chosen storage provider 
1. Set --dashboard-version to the name of the source branch (usually current branch)
1. Set --git-source to the name of the target branch (usually master/main or development)
1. Set any other options needed for your chosen storage provider (see: [Configuration](./Configuration.md))

Example for azure devops with dashboard storage provider:
```
dotnet stryker --dashboard-compare --baseline-storage-location Dashboard --dashboard-api-key $(Stryker.Dashboard.Api.Key)--dashboard-version $(System.PullRequest.SourceBranch) --git-source $(System.PullRequest.TargetBranch)
dotnet stryker -compare -bsl Dashboard -dk $(Stryker.Dashboard.Api.Key)-version $(System.PullRequest.SourceBranch) -source $(System.PullRequest.TargetBranch)
```
