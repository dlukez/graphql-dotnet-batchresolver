# Setup
$ErrorActionPreference = "Stop"
if (-not $env:Configuration) { $env:Configuration = "Release" }
if (-not $env:BuildRunner) { $env:PackageVersion = (gitversion | ConvertFrom-Json).NuGetVersionV2 }
function Invoke-BuildStep { param([scriptblock]$cmd) & $cmd; if ($LASTEXITCODE -ne 0) { exit 1 } }

# Build
Push-Location src/GraphQL.BatchResolver/
try {
    Invoke-BuildStep { dotnet restore }
    Invoke-BuildStep { dotnet build }
    Invoke-BuildStep { dotnet pack --include-symbols --no-build }
} finally { Pop-Location }

# Done
exit
