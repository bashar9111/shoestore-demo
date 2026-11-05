$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'Build.ps1')
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
$run = Join-Path $PSScriptRoot ('.test-work\' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'WebApplication5\bin\ShoeStoreDemo.dll') -Destination $run
& $compiler /nologo "/out:$run\DemoTests.exe" "/reference:$run\ShoeStoreDemo.dll" /reference:System.Web.dll (Join-Path $PSScriptRoot 'tests\DemoTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& (Join-Path $run 'DemoTests.exe') (Join-Path $run 'test-store.xml')
if ($LASTEXITCODE -ne 0) { throw 'Domain tests failed.' }
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'WebApplication5') -Destination (Join-Path $run 'app') -Recurse
& $compiler /nologo "/out:$run\FlowTests.exe" /reference:System.Web.dll (Join-Path $PSScriptRoot 'tests\FlowTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Flow test compilation failed.' }
Copy-Item -LiteralPath (Join-Path $run 'FlowTests.exe') -Destination (Join-Path $run 'app\bin\FlowTests.exe')
& (Join-Path $run 'FlowTests.exe') ((Join-Path $run 'app') + '\')
if ($LASTEXITCODE -ne 0) { throw 'Flow tests failed.' }
Write-Host 'All 27 checks passed. Test data remains isolated in .test-work.'
