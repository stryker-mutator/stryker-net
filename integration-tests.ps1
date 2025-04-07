#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param()

$ErrorActionPreference = "Stop"

${env:RestoreLockedMode} = "true"

$repoRoot = ${env:GITHUB_WORKSPACE} ?? $PSScriptRoot

$publishPath = Join-Path $repoRoot "publish"

$netCoreTargetPath = Join-Path $repoRoot "integrationtest" "TargetProjects" "NetCore" "NetCoreTestProject.XUnit"
$netfxTargetPath = Join-Path $repoRoot "integrationtest" "TargetProjects" "NetFramework" "FullFrameworkApp.Test"

$testPath = Join-Path $repoRoot "integrationtest" "Validation" "ValidationProject"
$testFilter = "Category=SingleTestProject"

$toolPath = Join-Path $repoRoot ".nuget" "tools"
$toolProject = Join-Path $repoRoot "src" "Stryker.CLI" "Stryker.CLI" "Stryker.CLI.csproj"

$toolVersion = "0.0.0-"

if (${env:GITHUB_ACTIONS} -eq "true") {
  $toolVersion += "github-${env:GITHUB_RUN_NUMBER}"
} else {
  $toolVersion += "localdev"
}

$stryker = Join-Path $toolPath "dotnet-stryker"

dotnet pack $toolProject "-p:PackageVersion=${toolVersion}" --output $publishPath

if ($LASTEXITCODE -ne 0) {
  throw "dotnet pack failed with exit code ${LASTEXITCODE}"
}

dotnet tool install dotnet-stryker --add-source $publishPath --allow-downgrade --tool-path $toolPath --version $toolVersion

if ($LASTEXITCODE -ne 0) {
  throw "dotnet tool install failed with exit code ${LASTEXITCODE}"
}

pushd $netCoreTargetPath

try {
  & $stryker --dev-mode
} finally {
  popd
}

if ($LASTEXITCODE -ne 0) {
  throw "dotnet-stryker failed for .NET with exit code ${LASTEXITCODE}"
}

if ($IsWindows) {
  pushd $netfxTargetPath

  try {
    dotnet msbuild -t:restore

    if ($LASTEXITCODE -ne 0) {
      throw "dotnet msbuild failed to restore NuGet packages with exit code ${LASTEXITCODE}"
    }

    & $stryker --dev-mode

    if ($LASTEXITCODE -ne 0) {
      throw "dotnet-stryker failed for .NET Framework with exit code ${LASTEXITCODE}"
    }
  } finally {
    popd
  }
}

dotnet test $testPath --filter $testFilter

if ($LASTEXITCODE -ne 0) {
  throw "dotnet test failed with exit code ${LASTEXITCODE}"
}
