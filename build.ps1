# Set script parameters
$ErrorActionPreference = "Stop"
if (-not $env:Configuration) { $env:Configuration = "Release" }

# Install .NET CLI Tools
if ($env:BuildRunner) { Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" | Invoke-Expression }

# Run GitVersion
$env:PackageVersion = (gitversion | ConvertFrom-Json).NuGetVersionV2

# Helper functions
function Invoke-BuildStep { param([scriptblock]$cmd) & $cmd; if ($LASTEXITCODE -ne 0) { exit 1 } }

# Run build
Push-Location ./src/GraphQL.BatchResolver
Invoke-BuildStep { dotnet restore }
Invoke-BuildStep { dotnet pack --include-symbols }
Pop-Location

# End
exit
