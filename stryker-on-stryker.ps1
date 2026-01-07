#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param(
  [Parameter(Mandatory = $true)]
  [string]$WorkingDirectory,
  [switch]$UseLocalTool
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
    [string[]]$ToolParameters,
    [string]$StrykerPath
  )

  $effectiveToolParameters = @(
    $ToolParameters |
      Where-Object { $_ -is [string] -and $_.Trim().Length -gt 0 }
  )
  $effectiveToolParameters += '-V trace'

  Push-Location $RunDirectory
  Write-Info ("Current working directory: " + (Get-Location))
  try {
    $parametersForLog = Format-ToolParametersForLog -ToolParameters $effectiveToolParameters

    if (-not [string]::IsNullOrWhiteSpace($StrykerPath)) {
      Write-Info "Running local dotnet-stryker in '$RunDirectory'"
      Write-Info ("Run command: " + (($StrykerPath + ' ' + (($parametersForLog | ForEach-Object { $_ }) -join ' ')).Trim()))
      & $StrykerPath @effectiveToolParameters
    } else {
      $dotnetInvocation = @('tool', 'run', 'dotnet-stryker') + $effectiveToolParameters
      Write-Info "Running dotnet-stryker (from tool manifest) in '$RunDirectory'"
      Write-Info ("Run command: " + (((@('tool', 'run', 'dotnet-stryker') + $parametersForLog) | ForEach-Object { $_ }) -join ' '))
      & dotnet @dotnetInvocation
    }
  } finally {
    Pop-Location
  }

  if ($LASTEXITCODE -ne 0) { throw "dotnet-stryker failed with exit code ${LASTEXITCODE}" }
}

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

$repoRoot = if (-not [string]::IsNullOrWhiteSpace(${env:GITHUB_WORKSPACE})) {
  ${env:GITHUB_WORKSPACE}
} else {
  (Resolve-Path (Join-Path $PSScriptRoot '.')).Path
}
$absoluteWorkingDirectory = (Resolve-Path (Join-Path $repoRoot $WorkingDirectory)).Path

# Detect tool manifest in standard locations
$toolManifestCandidates = @(
  (Join-Path $repoRoot '.config' 'dotnet-tools.json'),
  (Join-Path $repoRoot 'dotnet-tools.json')
)
if (-not ($toolManifestCandidates | Where-Object { Test-Path $_ })) {
  throw "Expected dotnet tool manifest not found in '.config/dotnet-tools.json' or 'dotnet-tools.json' at repo root."
}

# Decide whether to use local tool install mode
$useLocalTool = $UseLocalTool.IsPresent -or (${env:USE_LOCAL_TOOL} -eq 'true')
if (-not $useLocalTool) {
  Restore-DotNetTools -RepoRoot $repoRoot
}

# If using local tool, pack and install just like the integration tests
$strykerPath = $null
if ($useLocalTool) {
  $publishPath = Join-Path $repoRoot 'publish'
  $toolPath = Join-Path $repoRoot '.nuget' 'tools'
  $toolProject = Join-Path $repoRoot 'src' 'Stryker.CLI' 'Stryker.CLI' 'Stryker.CLI.csproj'

  $toolVersion = '0.0.0-'
  if (${env:GITHUB_ACTIONS} -eq 'true') { $toolVersion += "github-${env:GITHUB_RUN_NUMBER}" } else { $toolVersion += 'localdev' }

  Pack-And-Install-Tool -ToolProject $toolProject -ToolVersion $toolVersion -PublishPath $publishPath -ToolPath $toolPath
  $strykerPath = Join-Path $toolPath 'dotnet-stryker'
}

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
  ) -StrykerPath $strykerPath
} else {
  Invoke-Stryker -RunDirectory $absoluteWorkingDirectory -ToolParameters @(
    '--reporter',
    'html'
  ) -StrykerPath $strykerPath
}
