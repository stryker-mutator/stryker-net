#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

<#
.SYNOPSIS
  Verifies that the default values documented in docs/configuration.md match the actual
  `Default =>` values declared in the Stryker Configuration Input classes.

.DESCRIPTION
  Parses every "### `<option-name>`" section of the configuration documentation and extracts
  its "Default:" line. Then parses every Input class under
  src/Stryker.Configuration/Options/Inputs and extracts its `Default =>` expression.

  Every documented default is enforced - nothing is silently skipped. Literal expressions
  (numbers, booleans, single string literals, string collections and null) are compared
  directly. Derived entries (constants, cross-class defaults, computed formulas, composite
  objects, pass-through prose) are checked against their concrete code anchor through
  per-option comparison modes, so a change on either side must be reflected on the other.

  Exits non-zero when a documented default mismatches its code anchor, or when a documented
  default has no code anchor at all (this prevents new options from silently drifting).

  In GitHub Actions (detected via $env:GITHUB_ACTIONS, or forced with -CI) each mismatch is
  also emitted as a workflow error annotation pointing at the offending docs line.

.PARAMETER RepoRoot
  Root of the repository to check. Defaults to the directory containing this script.

.PARAMETER DocsRelativePath
  Path of the configuration documentation, relative to RepoRoot.

.PARAMETER CI
  Force CI mode (error annotations in GitHub Actions format), even when not running on
  GitHub Actions.

.EXAMPLE
  pwsh ./check-docs-defaults.ps1
#>

param(
    [string]$RepoRoot = $PSScriptRoot,
    [string]$DocsRelativePath = 'docs/configuration.md',
    [switch]$CI
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$ciMode = $CI.IsPresent -or $env:GITHUB_ACTIONS -eq 'true'

function Write-Info([string]$message) { Write-Host "[INFO] $message" }
function Write-Fail([string]$message) { Write-Host "[FAIL] $message" -ForegroundColor Red }
function ConvertTo-RelativePath([string]$path) {
    [System.IO.Path]::GetRelativePath($RepoRoot, $path)
}
function Write-Annotation([string]$message, [string]$file, [int]$line) {
    if ($ciMode) {
        Write-Host "::error file=$file,line=$line::$(ConvertTo-AnnotationSafe $message)"
    } else {
        Write-Fail "$file`:$line $message"
    }
}

# Workflow-commands spec: annotation messages must have %, CR and LF percent-encoded,
# otherwise special characters corrupt the annotation.
function ConvertTo-AnnotationSafe([string]$message) {
    $message -replace '%', '%25' -replace "`r", '%0D' -replace "`n", '%0A'
}

$docsPath = Join-Path $RepoRoot $DocsRelativePath
$inputsPath = Join-Path $RepoRoot 'src/Stryker.Configuration/Options/Inputs'
if (-not (Test-Path $docsPath)) { throw "Documentation file not found: $docsPath" }
if (-not (Test-Path $inputsPath)) { throw "Inputs directory not found: $inputsPath" }

# -----------------------------------------------------------------------------
# Docs extraction: "### `<option-name>`" sections with a "Default:" line
# -----------------------------------------------------------------------------

function Get-DocsDefaults([string]$path) {
    $lines = Get-Content -LiteralPath $path
    $sections = [ordered]@{}
    $current = $null
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^#{2,4} ') {
            # any heading resets section tracking, so a stray Default: line under a
            # non-option heading is never attributed to the previous option
            if ($lines[$i] -match '^### `(?<name>[a-z0-9.\-]+)`') {
                $current = [pscustomobject]@{
                    Name          = $Matches['name']
                    HeadingLine   = $i + 1
                    DefaultRaw    = $null
                    DefaultLine   = 0
                }
                $sections[$current.Name] = $current
            } else {
                $current = $null
            }
            continue
        }
        if ($null -ne $current -and $null -eq $current.DefaultRaw -and $lines[$i] -match '^Default:\s*(?<value>.*)$') {
            $current.DefaultRaw = $Matches['value'].Trim()
            $current.DefaultLine = $i + 1
        }
    }
    @($sections.Values | Where-Object { $null -ne $_.DefaultRaw })
}

# -----------------------------------------------------------------------------
# Code extraction: `Default => <expr>` in the Input classes
# -----------------------------------------------------------------------------

