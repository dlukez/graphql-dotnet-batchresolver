# Setup
$ErrorActionPreference = "Stop"
if (-not $env:Configuration) { $env:Configuration = "Release" }
if (-not $env:BuildRunner) { $env:PackageVersion = (gitversion | ConvertFrom-Json).NuGetVersionV2 }
function Invoke-BuildStep { param([scriptblock]$cmd) & $cmd; if ($LASTEXITCODE -ne 0) { exit 1 } }

# Build
Invoke-BuildStep { dotnet restore src/GraphQL.BatchResolver/GraphQL.BatchResolver.csproj }
Invoke-BuildStep { dotnet build src/GraphQL.BatchResolver/GraphQL.BatchResolver.csproj }
Invoke-BuildStep { dotnet pack src/GraphQL.BatchResolver/GraphQL.BatchResolver.csproj --include-symbols --no-build }

# Done
exit
