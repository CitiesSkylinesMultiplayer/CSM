#!/bin/pwsh
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
    [string]$OutputDirectory = "..\src\csm\bin\Release",
    [string]$ModDirectory = "Default"
 )

# Functions
Function Find-MsBuild([int] $MaxVersion = 2019)
{
    $agent2019Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\msbuild.exe"
    $ent2019Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\msbuild.exe"
    $pro2019Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\msbuild.exe"
    $community2019Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe"

    $agent2017Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild.exe"
    $ent2017Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe"
    $pro2017Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\msbuild.exe"
    $community2017Path = "$Env:programfiles (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe"
    
    $fallback2015Path = "${Env:ProgramFiles(x86)}\MSBuild\14.0\Bin\MSBuild.exe"
    $fallback2013Path = "${Env:ProgramFiles(x86)}\MSBuild\12.0\Bin\MSBuild.exe"
    $fallbackPath = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
		
    If ((2019 -le $MaxVersion) -And (Test-Path $agent2019Path)) { return $agent2019Path } 
    If ((2019 -le $MaxVersion) -And (Test-Path $ent2019Path)) { return $ent2019Path } 
    If ((2019 -le $MaxVersion) -And (Test-Path $pro2019Path)) { return $pro2019Path } 
    If ((2019 -le $MaxVersion) -And (Test-Path $community2019Path)) { return $community2019Path }  

    If ((2017 -le $MaxVersion) -And (Test-Path $agent2017Path)) { return $agent2017Path } 
    If ((2017 -le $MaxVersion) -And (Test-Path $ent2017Path)) { return $ent2017Path } 
    If ((2017 -le $MaxVersion) -And (Test-Path $pro2017Path)) { return $pro2017Path } 
    If ((2017 -le $MaxVersion) -And (Test-Path $community2017Path)) { return $community2017Path } 

    If ((2015 -le $MaxVersion) -And (Test-Path $fallback2015Path)) { return $fallback2015Path } 
    If ((2013 -le $MaxVersion) -And (Test-Path $fallback2013Path)) { return $fallback2013Path } 
    If (Test-Path $fallbackPath) { return $fallbackPath } 
        
    throw "Yikes - Unable to find msbuild"
}

$Sep = "\"
If (($IsMacOS) -or ($IsLinux))
{
    $OutputDirectory = ($OutputDirectory).Replace("\", "/")
    $Sep = "/"
}

# Assume windows, since PowerShell 6 is the first with official cross-platform support
If ($PSVersionTable.PSVersion.Major -lt 6)
{
    $IsWindows = $true
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

# Introduction
Write-Host "[CSM Build Script] Welcome to the CSM build script. This script will now perform the specified actions.";
Write-Host "[CSM Build Script] Depending on your choice of options, you will need Visual Studio 2017/2019 (or the 'Build Tools for Visual Studio 2017/2019'), and Cities: Skylines installed."

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
    $GameDirectory = Read-Host "[CSM Update Script] Please enter your game folder directory. For example, for Windows, 'C:\Program Files\Steam\steamapps\common\Cities_Skylines' for Steam or 'C:\Program Files\Epic Games\CitiesSkylines' for Epic Games." 

    If ($IsMacOS)
    {
        # Full folder path
        $AssemblyDirectory = $GameDirectory.TrimEnd($Sep) + "$($Sep)Cities.App$($Sep)Contents$($Sep)Resources$($Sep)Data$($Sep)Managed$($Sep)"
    }
    Else
    {
        # Full folder path
        $AssemblyDirectory = $GameDirectory.TrimEnd($Sep) + "$($Sep)Cities_Data$($Sep)Managed$($Sep)"
    }

    # Test to see if the path is valid
    $PathValid = Test-Path -Path $AssemblyDirectory
    If (!$PathValid)
    {
        Write-Host "[CSM Update Script] Path is invalid. Make sure Cities Skylines is installed and that the provided folder is correct"
        Return;
    }

    # Start copying the items
    Write-Host "[CSM Update Script] Copying assemblies..."

    # Create assemblies folder if it doesn't exist
    New-Item -ItemType directory -Force -Path "..$($Sep)assemblies" | Out-Null

    Copy-Item -Path "$($AssemblyDirectory)Assembly-CSharp.dll"  -Destination "..$($Sep)assemblies$($Sep)Assembly-CSharp.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)ColossalManaged.dll"  -Destination "..$($Sep)assemblies$($Sep)ColossalManaged.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)ICities.dll"          -Destination "..$($Sep)assemblies$($Sep)ICities.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)UnityEngine.dll"      -Destination "..$($Sep)assemblies$($Sep)UnityEngine.dll" -Force
    Copy-Item -Path "$($AssemblyDirectory)UnityEngine.UI.dll"   -Destination "..$($Sep)assemblies$($Sep)UnityEngine.UI.dll" -Force

    Write-Host "[CSM Update Script] Done copying assemblies!"
}

# Build the project if the build
# flag is specified.
If ($Build)
{
    Write-Host "[CSM Build Script] You have specified the -Build flag. The script will auto detect MSBuild and build the mod."
    # Run MSBuild
    $msbuild = "msbuild"
    If ($IsWindows)
    {
        $msbuild = Find-MsBuild
        Write-Host "Using MSBuild at: $msbuild"
    }

    & $msbuild "..\CSM.sln" /restore /t:CSM_API /p:Configuration=Release /p:Platform="Any CPU" 
    & $msbuild "..\CSM.sln" /restore /t:CSM /p:Configuration=Release /p:Platform="Any CPU" 
    If ($LastExitCode -ne 0)
    {
        Write-Host "[CSM Build Script] Build failed!"
        exit $LastExitCode
    }
    Write-Host "[CSM Build Script] Build Complete!"
}

# Copy files if the install flag is specified
If ($Install)
{
    # Clear the directory
    Write-Host "[CSM Install Script] Clearing and creating mod directory."

    # Delete directory and contents
    Remove-Item $ModDirectory -Recurse -ErrorAction Ignore

    # Make sure the game mod directory exists
    If (!(Test-Path -Path $ModDirectory ))
    {
        New-Item -ItemType directory -Path $ModDirectory
    }

    # Copy the required files
    Write-Host "[CSM Install Script] Copying required files..."
    Copy-Item -Path "$($OutputDirectory)$($Sep)LiteNetLib.dll"        -Destination "$($ModDirectory)$($Sep)LiteNetLib.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)protobuf-net.dll"      -Destination "$($ModDirectory)$($Sep)protobuf-net.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)CSM.dll"               -Destination "$($ModDirectory)$($Sep)CSM.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)CSM.API.dll"           -Destination "$($ModDirectory)$($Sep)CSM.API.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)Open.Nat.dll"          -Destination "$($ModDirectory)$($Sep)Open.Nat.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)System.Threading.dll"  -Destination "$($ModDirectory)$($Sep)System.Threading.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)0Harmony.dll"          -Destination "$($ModDirectory)$($Sep)0Harmony.dll" -Force
    Copy-Item -Path "$($OutputDirectory)$($Sep)NLog.dll"          	  -Destination "$($ModDirectory)$($Sep)NLog.dll" -Force

    # Done
    Write-Host "[CSM Install Script] Completed Copy"
}

Write-Host "[CSM Build Script] Done!"
