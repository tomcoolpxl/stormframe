$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'UnityEditor.ps1')

$projectPath = Split-Path $PSScriptRoot -Parent
$buildPath = Join-Path $projectPath 'Builds\Windows\Stormframe.exe'
$logPath = Join-Path $projectPath 'unity-build.log'

Invoke-StormframeUnity -Arguments @(
    '-batchmode',
    '-quit',
    '-projectPath', $projectPath,
    '-buildWindows64Player', $buildPath,
    '-logFile', $logPath
)

Write-Output "Windows build: $buildPath"