function Get-CodeDefaults([string]$directory) {
    $defaults = @{}
    Get-ChildItem -LiteralPath $directory -Filter '*.cs' | ForEach-Object {
        $file = $_
        $lines = Get-Content -LiteralPath $file.FullName
        for ($i = 0; $i -lt $lines.Count; $i++) {
            if ($lines[$i] -notmatch 'public\s+override\s+[\w<>?,\.\[\]\s]+\s+Default\s*=>\s*(?<expr>.+?);\s*$') { continue }
            $expr = $Matches['expr'].Trim()
            $className = $file.BaseName
            if ($defaults.ContainsKey($className)) {
                throw "Duplicate Default declaration found for $className in $($file.FullName)"
            }
            $defaults[$className] = [pscustomobject]@{
                ClassName = $className
                File      = $file.FullName
                Line      = $i + 1
                Expression = $expr
                Parsed    = Convert-CodeExpression $expr $file.FullName
            }
        }
    }
    $defaults
}

# Parses a `Default =>` expression into a comparable value, or marks it non-literal.
function Convert-CodeExpression([string]$expr, [string]$file) {
    if ($expr -eq 'null') {
        return [pscustomobject]@{ Kind = 'null'; Value = $null }
    }
    if ($expr -match '^-?\d+(\.\d+)?$') {
        return [pscustomobject]@{ Kind = 'number'; Value = [double]$expr }
    }
    if ($expr -imatch '^(true|false)$') {
        # Do NOT use [bool]$expr here: [bool]'false' is $true (non-empty string is truthy).
        return [pscustomobject]@{ Kind = 'bool'; Value = ($expr -ieq 'true') }
    }
    if ($expr -eq 'string.Empty') {
        return [pscustomobject]@{ Kind = 'string'; Value = '' }
    }
    if ($expr -match '^"(?<text>(?:[^"\\]|\\.)*)"$') {
        return [pscustomobject]@{ Kind = 'string'; Value = [regex]::Unescape($Matches['text']) }
    }
    if ($expr -imatch '^(?<enum>[\w\.]+)\.(?<member>\w+)\.ToString\(\)$') {
        # e.g. MutationLevel.Standard.ToString() - the docs document the member name
        return [pscustomobject]@{ Kind = 'string'; Value = $Matches['member'] }
    }
    # cross-class default: e.g. Default => new SinceTargetInput().Default -
    # resolved after all classes are parsed (see the cross-reference pass in Main)
    if ($expr -imatch '^new\s+(?<class>\w+)\(\)\.Default$') {
        return [pscustomobject]@{ Kind = 'crossref'; Value = $Matches['class'] }
    }
    # string collections: new List<string> { "A", "B" }, string[] { "A" }
    if ($expr -match '^(new\s|string\[\s*\]\s*\{)') {
        if ($expr -match '\{(?<items>[^}]*)\}') {
            $items = @(Get-QuotedLiterals $Matches['items'])
            # non-string members (e.g. `new List<string> { _defaultInput }`) are resolved
            # as constants defined in the same file; anything unresolvable is non-literal
            $residual = ($Matches['items'] -replace '"(?:[^"\\]|\\.)*"', '') -replace '\s', ''
            if ($residual -ne '') {
                foreach ($token in ($residual -split ',' | Where-Object { $_ -ne '' })) {
                    $resolved = Resolve-ConstantInFile $token $file
                    if ($null -eq $resolved) {
                        return [pscustomobject]@{ Kind = 'nonliteral'; Value = $expr }
                    }
                    $items += $resolved
                }
            }
            return [pscustomobject]@{ Kind = 'list'; Value = $items }
        }
    }
    if ($expr -match 'Enumerable\.Empty<[\w<>\s]+>\(\)') {
        return [pscustomobject]@{ Kind = 'list'; Value = @() }
    }
    # bare identifier: try to resolve a constant like `private const string DefaultUrl = "..."` in the same file
    if ($expr -match '^[A-Za-z_]\w*$') {
        $resolved = Resolve-ConstantInFile $expr $file
        if ($null -ne $resolved) {
            return [pscustomobject]@{ Kind = 'string'; Value = $resolved }
        }
    }
    return [pscustomobject]@{ Kind = 'nonliteral'; Value = $expr }
}

# Extracts every double-quoted string literal from a C# expression fragment.
function Get-QuotedLiterals([string]$text) {
    @([regex]::Matches($text, '"(?<text>(?:[^"\\]|\\.)*)"') |
        ForEach-Object { [regex]::Unescape($_.Groups['text'].Value) })
}

