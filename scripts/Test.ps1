$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'UnityEditor.ps1')

$projectPath = Split-Path $PSScriptRoot -Parent
$platforms = @('EditMode', 'PlayMode')
$total = 0
$passed = 0

foreach ($platform in $platforms) {
    $platformResultsPath = Join-Path $projectPath "TestResults-$platform.xml"
    $platformLogPath = Join-Path $projectPath "unity-tests-$($platform.ToLowerInvariant()).log"

    Invoke-StormframeUnity -Arguments @(
        '-batchmode',
        '-projectPath', $projectPath,
        '-runTests',
        '-testPlatform', $platform,
        '-testResults', $platformResultsPath,
        '-logFile', $platformLogPath
    )

    [xml] $results = Get-Content -LiteralPath $platformResultsPath
    $run = $results.'test-run'
    $total += [int] $run.total
    $passed += [int] $run.passed
    Write-Output "$platform tests: $($run.result); passed $($run.passed)/$($run.total)"
}

Write-Output "All tests: passed $passed/$total"
