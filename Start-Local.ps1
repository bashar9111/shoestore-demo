$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'Build.ps1')
$iis = Join-Path $env:ProgramFiles 'IIS Express\iisexpress.exe'
if (-not (Test-Path -LiteralPath $iis)) { $iis = Join-Path ${env:ProgramFiles(x86)} 'IIS Express\iisexpress.exe' }
if (-not (Test-Path -LiteralPath $iis)) { throw 'Install IIS Express or use Visual Studio ASP.NET tools.' }
Write-Host 'Open http://localhost:58241/ in your browser. Press Ctrl+C to stop.'
& $iis "/path:$PSScriptRoot\WebApplication5" /port:58241 /systray:false
if ($LASTEXITCODE -ne 0) { throw 'IIS Express did not start. Use Visual Studio to diagnose its configuration.' }
