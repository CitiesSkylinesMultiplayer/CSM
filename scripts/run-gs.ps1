#!/bin/pwsh
#########################################################################
# This script runs the CSM.GS server locally (the NAT relay / public   #
# server list service). Useful for testing hosting a server against a #
# self-hosted API server instead of the official one.                  #
#########################################################################

# Params that can be passed in
param (
    [int]$Port = 4240,
    [int]$HttpPort = 4241,
    [string]$Configuration = "Debug"
)

Write-Host "[CSM GS Script] Starting the CSM.GS server locally."
Write-Host "[CSM GS Script] UDP (NAT relay) port: $($Port)"
Write-Host "[CSM GS Script] HTTP (public server list) port: $($HttpPort)"
Write-Host "[CSM GS Script] Point your game's 'CSM API Server' setting at this machine, with matching ports, to use it."

$env:GS_PORT = $Port
$env:GS_HTTP_PORT = $HttpPort

# CSM.GS targets net7.0, which may not be installed if only a newer .NET is
# present. Roll forward to whatever major version is available.
$env:DOTNET_ROLL_FORWARD = "LatestMajor"

dotnet run --project "$PSScriptRoot/../src/gs/CSM.GS.csproj" --configuration $Configuration
