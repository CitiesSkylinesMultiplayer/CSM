#!/bin/pwsh
# Installer script to be bundled with release files
param (
    [string]$ModDirectory = "Default"
)

# Assume windows, since PowerShell 6 is the first with official cross-platform support
If ($PSVersionTable.PSVersion.Major -lt 6)
{
    $IsWindows = $true
}

$Sep = "\"
If (($IsMacOS) -or ($IsLinux))
{
    $Sep = "/"
}

If ($ModDirectory -eq "Default")
{
    If ($IsLinux)
    {
        $ModDirectory = "~/.local/share/Colossal Order/Cities_Skylines/Addons/Mods/CSM"
    }
    ElseIf ($IsMacOS)
    {
        $ModDirectory = "~/Library/Application Support/Colossal Order/Cities_Skylines/Addons/Mods/CSM"
    }
    ElseIf ($IsWindows)
    {
        $ModDirectory = "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\CSM"
    }
}

Write-Host "Installing..."

# Delete directory and contents
Remove-Item $ModDirectory -Recurse -ErrorAction Ignore

# Make sure the game mod directory exists
New-Item -ItemType directory -Path $ModDirectory | Out-Null

# Copy the items
Copy-Item -Path "LiteNetLib.dll"        -Destination "$($ModDirectory)$($Sep)LiteNetLib.dll" -Force
Copy-Item -Path "protobuf-net.dll"      -Destination "$($ModDirectory)$($Sep)protobuf-net.dll" -Force
Copy-Item -Path "CSM.dll"               -Destination "$($ModDirectory)$($Sep)CSM.dll" -Force
Copy-Item -Path "Open.Nat.dll"          -Destination "$($ModDirectory)$($Sep)Open.Nat.dll" -Force
Copy-Item -Path "System.Threading.dll"  -Destination "$($ModDirectory)$($Sep)System.Threading.dll" -Force
Copy-Item -Path "0Harmony.dll"          -Destination "$($ModDirectory)$($Sep)0Harmony.dll" -Force
Copy-Item -Path "NLog.dll"         	-Destination "$($ModDirectory)$($Sep)NLog.dll" -Force

Write-Host "Mod is now installed, open Cities Skylines and enable the mod."
