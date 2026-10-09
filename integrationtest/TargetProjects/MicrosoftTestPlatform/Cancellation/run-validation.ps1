#Requires -Version 7
param(
  [Parameter(Mandatory)]
  [string]$EvidencePath
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..'))
$evidence = [IO.Path]::GetFullPath($EvidencePath)
[IO.Directory]::CreateDirectory($evidence) | Out-Null
$testDirectory = Join-Path $PSScriptRoot 'Tests'
$cliProject = Join-Path $repoRoot 'src\Stryker.CLI\Stryker.CLI\Stryker.CLI.csproj'
$unitProject = Join-Path $repoRoot 'src\Stryker.TestRunner.MicrosoftTestPlatform.UnitTest\Stryker.TestRunner.MicrosoftTestPlatform.UnitTest.csproj'
$cli = Join-Path $repoRoot 'src\Stryker.CLI\Stryker.CLI\bin\Release\Stryker.CLI.dll'
$unitHost = Join-Path $repoRoot 'src\Stryker.TestRunner.MicrosoftTestPlatform.UnitTest\bin\Release\Stryker.TestRunner.MicrosoftTestPlatform.UnitTest.dll'
$fixture = Join-Path $testDirectory 'bin\Release\net10.0\Tests.dll'

dotnet build $cliProject -c Release --no-restore --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'Release CLI build failed. Restore its locked dependencies first.' }
dotnet build $unitProject -c Release --no-restore --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'MTP unit host build failed. Restore its locked dependencies first.' }
$backup = Join-Path $testDirectory 'bin\Release\net10.0\Subject.dll.stryker-unchanged'
if (Test-Path -LiteralPath $backup) { Remove-Item -LiteralPath $backup }
dotnet build (Join-Path $testDirectory 'Tests.csproj') -c Release --no-incremental --nologo --verbosity quiet
if ($LASTEXITCODE -ne 0) { throw 'Cancellation fixture build failed.' }

$previousScenario = $env:STRYKER_CANCELLATION_SCENARIO
$previousEvidence = $env:STRYKER_CANCELLATION_EVIDENCE
$previousFixture = $env:STRYKER_MTP_CANCELLATION_FIXTURE
$summaries = [Collections.Generic.List[object]]::new()
try {
  $env:STRYKER_MTP_CANCELLATION_FIXTURE = $fixture
  $env:STRYKER_CANCELLATION_SCENARIO = 'cooperative'
  $env:STRYKER_CANCELLATION_EVIDENCE = Join-Path $evidence 'acceptance-events'
  dotnet $unitHost --filter 'TestCategory=MtpCancellationAcceptance' --report-trx --results-directory (Join-Path $evidence 'acceptance')
  if ($LASTEXITCODE -ne 0) { throw 'Real-host cancellation acceptance tests failed.' }
  $trxFile = Get-ChildItem (Join-Path $evidence 'acceptance') -Filter '*.trx' | Sort-Object LastWriteTime -Descending | Select-Object -First 1
  [xml]$trx = Get-Content $trxFile.FullName -Raw
  $counters = $trx.TestRun.ResultSummary.Counters
  if ([int]$counters.total -ne 7 -or [int]$counters.passed -ne 7) {
    throw 'Expected seven executed, passing real-host acceptance tests with no skips.'
  }

  foreach ($scenario in @('cooperative', 'disable-bail', 'noncooperative', 'timeout', 'crash', 'legacy', 'baseline')) {
    $env:STRYKER_CANCELLATION_SCENARIO = if ($scenario -eq 'disable-bail') { 'cooperative' } else { $scenario }
    $env:STRYKER_CANCELLATION_EVIDENCE = Join-Path $evidence "$scenario-events"
    $output = Join-Path $evidence "$scenario-report"
    $log = Join-Path $evidence "$scenario.log"
    $arguments = @($cli, '--skip-version-check', '--verbosity', 'debug', '--log-to-file', '--output', $output)
    if ($scenario -eq 'disable-bail') { $arguments += '--disable-bail' }
    Push-Location $testDirectory
    try {
      $watch = [Diagnostics.Stopwatch]::StartNew()
      dotnet @arguments *> $log
      $exitCode = $LASTEXITCODE
      $watch.Stop()
    } finally {
      Pop-Location
    }
    if ($exitCode -ne 0) { throw "$scenario exited with $exitCode. See $log" }
    $report = Get-Content (Join-Path $output 'reports\mutation-report.json') -Raw | ConvertFrom-Json
    $mutants = @($report.files.PSObject.Properties.Value.mutants | Sort-Object { [int]$_.id })
    if ($mutants.Count -ne 3) { throw "$scenario generated $($mutants.Count) mutants, expected exactly three." }
    $expectedFirst = switch ($scenario) {
      'timeout' { 'Timeout' }
      'crash' { 'RuntimeError' }
      default { 'Killed' }
    }
    $expected = @($expectedFirst, 'Survived', 'Survived')
    for ($index = 0; $index -lt 3; $index++) {
      if ($mutants[$index].status -ne $expected[$index]) {
        throw "$scenario mutant $index was $($mutants[$index].status), expected $($expected[$index])."
      }
    }
    $logText = Get-Content $log -Raw
    if ($logText.Contains('not fully tested') -or $logText.Contains('failed to test 1 mutant')) {
      throw "$scenario left a mutant incompletely assessed."
    }
    $durations = @([regex]::Matches($logText, 'MTP assembly run completed in ([\d.]+) ms;') |
      ForEach-Object { [double]::Parse($_.Groups[1].Value, [Globalization.CultureInfo]::InvariantCulture) })
    $summaries.Add([pscustomobject]@{
      Scenario = $scenario
      ExitCode = $exitCode
      Statuses = @($mutants.status)
      WallMilliseconds = $watch.Elapsed.TotalMilliseconds
      FirstMutantMilliseconds = if ($durations.Count -eq 4) { $durations[1] } else { $null }
      Report = Join-Path $output 'reports\mutation-report.json'
      Log = $log
    })
  }
  $withBail = ($summaries | Where-Object Scenario -eq 'cooperative').FirstMutantMilliseconds
  $withoutBail = ($summaries | Where-Object Scenario -eq 'disable-bail').FirstMutantMilliseconds
  if ($null -eq $withBail -or $null -eq $withoutBail -or $withoutBail - $withBail -lt 1000) {
    throw 'The slow selected-test fixture did not demonstrate at least one second of bail savings.'
  }
  $summaries | ConvertTo-Json -Depth 6 | Set-Content (Join-Path $evidence 'outcomes.json')
  $summaries | Format-Table Scenario, ExitCode, Statuses, FirstMutantMilliseconds
} finally {
  $env:STRYKER_CANCELLATION_SCENARIO = $previousScenario
  $env:STRYKER_CANCELLATION_EVIDENCE = $previousEvidence
  $env:STRYKER_MTP_CANCELLATION_FIXTURE = $previousFixture
}
