$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'UnityEditor.ps1')

$projectPath = Split-Path $PSScriptRoot -Parent
$resultsPath = Join-Path $projectPath 'TestResults.xml'
$logPath = Join-Path $projectPath 'unity-tests.log'

Invoke-StormframeUnity -Arguments @(
    '-batchmode',
    '-projectPath', $projectPath,
    '-runTests',
    '-testPlatform', 'EditMode',
    '-testResults', $resultsPath,
    '-logFile', $logPath
)

[xml] $results = Get-Content -LiteralPath $resultsPath
$run = $results.'test-run'
Write-Output "Tests: $($run.result); passed $($run.passed)/$($run.total)"