function Resolve-ConstantInFile([string]$identifier, [string]$file) {
    $text = Get-Content -LiteralPath $file -Raw
    if ($text -match "\b$identifier\s*=\s*`"(?<value>[^""]*)`"") {
        return $Matches['value']
    }
    return $null
}

# -----------------------------------------------------------------------------
# Mapping table: docs option name -> Input class name
# Auto-mapping covers simple names (kebab-case -> PascalCase + 'Input');
# the table below lists the exceptions.
# -----------------------------------------------------------------------------

$explicitMapping = @{
    'project'                       = 'SourceProjectNameInput'
    'project-info.name'             = 'ProjectNameInput'
    'project-info.module'           = 'ModuleNameInput'
    'project-info.version'          = 'ProjectVersionInput'
    'reporter'                      = 'ReportersInput'
    'break-at'                      = 'ThresholdBreakInput'
    'since.enabled'                 = 'SinceInput'
    'since.target'                  = 'SinceTargetInput'
    'since.ignore-changes-in'       = 'DiffIgnoreChangesInput'
    'baseline.enabled'              = 'WithBaselineInput'
    'baseline.fallback-version'     = 'FallbackVersionInput'
    'baseline.provider'             = 'BaselineProviderInput'
    'baseline.azure-fileshare-url'  = 'AzureFileStorageUrlInput'
    'azure-fileshare-sas'           = 'AzureFileStorageSasInput'
    'baseline.s3-bucket-name'       = 'S3BucketNameInput'
    'baseline.s3-region'            = 'S3RegionInput'
    'baseline.s3-endpoint'          = 'S3EndpointInput'
    'dev-mode'                      = 'DiagModeInput'
    'output'                        = 'OutputPathInput'
    'dashboard-url'                 = 'DashboardUrlInput'
}

# Custom code locations: options whose default is defined outside the Input classes,
# but still has a concrete, checkable code anchor. Every skip-list entry that has a
# knowable code location belongs here instead - a skip-list entry without one is a
# silent blind spot for docs drift.
$customCodeLocations = @{
    # Docs documents the first of the fallback config file names, so compare as a
    # member of the code list rather than as an exact set.
    'config-file' = @{
        File    = 'src/Stryker.CLI/Stryker.CLI/ConfigBuilder.cs'
        Pattern = '_defaultConfigFileNames\s*=\s*\[(?<items>[^\]]*)\]'
        Mode    = 'member'
    }
    # skip-version-check's default is documented in the CLI option help text
    'skip-version-check' = @{
        File    = 'src/Stryker.CLI/Stryker.CLI/CommandLineConfig/CommandLineConfigReader.cs'
        Pattern = 'AddCliOnlyInput\("skip-version-check".*\| default:\s*(?<value>\w+)'
        Mode    = 'string'
    }
    # concurrency's computed default: the divisor in the code formula must match the
    # divisor quoted in the docs prose ("your number of logical processors / N")
    'concurrency' = @{
        File    = 'src/Stryker.Configuration/Options/Inputs/ConcurrencyInput.cs'
        Pattern = 'Environment\.ProcessorCount\s*/\s*(?<value>\d+)'
        Mode    = 'formula'
    }
    # the docs document `null` (absent = auto-detect); that semantics only holds while
    # the code flag literal stays false - any other literal means docs drift
    'since.enabled' = @{
        File    = 'src/Stryker.Configuration/Options/Inputs/SinceInput.cs'
        Pattern = 'Default\s*=>\s*(?<value>true|false)'
        Mode    = 'autoDetect'
    }
    'baseline.enabled' = @{
        File    = 'src/Stryker.Configuration/Options/Inputs/WithBaselineInput.cs'
        Pattern = 'Default\s*=>\s*(?<value>true|false)'
        Mode    = 'autoDetect'
    }
}

# Validate custom locations up front: a typo in Mode or a pattern without the expected
# capture group would otherwise fail silently (or fall through mid-loop).
$knownModes = @('set', 'member', 'string', 'formula', 'autoDetect')
foreach ($entry in $customCodeLocations.Values) {
    if ($knownModes -notcontains $entry.Mode) {
        throw "Unknown comparison mode '$($entry.Mode)' in a custom code location (expected one of: $($knownModes -join ', '))"
    }
    if ($entry.Pattern -notmatch '\(\?<items>' -and $entry.Pattern -notmatch '\(\?<value>') {
        throw "Custom location pattern '$($entry.Pattern)' must capture either (?<items>...) or (?<value>...)"
    }
}

# -----------------------------------------------------------------------------
# Per-option enforcement configuration - full coverage. Every documented default is
# enforced either through direct literal comparison or through one of the modes below
# that anchors it to its real code definition.
# -----------------------------------------------------------------------------

# Comparison mode overrides for options resolved through the normal Input mapping.
$optionModes = @{
    # code default is null (inherited from the project/SDK); the docs entry must stay
    # prose describing that source - a literal docs value here means drift
    'language-version'          = 'passthrough'
    'configuration'             = 'passthrough'
    'target-framework'          = 'passthrough'
    # docs value is a markdown link to another option; the code must keep delegating to
    # that option's input (cross-class resolution) and the link must keep pointing at it
    'baseline.fallback-version' = 'crossref'
}

# For 'crossref' mode: the option the docs link must point at.
$crossReferences = @{
    'baseline.fallback-version' = 'since.target'
}

# Composite docs objects: every documented key maps to its own Input class.
$compositeChecks = [ordered]@{
    'thresholds' = [ordered]@{
        'high'  = 'ThresholdHighInput'
        'low'   = 'ThresholdLowInput'
        'break' = 'ThresholdBreakInput'
    }
}

function ConvertTo-NormalizedOptionName([string]$name) {
    ($name -replace '[\.\-]', '').ToLowerInvariant()
}

function Get-InputClassName([string]$docsName, [hashtable]$codeDefaults) {
    $explicit = $explicitMapping[$docsName]
    if ($explicit) { return $explicit }

    # auto-map: additional-timeout -> AdditionalTimeoutInput
    $pascal = ($docsName -split '-' | ForEach-Object {
        if ($_.Length -gt 0) { $_.Substring(0, 1).ToUpperInvariant() + $_.Substring(1) } else { $_ }
    }) -join ''
    $candidate = "${pascal}Input"
    if ($codeDefaults.ContainsKey($candidate)) { return $candidate }
    return $null
}

# Extracts a default value from a non-Input code location (see $customCodeLocations).
# The location Pattern must capture either `(?<items>...)` (a list of quoted fallbacks)
# or `(?<value>...)` (a single literal). Returns a parsed code value object, or $null
# when the file or pattern is not found.
function Get-CustomCodeValue([hashtable]$location, [string]$repoRoot) {
    $path = Join-Path $repoRoot $location.File
    if (-not (Test-Path $path)) { return $null }
    $lines = Get-Content -LiteralPath $path
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -notmatch $location.Pattern) { continue }
        if ($Matches.ContainsKey('items')) {
            return [pscustomobject]@{
                Kind = 'list'
                Value = @(Get-QuotedLiterals $Matches['items'])
                Line = $i + 1
            }
        }
        return [pscustomobject]@{
            Kind = 'string'
            Value = $Matches['value']
            Line = $i + 1
        }
    }
    return $null
}

# -----------------------------------------------------------------------------
# Comparison
# -----------------------------------------------------------------------------

# Docs convention: optional inputs without a value are documented as `null`,
# while the code uses string.Empty / empty collections for the same state.
function Test-DocsValueMatchesCode([string]$docsValue, [pscustomobject]$code, [string]$comparisonMode = 'set') {
    # Docs convention: optional inputs are documented as `null`, while the code uses
    # string.Empty / empty collections for the same "not set" state - treat them as equal.
    $docsTrimmed = $docsValue.Trim()
    $docsIsBackticked = $docsTrimmed -match '^`(?<inner>[^`]+)`$'
    $docsInner = if ($docsIsBackticked) { $Matches['inner'].Trim() } else { $docsTrimmed }
    # strip a wrapping pair of double quotes from the docs value (`""` means empty string)
    if ($docsInner -match '^"(?<quoted>[^"]*)"$') {
        $docsInner = $Matches['quoted']
    }

    switch ($code.Kind) {
        'null' {
            if ($docsIsBackticked -and $docsInner -ieq 'null') { return $true }
            if ($comparisonMode -eq 'passthrough') {
                # the code default is null (inherited from the project/SDK); the docs entry
                # must stay prose describing that source - a documented literal here means
                # the docs claim a concrete value the code does not define
                return $docsTrimmed -notmatch '^`[^`]+`$'
            }
            return $false
        }
        'number' {
            if ($docsInner -notmatch '^-?\d+(\.\d+)?$') { return $false }
            return ([double]$docsInner) -eq [double]$code.Value
        }
        'bool' {
            $docsBool = $docsInner -ieq 'true' ? $true : ($docsInner -ieq 'false' ? $false : $null)
            if ($null -eq $docsBool) { return $false }
            return $docsBool -eq [bool]$code.Value
        }
        'string' {
            if (-not $docsIsBackticked) { return $false }
            if ($comparisonMode -eq 'formula') {
                # the code value is the divisor; the docs prose must quote the same one
                if ($docsInner -notmatch '/\s*(?<divisor>\d+)') { return $false }
                return [double]$Matches['divisor'] -eq [double]$code.Value
            }
            if ($comparisonMode -eq 'autoDetect') {
                # the docs document null (absent = auto-detect); that only holds while the
                # code flag literal stays false
                if ($docsInner -ieq 'null') { return [string]$code.Value -ieq 'false' }
                $docsBool = $docsInner -ieq 'true' ? $true : ($docsInner -ieq 'false' ? $false : $null)
                return $null -ne $docsBool -and $docsBool -eq ($code.Value -ieq 'true')
            }
            if ($docsInner -ieq 'null') { return [string]$code.Value -eq '' }
            return $docsInner -ieq [string]$code.Value
        }
        'list' {
            if (-not $docsIsBackticked) { return $false }
            $docsItems = @($docsInner -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_.Length -gt 0 })
            $codeItems = @([string[]]$code.Value)
            if ($docsInner -ieq 'null') { return $codeItems.Count -eq 0 }
            # 'member' mode: the docs document one of several code fallbacks (e.g. config file names)
            if ($comparisonMode -eq 'member') {
                if ($docsItems.Count -ne 1) { return $false }
                return @($codeItems | Where-Object { $_ -ieq $docsItems[0] }).Count -gt 0
            }
            if ($docsItems.Count -ne $codeItems.Count) { return $false }
            $sortedDocs = @($docsItems | Sort-Object { $_.ToLowerInvariant() })
            $sortedCode = @($codeItems | Sort-Object { $_.ToLowerInvariant() })
            for ($i = 0; $i -lt $sortedDocs.Count; $i++) {
                if ($sortedDocs[$i] -ine $sortedCode[$i]) { return $false }
            }
            return $true
        }
        default { return $false }
    }
}

function Format-CodeValue([pscustomobject]$code) {
    switch ($code.Kind) {
        'null' { return 'null' }
        'bool' { return ([bool]$code.Value).ToString().ToLowerInvariant() }
        'list' { return '"' + ($code.Value -join '", "') + '"' }
        default { return [string]$code.Value }
    }
}

# -----------------------------------------------------------------------------
# Main
# -----------------------------------------------------------------------------

# One result row per option, for the console log and the CI job summary.
$summaryRows = @()
function Add-SummaryRow([string]$name, [string]$docsValue, [string]$codeValue, [string]$status, [string]$source) {
    $script:summaryRows += [pscustomobject]@{
        Option = $name
        Docs   = $docsValue
        Code   = $codeValue
        Status = $status
        Source = $source
    }
}

$docsDefaults = Get-DocsDefaults $docsPath
$codeDefaults = Get-CodeDefaults $inputsPath

# Resolve cross-class defaults (Kind 'crossref') now that every class is parsed; an
# unresolvable reference must fail loudly rather than silently pass.
foreach ($entry in $codeDefaults.Values) {
    if ($entry.Parsed.Kind -ne 'crossref') { continue }
    $refClassName = $entry.Parsed.Value
    $refParsed = if ($codeDefaults.ContainsKey($refClassName)) { $codeDefaults[$refClassName].Parsed } else { $null }
    if ($null -eq $refParsed -or $refParsed.Kind -in @('nonliteral', 'crossref')) {
        $entry.Parsed = [pscustomobject]@{ Kind = 'nonliteral'; Value = "$($entry.Expression) (unresolvable cross-reference to $refClassName)" }
    } else {
        $entry.Parsed = $refParsed
    }
}

$checked = 0
$mismatches = 0

Write-Info ("Checking {0} documented defaults in {1} against {2} Input classes and {3} custom code anchors" -f $docsDefaults.Count, $DocsRelativePath, $codeDefaults.Count, $customCodeLocations.Count)

foreach ($docsEntry in $docsDefaults) {
    $name = $docsEntry.Name

    # Composite docs objects: every documented key maps to its own Input class
    if ($compositeChecks.Contains($name)) {
        $checks = $compositeChecks[$name]
        $pairs = @{}
        foreach ($pair in [regex]::Matches($docsEntry.DefaultRaw, '(?<key>\w+):\s*(?<value>-?\d+)')) {
            $pairs[$pair.Groups['key'].Value.ToLowerInvariant()] = $pair.Groups['value'].Value
        }
        $checked++
        $compositeOk = $true
        foreach ($key in $checks.Keys) {
            $inputName = $checks[$key]
            if (-not $pairs.ContainsKey($key)) {
                Write-Annotation "option '$name': documented object is missing the '$key' key (expected by $inputName)" $DocsRelativePath $docsEntry.DefaultLine
                $compositeOk = $false
                continue
            }
            $keyCode = if ($codeDefaults.ContainsKey($inputName)) { $codeDefaults[$inputName].Parsed } else { $null }
            if ($null -eq $keyCode -or $keyCode.Kind -ne 'number' -or [double]$pairs[$key] -ne [double]$keyCode.Value) {
                $keyValue = if ($null -ne $keyCode) { Format-CodeValue $keyCode } else { '<missing>' }
                Write-Annotation "option '${name}' key '${key}': documented value '$($pairs[$key])' does not match code default '$keyValue' ($inputName)" $DocsRelativePath $docsEntry.DefaultLine
                $compositeOk = $false
            }
        }
        foreach ($key in $pairs.Keys) {
            if (-not $checks.Contains($key)) {
                Write-Annotation "option '$name': documented key '$key' has no mapped Input class - update compositeChecks in check-docs-defaults.ps1" $DocsRelativePath $docsEntry.DefaultLine
                $compositeOk = $false
            }
        }
        $compositeCode = ($checks.Keys | ForEach-Object {
            $keyParsed = $codeDefaults[$checks[$_]].Parsed
            "$_=$(if ($keyParsed.Kind -eq 'nonliteral') { '<unresolvable>' } else { Format-CodeValue $keyParsed })"
        }) -join ', '
        $status = if ($compositeOk) { 'ok' } else { 'MISMATCH' }
        Add-SummaryRow $name $docsEntry.DefaultRaw $compositeCode $status 'threshold input classes'
        if ($compositeOk) {
            Write-Host "[ OK ] $name = $($docsEntry.DefaultRaw)" -ForegroundColor Green
        }
        continue
    }

    # Option with a concrete code anchor outside the Input classes
    if ($customCodeLocations.ContainsKey($name)) {
        $location = $customCodeLocations[$name]
        $customCode = Get-CustomCodeValue $location $RepoRoot
        if ($null -eq $customCode) {
            Write-Annotation "option '$name': default not found in $($location.File) (pattern '$($location.Pattern)') - the code moved, update check-docs-defaults.ps1" $DocsRelativePath $docsEntry.HeadingLine
            $mismatches++
            continue
        }
        $checked++
        if (-not (Test-DocsValueMatchesCode $docsEntry.DefaultRaw $customCode $location.Mode)) {
            $message = "option '$name': documented default '$($docsEntry.DefaultRaw)' does not match code default '$(Format-CodeValue $customCode)' defined in $($location.File):$($customCode.Line)"
            Write-Annotation $message $DocsRelativePath $docsEntry.DefaultLine
            Add-SummaryRow $name $docsEntry.DefaultRaw (Format-CodeValue $customCode) 'MISMATCH' "$(ConvertTo-RelativePath (Join-Path $RepoRoot $location.File)):$($customCode.Line)"
            $mismatches++
        } else {
            $source = "$(ConvertTo-RelativePath (Join-Path $RepoRoot $location.File)):$($customCode.Line)"
            Add-SummaryRow $name $docsEntry.DefaultRaw (Format-CodeValue $customCode) 'ok' $source
            Write-Host "[ OK ] $name = $(Format-CodeValue $customCode) (via $source)" -ForegroundColor Green
        }
        continue
    }

    $className = Get-InputClassName $name $codeDefaults
    if (-not $className -or -not $codeDefaults.ContainsKey($className)) {
        Write-Annotation "option '$name' has no matching Input class - update the mapping table in check-docs-defaults.ps1" $DocsRelativePath $docsEntry.HeadingLine
        Add-SummaryRow $name $docsEntry.DefaultRaw '<no code anchor found>' 'MISMATCH' 'mapping table'
        $mismatches++
        continue
    }

    $code = $codeDefaults[$className]
    if ($code.Parsed.Kind -eq 'nonliteral') {
        # Reaching this point means the entry is mapped but its code default is not
        # comparable - the option needs an enforcement mode or a custom code location.
        Write-Annotation "option '$name' maps to $className whose Default is not comparable ('$($code.Expression)') - extend check-docs-defaults.ps1" $DocsRelativePath $docsEntry.HeadingLine
        Add-SummaryRow $name $docsEntry.DefaultRaw $code.Expression 'MISMATCH' "$(ConvertTo-RelativePath $code.File):$($code.Line)"
        $mismatches++
        continue
    }

    $mode = $optionModes.ContainsKey($name) ? $optionModes[$name] : 'set'
    $source = "$(ConvertTo-RelativePath $code.File):$($code.Line)"

    if ($mode -eq 'crossref') {
        $reference = $crossReferences[$name]
        $label = if ($docsEntry.DefaultRaw -match '^\[(?<label>[^\]]+)\]\(') { $Matches['label'] } else { $null }
        # the code must keep delegating to the referenced input (enforced by the
        # cross-reference resolution pass) and the docs link must keep pointing at it
        $delegates = $code.Expression -match '^new\s+\w+\(\)\.Default$'
        $checked++
        if ($delegates -and $null -ne $label -and (ConvertTo-NormalizedOptionName $label) -eq (ConvertTo-NormalizedOptionName $reference)) {
            Add-SummaryRow $name $docsEntry.DefaultRaw "delegates to $reference" 'ok' $source
            Write-Host "[ OK ] $name -> $reference (cross-reference)" -ForegroundColor Green
        } else {
            $reason = if (-not $delegates) { "code default '$($code.Expression)' no longer delegates to $reference" } else { "docs must link to '$reference' (found '$($docsEntry.DefaultRaw)')" }
            Write-Annotation "option '$name': $reason" $DocsRelativePath $docsEntry.DefaultLine
            Add-SummaryRow $name $docsEntry.DefaultRaw "must delegate to $reference" 'MISMATCH' $source
            $mismatches++
        }
        continue
    }

    $checked++
    if (-not (Test-DocsValueMatchesCode $docsEntry.DefaultRaw $code.Parsed $mode)) {
        $message = "option '$name': documented default '$($docsEntry.DefaultRaw)' does not match code default '$(Format-CodeValue $code.Parsed)' ($className)"
        Write-Annotation $message $DocsRelativePath $docsEntry.DefaultLine
        Write-Host "       code default defined at $source"
        Add-SummaryRow $name $docsEntry.DefaultRaw (Format-CodeValue $code.Parsed) 'MISMATCH' $source
        $mismatches++
    } else {
        Add-SummaryRow $name $docsEntry.DefaultRaw (Format-CodeValue $code.Parsed) 'ok' $source
        Write-Host "[ OK ] $name = $(Format-CodeValue $code.Parsed)" -ForegroundColor Green
    }
}

Write-Info "Done. Checked: $checked. Mismatches: $mismatches."

# Job summary: renders as a markdown table on the GitHub Actions run page, independent
# of how much log output scrolled past.
if ($env:GITHUB_STEP_SUMMARY) {
    $escape = { param($text) $text -replace '\|', '\|' }
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('# Docs defaults check')
    $lines.Add('')
    $lines.Add("Checked **$checked** documented defaults against code; **$mismatches** mismatch(es).")
    $lines.Add('')
    $lines.Add('| Option | Documented | Code | Defined in | Status |')
    $lines.Add('|---|---|---|---|---|')
    foreach ($row in $summaryRows) {
        $status = if ($row.Status -eq 'ok') { 'ok' } else { '**MISMATCH**' }
        $lines.Add(('| {0} | {1} | {2} | {3} | {4} |' -f `
            (& $escape $row.Option), (& $escape $row.Docs), (& $escape $row.Code), (& $escape $row.Source), $status))
    }
    Set-Content -LiteralPath $env:GITHUB_STEP_SUMMARY -Value $lines -Encoding utf8
}

if ($mismatches -gt 0) {
    Write-Fail 'Fix the documented values above (or the code defaults they point at), then re-run.'
    exit 1
}
exit 0