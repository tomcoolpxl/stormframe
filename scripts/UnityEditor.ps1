function Get-StormframeUnityEditor {
    $versionFile = Join-Path $PSScriptRoot '..\ProjectSettings\ProjectVersion.txt'
    $versionLine = Get-Content -LiteralPath $versionFile -First 1
    $editorVersion = ($versionLine -split ':', 2)[1].Trim()
    $editorPath = "C:\Program Files\Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"

    if (-not (Test-Path -LiteralPath $editorPath)) {
        throw "Unity $editorVersion was not found at $editorPath"
    }

    return $editorPath
}

function Invoke-StormframeUnity {
    param(
        [Parameter(Mandatory)]
        [string[]] $Arguments
    )

    $editorPath = Get-StormframeUnityEditor
    $process = Start-Process `
        -FilePath $editorPath `
        -ArgumentList $Arguments `
        -PassThru `
        -Wait `
        -WindowStyle Hidden

    if ($process.ExitCode -ne 0) {
        throw "Unity exited with code $($process.ExitCode)."
    }
}
