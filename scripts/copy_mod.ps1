param (
    [switch]$update = $false,
    [switch]$build = $false
 )

# Introduction
Write-Host "This script will copy the required assemblies, build the mode and copy the mod and install it into your game."
Write-Host "You will need Cities: Skylines installed and Visual Studio (2017). Windows 10 is also recomended (other operating systems not supported/tested)."
Write-Host "------------------------------------"

# Variables
$ModDirectory = "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\CitiesSkylinesMultiplayer"
$OutputDirectory = "..\src\CitiesSkylinesMultiplayer\bin\Release"

# Make sure we have the latest assemblies
# only if the update flag is set
If ($update)
{
    Write-Host "Updaing Assemblies to current installed Cities: Skylines version. Please enter follow the next steps..."
    Invoke-Expression ".\update_assemblies.ps1"
}

# Build the project if the build
# flag is specified.
If ($build)
{
    Write-Host "Building mod. Please wait..."
    Invoke-Expression ".\build.ps1"
}

# Clear the directory
Write-Host "Clearing and creating mod directory"

# Delete directory and contents
Remove-Item $ModDirectory -Recurse -ErrorAction Ignore

# Make sure the game mod directory exists
New-Item -ItemType directory -Path $ModDirectory | Out-Null

# Copy the required files
Write-Host "Copying required files..."
Copy-Item -Path "$($OutputDirectory)\LiteNetLib.dll"                 -Destination "$($ModDirectory)\LiteNetLib.dll" -Force
Copy-Item -Path "$($OutputDirectory)\protobuf-net.dll"               -Destination "$($ModDirectory)\protobuf-net.dll" -Force
Copy-Item -Path "$($OutputDirectory)\CitiesSkylinesMultiplayer.dll"  -Destination "$($ModDirectory)\CitiesSkylinesMultiplayer.dll" -Force

# Done
Write-Host "Completed Copy"