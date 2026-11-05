$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'WebApplication5'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw 'Install .NET Framework 4.8 on Windows first.' }
New-Item -ItemType Directory -Path (Join-Path $project 'bin') -Force | Out-Null
$sources = Get-ChildItem -LiteralPath $project -Recurse -Filter '*.cs' | Select-Object -ExpandProperty FullName
& $compiler /nologo /target:library "/out:$project\bin\ShoeStoreDemo.dll" /reference:System.Web.dll /reference:System.Core.dll /reference:System.Data.dll /reference:System.Xml.dll $sources
if ($LASTEXITCODE -ne 0) { throw 'Compilation failed.' }
Write-Host 'Build passed.'
