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

  Write-Info "Packing $ToolVersion to $PublishPath"
  dotnet pack $ToolProject "-c=Debug -p:PackageVersion=${ToolVersion}" --output $PublishPath
  if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed with exit code ${LASTEXITCODE}" }

  # Verify that the pack produced a package for the tool
  if (-not (Test-Path $PublishPath)) {
    throw "Publish path '$PublishPath' does not exist after packing"
  }

  # Ensure we always use the freshly packed tool version in the local tool-path.
  # `dotnet tool install` does not overwrite an existing local tool installation.
  dotnet tool uninstall dotnet-stryker --tool-path $ToolPath 2>$null
  if ($LASTEXITCODE -ne 0) { Write-Info "dotnet-stryker was not previously installed in '$ToolPath' (continuing)" }

  dotnet tool install dotnet-stryker --source $PublishPath --allow-downgrade --tool-path $ToolPath --version $ToolVersion
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
  Push-Location $WorkingDirectory
  try {
    if ($effectiveArguments.Count -gt 0) {
      & $stryker @effectiveArguments
    } else {
      & $stryker
    }
  } finally {
    Pop-Location
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
    [string]$Runtime,
    [string]$RepoRoot
  )

  # Log chosen category and any OS-specific behavior here
  Write-Info "Selected category: $Category"
  Write-Info "Selected runtime: $Runtime"

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
      if ($Runtime -eq 'netcore') {
        Run-Stryker -WorkingDirectory (Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore\NetCoreTestProject.XUnit')
      } elseif ($Runtime -eq 'netframework') {
        $netFrameworkTestProject = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetFramework\FullFrameworkApp.Test'
        Run-Stryker -WorkingDirectory $netFrameworkTestProject -Arguments @('--diag')
      } else {
        throw "Unknown runtime: $Runtime"
      }
      break
    }
    'MultipleTestProjects' {
      if ($Runtime -ne 'netcore') { throw "MultipleTestProjects only supports runtime 'netcore'." }
      $multiWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore\TargetProject'
      if (Test-Path $multiWd) { Run-Stryker -WorkingDirectory $multiWd } else { Write-Warn "Multi test project not found at $multiWd" }
      break
    }
    'MSTestMTP' {
      if ($Runtime -ne 'netcore') { throw "MSTestMTP only supports runtime 'netcore'." }
      $mtpWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\MicrosoftTestPlatform\UnitTests.MSTest'
      if (Test-Path $mtpWd) { Run-Stryker -WorkingDirectory $mtpWd } else { Write-Warn "MTP test project not found at $mtpWd" }
      break
    }
    'XUnitMTP' {
      if ($Runtime -ne 'netcore') { throw "XUnitMTP only supports runtime 'netcore'." }
      $xunitMtpWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\MicrosoftTestPlatform\UnitTests.XUnit'
      if (Test-Path $xunitMtpWd) { Run-Stryker -WorkingDirectory $xunitMtpWd } else { Write-Warn "XUnit MTP test project not found at $xunitMtpWd" }
      break
    }
    'NUnitMTP' {
      if ($Runtime -ne 'netcore') { throw "NUnitMTP only supports runtime 'netcore'." }
      $nunitMtpWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\MicrosoftTestPlatform\UnitTests.NUnit'
      if (Test-Path $nunitMtpWd) { Run-Stryker -WorkingDirectory $nunitMtpWd } else { Write-Warn "NUnit MTP test project not found at $nunitMtpWd" }
      break
    }
    'TUnit' {
      if ($Runtime -ne 'netcore') { throw "TUnit only supports runtime 'netcore'." }
      $tunitWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\MicrosoftTestPlatform\UnitTests.TUnit'
      if (Test-Path $tunitWd) { Run-Stryker -WorkingDirectory $tunitWd } else { Write-Warn "TUnit test project not found at $tunitWd" }
      break
    }
    'MTPSolution' {
      if ($Runtime -ne 'netcore') { throw "MTPSolution only supports runtime 'netcore'." }
      $mtpSolutionWd = Join-Path $RepoRoot 'integrationtest\TargetProjects'
      $mtpSolutionPath = Join-Path $mtpSolutionWd 'MicrosoftTestPlatform.slnx'
      if (Test-Path $mtpSolutionPath) { Run-Stryker -WorkingDirectory $mtpSolutionWd -Arguments @('--solution', $mtpSolutionPath, '--test-runner', 'mtp') } else { Write-Warn "MTP Solution not found at $mtpSolutionPath" }
      break
    }
    'Solution' {
      if ($Runtime -eq 'netcore') {
        $netcoreWd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetCore'
        $solutionPath = Join-Path $netcoreWd 'IntegrationTestApp.sln'
        if (Test-Path $solutionPath) { Run-Stryker -WorkingDirectory $netcoreWd -Arguments @('--solution', $solutionPath) } else { Write-Warn "Solution not found at $solutionPath" }
      } elseif ($Runtime -eq 'netframework') {
        $wd = Join-Path $RepoRoot 'integrationtest\TargetProjects\NetFramework\FullFrameworkApp.Test'
        Run-Stryker -WorkingDirectory $wd -Arguments @('--diag')
      } else {
        throw "Unknown runtime: $Runtime"
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
  $runtime = ${env:RUNTIME}
  if (-not $category) { throw "In GitHub Actions the CATEGORY environment variable must be set." }
  if (-not $runtime) { throw "In GitHub Actions the RUNTIME environment variable must be set." }
} else {
  Write-Info "Not running in GitHub Actions; defaulting to category 'Solution' and runtime 'netcore'"
  $category = 'Solution'
  $runtime = 'netcore'
}

$testFilter = "Category=$category&Runtime=$runtime"

Run-Category -Category $category -Runtime $runtime -RepoRoot $repoRoot
Run-ValidationTests -TestPath $testPath -Filter $testFilter
