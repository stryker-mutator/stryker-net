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
function Write-Warn([string]$Message) { Write-Host "[WARN] $Message" }

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

function Get-DashboardVersion {
  param(
    [string]$RepoRoot
  )

  $isGh = ${env:GITHUB_ACTIONS} -eq 'true'
  if ($isGh) {
    $event = ${env:GITHUB_EVENT_NAME}
    $refName = ${env:GITHUB_REF_NAME}
    $ref = ${env:GITHUB_REF}
    $headRef = ${env:GITHUB_HEAD_REF}

    if ($event -eq 'pull_request' -and -not [string]::IsNullOrWhiteSpace($ref)) {
      return $ref
    }
    if (-not [string]::IsNullOrWhiteSpace($refName)) { return $refName }
    if (-not [string]::IsNullOrWhiteSpace($ref)) { return $ref }
    return 'unknown'
  }

  # Local fallback: try to read current git branch
  Push-Location $RepoRoot
  try {
    $branch = (git rev-parse --abbrev-ref HEAD 2>$null).Trim()
    if (-not [string]::IsNullOrWhiteSpace($branch)) { return $branch }
  } catch { }
  finally { Pop-Location }
  return 'local'
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
    if (-not [string]::IsNullOrWhiteSpace($StrykerPath)) {
      Write-Info "Running local dotnet-stryker in '$RunDirectory'"
      Write-Info ("Run command: " + (($StrykerPath + ' ' + (($effectiveToolParameters | ForEach-Object { $_ }) -join ' ')).Trim()))
      & $StrykerPath @effectiveToolParameters
    } else {
      $dotnetInvocation = @('tool', 'run', 'dotnet-stryker') + $effectiveToolParameters
      Write-Info "Running dotnet-stryker (from tool manifest) in '$RunDirectory'"
      Write-Info ("Run command: " + (((@('tool', 'run', 'dotnet-stryker') + $effectiveToolParameters) | ForEach-Object { $_ }) -join ' '))
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
$dashboardApiKey = ${env:STRYKER_DASHBOARD_API_KEY}

$publishToDashboard = $isGitHubActions -and -not [string]::IsNullOrWhiteSpace($dashboardApiKey)
if (-not $publishToDashboard -and -not [string]::IsNullOrWhiteSpace($dashboardApiKey)) {
  Write-Warn 'STRYKER_DASHBOARD_API_KEY is set but dashboard publishing is disabled for this run.'
}

if ($publishToDashboard) {
  $dashboardVersion = Get-DashboardVersion -RepoRoot $repoRoot
  Invoke-Stryker -RunDirectory $absoluteWorkingDirectory -ToolParameters @(
    '--reporter',
    'dots',
    '--reporter',
    'dashboard',
    '--version',
    $dashboardVersion
  ) -StrykerPath $strykerPath
} else {
  Invoke-Stryker -RunDirectory $absoluteWorkingDirectory -ToolParameters @(
    '--reporter',
    'html'
  ) -StrykerPath $strykerPath
}
