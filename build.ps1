# Set parameters
$ErrorActionPreference = "Stop"
if (-not $env:Configuration) { $env:Configuration = "Release" }
if (-not $env:BuildRunner) { $env:PackageVersion = (gitversion | ConvertFrom-Json).NuGetVersionV2 }

# Helper functions
function Invoke-BuildStep { param([scriptblock]$cmd) & $cmd; if ($LASTEXITCODE -ne 0) { exit 1 } }

# Run build
Push-Location ./src/GraphQL.BatchResolver
Invoke-BuildStep { dotnet restore }
Invoke-BuildStep { dotnet pack --include-symbols }
Pop-Location

# End
exit
