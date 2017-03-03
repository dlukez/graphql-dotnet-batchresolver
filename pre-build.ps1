# Install .NET CLI Tools
if ($env:BuildRunner) { Invoke-WebRequest -Url "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" | Invoke-Expression }

# Add path to the .NET CLI
Write-Output "##myget[setParameter name='PATH' value='$env:PATH']"

# Run GitVersion
Invoke-Expression "$env:GitVersion /output buildserver"
