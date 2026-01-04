#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param()

$ErrorActionPreference = "Stop"
${env:RestoreLockedMode} = "true"

# -------------------------
# Helpers
# -------------------------
function Write-Info([string]$msg) { Write-Host "[INFO] $msg" }
function Write-Warn([string]$msg) { Write-Warning "[WARN] $msg" }

function Pack-And-Install-Tool {
  param(
    [string]$ToolProject,
    [string]$ToolVersion,
    [string]$PublishPath,
    [string]$ToolPath
  )

  dotnet pack $ToolProject "-p:PackageVersion=${ToolVersion}" --output $PublishPath
  if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed with exit code ${LASTEXITCODE}" }

  # Ensure we always use the freshly packed tool version in the local tool-path.
  # `dotnet tool install` does not overwrite an existing local tool installation.
  dotnet tool uninstall dotnet-stryker --tool-path $ToolPath 2>$null
  if ($LASTEXITCODE -ne 0) { Write-Info "dotnet-stryker was not previously installed in '$ToolPath' (continuing)" }

  dotnet tool install dotnet-stryker --add-source $PublishPath --allow-downgrade --tool-path $ToolPath --version $ToolVersion
  if ($LASTEXITCODE -ne 0) { throw "dotnet tool install failed with exit code ${LASTEXITCODE}" }
}

function Run-Stryker {
  param(
    [string]$WorkingDirectory,
    [string[]]$Arguments = @(),
    [bool]$ContinueOnError = $false
  )

  $effectiveArguments = @(
    $Arguments |
      Where-Object { $_ -is [string] -and $_.Trim().Length -gt 0 }
  )

  $argumentText = ($effectiveArguments | ForEach-Object { $_ }) -join ' '
  Write-Info "Running dotnet-stryker in '$WorkingDirectory' with args: $argumentText"
  pushd $WorkingDirectory
  try {
    if ($effectiveArguments.Count -gt 0) {
      & $stryker @effectiveArguments
    } else {
      & $stryker
    }
  } finally {
    popd
  }

  if ($LASTEXITCODE -ne 0) {
    if ($ContinueOnError) {
      Write-Warn "dotnet-stryker failed in $WorkingDirectory with exit code $LASTEXITCODE (continuing)"
    } else {
      throw "dotnet-stryker failed in $WorkingDirectory with exit code $LASTEXITCODE"
    }
  }
}

function Run-ValidationTests {
  param(
    [string]$TestPath,
    [string]$Filter
  )

  dotnet test $TestPath --filter $Filter
  if ($LASTEXITCODE -ne 0) { throw "dotnet test failed with exit code ${LASTEXITCODE}" }
}



function Run-Category {
  param(
    [string]$Category,
    [string]$RepoRoot
  )

  # Log chosen category and any OS-specific behavior here
  Write-Info "Selected category: $Category"
  if ($Category -eq 'Solution') {
    if ($IsWindows) {
      Write-Info "Solution category will run the .NET Framework test suite on Windows"
    } else {
      Write-Info "Solution category will run the .NET Core solution on non-Windows"
    }
  }

  switch ($Category) {
    'InitCommand' {
      $wd = Join-Path $RepoRoot 'integrationtest\TargetProjects'
      Run-Stryker -WorkingDirectory $wd -Arguments @(
        'init'
        '--config-file'
        'InitCommand/test-config.json'
        '-p'
        'TestProject.csproj'
      )
      break
    }
    'SingleTestProject' {
      Run-Stryker -WorkingDirectory (Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore\NetCoreTestProject.XUnit')
      break
    }
    'MultipleTestProjects' {
      $multiWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore\TargetProject'
      if (Test-Path $multiWd) { Run-Stryker -WorkingDirectory $multiWd } else { Write-Warn "Multi test project not found at $multiWd" }
      break
    }
    'Solution' {
      if ($IsWindows) {
        $wd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetFramework\FullFrameworkApp.Test'
        pushd $wd
        try {
          dotnet msbuild -t:restore
          if ($LASTEXITCODE -ne 0) { throw "dotnet msbuild restore failed with exit code $LASTEXITCODE" }

          & $stryker --dev-mode
          if ($LASTEXITCODE -ne 0) { throw "dotnet-stryker failed for .NET Framework with exit code ${LASTEXITCODE}" }
        } finally { popd }
      } else {
        $netcoreWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore'
        $solutionPath = Join-Path $netcoreWd 'IntegrationTestApp.sln'
        if (Test-Path $solutionPath) { Run-Stryker -WorkingDirectory $netcoreWd -Arguments @('--solution', $solutionPath) } else { Write-Warn "Solution not found at $solutionPath" }
      }
      break
    }
    Default { throw "Unknown category: $Category" }
  }
}

# -------------------------
# Main
# -------------------------
$repoRoot = ${env:GITHUB_WORKSPACE} ?? $PSScriptRoot
$publishPath = Join-Path $repoRoot "publish"

$netCoreTargetPath = Join-Path $repoRoot "integrationtest" "TargetProjects" "NetCore" "NetCoreTestProject.XUnit"
$netfxTargetPath = Join-Path $repoRoot "integrationtest" "TargetProjects" "NetFramework" "FullFrameworkApp.Test"

$testPath = Join-Path $repoRoot "integrationtest" "Validation" "ValidationProject"

$toolPath = Join-Path $repoRoot ".nuget" "tools"
$toolProject = Join-Path $repoRoot "src" "Stryker.CLI" "Stryker.CLI" "Stryker.CLI.csproj"

$toolVersion = "0.0.0-"
if (${env:GITHUB_ACTIONS} -eq "true") { $toolVersion += "github-${env:GITHUB_RUN_NUMBER}" } else { $toolVersion += "localdev" }

$stryker = Join-Path $toolPath "dotnet-stryker"

# Install tool
Pack-And-Install-Tool -ToolProject $toolProject -ToolVersion $toolVersion -PublishPath $publishPath -ToolPath $toolPath

if (${env:GITHUB_ACTIONS} -eq "true") {
  $category = ${env:CATEGORY}
  if (-not $category) {
    throw "In GitHub Actions the CATEGORY environment variable must be set."
  }
} else {
  Write-Info "Not running in GitHub Actions; defaulting category to 'Solution'"
  $category = 'Solution'
}

$testFilter = "Category=$category"

Run-Category -Category $category -RepoRoot $repoRoot
Run-ValidationTests -TestPath $testPath -Filter $testFilter
