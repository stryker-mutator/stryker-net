---
title: Updating
custom_edit_url: https://github.com/stryker-mutator/stryker-net/edit/master/docs/updating.md
---
# Updating stryker
Dotnet tools do not auto update so you are responsible for making sure you're up-to-date. To help with this stryker will notify you when a new version is available

To update stryker as a global tool run `dotnet tool update --global dotnet-stryker`

To update stryker as a project tool run `dotnet tool update --local dotnet-stryker` or change the version in the `dotnet-tools.json` file. Then check in the updated `dotnet-tools.json` file to version control.