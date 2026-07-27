#!/bin/pwsh
#########################################################################
# This script copies the locally built CSM mod .dll files to a remote  #
# machine's Cities: Skylines mod folder (e.g. deploying from a Windows #
# dev box to a Mac for testing).                                       #
#########################################################################

# Params that can be passed in
param (
    [string]$RemoteUser,
    [string]$RemoteHost = "varuns-macbook-pro",
    [string]$RemoteOS = "mac",
    [string]$SourceDirectory = "..\src\csm\bin\Release",
    [string]$RemoteModDirectory = "Default"
)

if ([string]::IsNullOrEmpty($RemoteUser))
{
    Write-Host "[CSM Deploy Script] You must specify -RemoteUser (the SSH username on $($RemoteHost))."
    exit 1
}

If ($RemoteModDirectory -eq "Default")
{
    Switch ($RemoteOS.ToLower())
    {
        "mac"     { $RemoteModDirectory = "~/Library/Application Support/Colossal Order/Cities_Skylines/Addons/Mods/CSM" }
        "linux"   { $RemoteModDirectory = "~/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/CSM" }
        default
        {
            Write-Host "[CSM Deploy Script] Unknown -RemoteOS '$($RemoteOS)'. Expected 'mac' or 'linux', or pass -RemoteModDirectory explicitly."
            exit 1
        }
    }
}

$RemoteTarget = "$($RemoteUser)@$($RemoteHost)"

Write-Host "[CSM Deploy Script] Source directory: $($SourceDirectory)"
Write-Host "[CSM Deploy Script] Remote target: $($RemoteTarget)"
Write-Host "[CSM Deploy Script] Remote mod directory: $($RemoteModDirectory)"

# Make sure there is something to deploy
$Dlls = Get-ChildItem -Path $SourceDirectory -Filter "*.dll" -ErrorAction SilentlyContinue
If (-not $Dlls)
{
    Write-Host "[CSM Deploy Script] No .dll files found in $($SourceDirectory). Did you run build.ps1 -Build first?"
    exit 1
}

# Make sure the remote mod directory exists
Write-Host "[CSM Deploy Script] Ensuring remote mod directory exists..."
ssh $RemoteTarget "mkdir -p '$($RemoteModDirectory)'"
If ($LASTEXITCODE -ne 0)
{
    Write-Host "[CSM Deploy Script] Failed to create/verify the remote mod directory. Check SSH connectivity to $($RemoteTarget)."
    exit $LASTEXITCODE
}

# Copy the dll files across
Write-Host "[CSM Deploy Script] Copying $($Dlls.Count) file(s)..."
scp "$($SourceDirectory)\*.dll" "$($RemoteTarget):$($RemoteModDirectory)/"
If ($LASTEXITCODE -ne 0)
{
    Write-Host "[CSM Deploy Script] Deploy failed!"
    exit $LASTEXITCODE
}

Write-Host "[CSM Deploy Script] Done! Files deployed to $($RemoteTarget):$($RemoteModDirectory)"
