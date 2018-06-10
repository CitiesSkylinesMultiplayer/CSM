# Installer script to be bundled with release files
param (
    [string]$ModDirectory = "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\CSM"
)

Write-Host "Installing..."

# Delete directory and contents
Remove-Item $ModDirectory -Recurse -ErrorAction Ignore

# Make sure the game mod directory exists
New-Item -ItemType directory -Path $ModDirectory | Out-Null

# Copy the items
Copy-Item -Path "LiteNetLib.dll"    -Destination "$($ModDirectory)\LiteNetLib.dll" -Force
Copy-Item -Path "protobuf-net.dll"  -Destination "$($ModDirectory)\protobuf-net.dll" -Force
Copy-Item -Path "CSM.dll"           -Destination "$($ModDirectory)\CSM.dll" -Force

Write-Host "Mod is now installed, open Cities Skylines and enable the mod."