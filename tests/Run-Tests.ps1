$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
Push-Location $projectRoot
try {
    dotnet build PokemonMemoryBattle.sln -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
    $testProcess = Start-Process -FilePath "$projectRoot/src/bin/Release/net8.0-windows/PokemonMemoryBattle.exe" -ArgumentList '--self-test' -WorkingDirectory $projectRoot -Wait -PassThru
    Get-Content artifacts/test-results.txt
    if ($testProcess.ExitCode -ne 0) { throw 'Self-tests failed' }
} finally { Pop-Location }
