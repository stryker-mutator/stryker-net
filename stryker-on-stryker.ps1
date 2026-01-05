#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param(
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory
)

$ErrorActionPreference = 'Stop'

function Write-Info([string]$Message) { Write-Host "[INFO] $Message" }
function Write-Warn([string]$Message) { Write-Warning "[WARN] $Message" }

function Protect-SecretInGitHubActionsLog {
  param(
    [string]$Secret
  )

  if ([string]::IsNullOrWhiteSpace($Secret)) {
    return
  }

  if (${env:GITHUB_ACTIONS} -eq 'true') {
    Write-Host "::add-mask::$Secret"
  }
}

function Format-ToolParametersForLog {
  param(
    [string[]]$ToolParameters
  )

  $redactedParameters = New-Object System.Collections.Generic.List[string]
  for ($i = 0; $i -lt $ToolParameters.Count; $i++) {
    $parameter = $ToolParameters[$i]

    if ($parameter -eq '--dashboard-api-key') {
      $redactedParameters.Add($parameter)

      if ($i + 1 -lt $ToolParameters.Count) {
        $redactedParameters.Add('<redacted>')
        $i++
      }

      continue
    }

    if ($parameter -like '--dashboard-api-key=*') {
      $redactedParameters.Add('--dashboard-api-key=<redacted>')
      continue
    }

    $redactedParameters.Add($parameter)
  }

  return $redactedParameters.ToArray()
}

function Restore-DotNetTools {
  param(
    [string]$RepoRoot
  )

  Push-Location $RepoRoot
  try {
    Write-Info "Restoring local dotnet tools in '$RepoRoot'"
    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet tool restore failed with exit code ${LASTEXITCODE}" }
  } finally {
    Pop-Location
  }
}

function Invoke-Stryker {
  param(
    [string]$RunDirectory,
    [string[]]$ToolParameters
  )

  $effectiveToolParameters = @(
    $ToolParameters |
      Where-Object { $_ -is [string] -and $_.Trim().Length -gt 0 }
  )

  Push-Location $RunDirectory
  try {
    $dotnetInvocation = @('tool', 'run', 'dotnet-stryker', '--') + $effectiveToolParameters

    Write-Info "Running dotnet-stryker in '$RunDirectory'"
    $parametersForLog = Format-ToolParametersForLog -ToolParameters $effectiveToolParameters
    Write-Info ("Parameters: " + (($parametersForLog | ForEach-Object { $_ }) -join ' '))

    & dotnet @dotnetInvocation
  } finally {
    Pop-Location
  }

  if ($LASTEXITCODE -ne 0) { throw "dotnet-stryker failed with exit code ${LASTEXITCODE}" }
}

$repoRoot = if (-not [string]::IsNullOrWhiteSpace(${env:GITHUB_WORKSPACE})) {
  ${env:GITHUB_WORKSPACE}
} else {
  (Resolve-Path (Join-Path $PSScriptRoot '.')).Path
}
$absoluteWorkingDirectory = (Resolve-Path (Join-Path $repoRoot $WorkingDirectory)).Path
$toolManifestPath = Join-Path $repoRoot 'dotnet-tools.json'
if (-not (Test-Path $toolManifestPath)) { throw "Expected repo root dotnet tool manifest not found: $toolManifestPath" }

Restore-DotNetTools -RepoRoot $repoRoot

$isGitHubActions = ${env:GITHUB_ACTIONS} -eq 'true'
$isScheduledRun = ${env:GITHUB_EVENT_NAME} -eq 'schedule'
$dashboardApiKey = ${env:STRYKER_DASHBOARD_API_KEY}
Protect-SecretInGitHubActionsLog -Secret $dashboardApiKey

$publishToDashboard = $isGitHubActions -and $isScheduledRun -and -not [string]::IsNullOrWhiteSpace($dashboardApiKey)
if (-not $publishToDashboard -and -not [string]::IsNullOrWhiteSpace($dashboardApiKey)) {
  Write-Warn 'STRYKER_DASHBOARD_API_KEY is set but dashboard publishing is disabled for this run.'
}

if ($publishToDashboard) {
  Invoke-Stryker -RunDirectory $absoluteWorkingDirectory -ToolParameters @(
    '--reporter',
    'dots',
    '--reporter',
    'dashboard',
    '--version',
    'master',
    '--dashboard-api-key',
    $dashboardApiKey
  )
} else {
  Invoke-Stryker -RunDirectory $absoluteWorkingDirectory -ToolParameters @(
    '--reporter',
    'html'
  )
}
