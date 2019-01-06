#########################################################################
# This script handles updating, building and installing the mod.        #
# Version 2.0 - Created by Dominic Maas                                 #
# Update: Copies the latest version of the assemblies.                  #
# Build: Builds the project using provided assemblies.                  #
# Install: Copies all files over to the cities skylines mod folder      #
#########################################################################

# Params that can be passed in
param (
    [switch]$Update = $false,
    [switch]$Build = $false,
    [switch]$Install = $false,
    [string]$OutputDirectory = "..\src\bin\Release",
    [string]$ModDirectory = "$env:LOCALAPPDATA\Colossal Order\Cities_Skylines\Addons\Mods\CSM"
 )

# Functions
Function Find-MsBuild([int] $MaxVersion = 2017)
{
    $agentPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild.exe"
    $devPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
    $proPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe"
    $communityPath = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
    $fallback2015Path = "${Env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
    $fallback2013Path = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
    $fallbackPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319"
		
    If ((2017 -le $MaxVersion) -And (Test-Path $agentPath)) { return $agentPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $devPath)) { return $devPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $proPath)) { return $proPath } 
    If ((2017 -le $MaxVersion) -And (Test-Path $communityPath)) { return $communityPath } 
    If ((2015 -le $MaxVersion) -And (Test-Path $fallback2015Path)) { return $fallback2015Path } 
    If ((2013 -le $MaxVersion) -And (Test-Path $fallback2013Path)) { return $fallback2013Path } 
    If (Test-Path $fallbackPath) { return $fallbackPath } 
        
    throw "Yikes - Unable to find msbuild"
}

# Introduction
Write-Host "[CSM Build Script] Welcome to the CSM build script. This script will now perform the specified actions.";
Write-Host "[CSM Build Script] Depending on your choice of options, you will need Visual Studio 2017 (or the 'Build Tools for Visual Studio 2017'), and Cities: Skylines insalled."

# Chosen Directories
Write-Host "[CSM Build Script] Output Directory: $($OutputDirectory)"
Write-Host "[CSM Build Script] Mod Directory: $($ModDirectory)"

# Make sure we have the latest assemblies
# only if the update flag is set
If ($Update)
{
    # Tell the user what is happening
    Write-Host "[CSM Update Script] You have specified the -Update flag. The script will now update the local assemblies to the installed version under Cities: Skylines."
    Write-Host "[CSM Update Script] Please Note: This may break the mod if any major game changes have occured."

    # Get the steam directory
    $SteamDirectory = Read-Host "[CSM Update Script] Please enter your steam folder directory (not steamapps). For example, 'C:\Program Files\Steam\'" 

    # Full folder path
    $AssemblyDirectory = $SteamDirectory.TrimEnd("\") + "\steamapps\common\Cities_Skylines\Cities_Data\Managed\"

    # Test to see if the path is valid
    $PathValid = Test-Path -Path $AssemblyDirectory
    If (!$PathValid)
    {
        Write-Host "[CSM Update Script] Path is invalid. Make sure Cities Skylines is installed and that the provided folder is correct"
        Return;
    }

    # Start copying the items
    Write-Host "[CSM Update Script] Copying assemblies..."

    # Recreate the assemblies folder
    Remove-Item "..\assemblies" -Recurse -ErrorAction Ignore
    New-Item -ItemType directory -Path "..\assemblies" | Out-Null

    Copy-Item -Path "$($AssemblyDirectory)Assembly-CSharp.dll"  -Destination "..\assemblies\Assembly-CSharp.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)ColossalManaged.dll"  -Destination "..\assemblies\ColossalManaged.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)ICities.dll"          -Destination "..\assemblies\ICities.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)UnityEngine.dll"      -Destination "..\assemblies\UnityEngine.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)UnityEngine.UI.dll"   -Destination "..\assemblies\UnityEngine.UI.dll" -Force

    Write-Host "[CSM Update Script] Done copying assemblies!"
}

# Build the project if the build
# flag is specified.
if ($Build)
{
    Write-Host "[CSM Build Script] You have specified the -Build flag. The script will auto detect MSBuild and build the mod."

    # Run MSBuild
    $msbuild = Find-MsBuild
    & $msbuild "..\CSM.sln" /restore /t:CSM /p:Configuration=Release /p:Platform="Any CPU" 
    Write-Host "[CSM Build Script] Build Complete!"
}

# Copy files if the install flag is specified
if ($Install)
{
    # Clear the directory
    Write-Host "[CSM Install Script] Clearing and creating mod directory."

    # Delete directory and contents
    Remove-Item $ModDirectory -Recurse -ErrorAction Ignore

    # Make sure the game mod directory exists
    New-Item -ItemType directory -Path $ModDirectory | Out-Null

    # Copy the required files
    Write-Host "[CSM Install Script] Copying required files..."
    Copy-Item -Path "$($OutputDirectory)\LiteNetLib.dll"        -Destination "$($ModDirectory)\LiteNetLib.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\protobuf-net.dll"      -Destination "$($ModDirectory)\protobuf-net.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\CSM.dll"               -Destination "$($ModDirectory)\CSM.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\Open.Nat.dll"          -Destination "$($ModDirectory)\Open.Nat.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\System.Threading.dll"  -Destination "$($ModDirectory)\System.Threading.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\0Harmony.dll"          -Destination "$($ModDirectory)\0Harmony.dll" -Force
    Copy-Item -Path "$($OutputDirectory)\NLog.dll"          	-Destination "$($ModDirectory)\NLog.dll" -Force

    # Done
    Write-Host "[CSM Install Script] Completed Copy"
}

Write-Host "[CSM Build Script] Done!"