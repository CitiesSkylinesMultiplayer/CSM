# Introduction
Write-Host "This script will update the internal mod assemblies to the latest version."
Write-Host "Sometimes updating these assemblies will break the mod."
Write-Host "Make sure Cities: Skylines is fully updated."
Write-Host "------------------------------------"

# Get the steam directory
$SteamDirectory = Read-Host "Please enter your steam folder directory" 

# Full folder path
$AssemblyDirectory = $SteamDirectory.TrimEnd("\") + "\steamapps\common\Cities_Skylines\Cities_Data\Managed\"

# Test to see if the path is valid
$PathValid = Test-Path -Path $AssemblyDirectory
If (!$PathValid)
{
    Write-Host "Path is invalid. Make sure Cities Skylines is installed and that the provided folder is correct"
    Return;
}

# Start copying the items
Write-Host "Copying assemblies..."

Copy-Item -Path "$($AssemblyDirectory)Assembly-CSharp.dll"  -Destination "..\assemblies\Assembly-CSharp.dll" -Force
Copy-Item -Path "$($AssemblyDirectory)ColossalManaged.dll"  -Destination "..\assemblies\ColossalManaged.dll" -Force
Copy-Item -Path "$($AssemblyDirectory)ICities.dll"          -Destination "..\assemblies\ICities.dll" -Force
Copy-Item -Path "$($AssemblyDirectory)UnityEngine.dll"      -Destination "..\assemblies\UnityEngine.dll" -Force
Copy-Item -Path "$($AssemblyDirectory)UnityEngine.UI.dll"   -Destination "..\assemblies\UnityEngine.UI.dll" -Force

Write-Host "Done copying assemblies!"