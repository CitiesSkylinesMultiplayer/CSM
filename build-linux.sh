#!/bin/bash

# Configuration
REAL_GAME_DIR="/var/mnt/schijven/1TB SSD/Games/Cities - Skylines/drive_c/Program Files (x86)/Cities.Skylines.v1.21.1.F5"
MANAGED_DIR="$REAL_GAME_DIR/Cities_Data/Managed"
MODS_DIR="$REAL_GAME_DIR/Files/Mods"
CSM_ASSEMBLIES="/home/king/CS1 Mods/CSM/assemblies"

# Harmony Assembly Paths
HARMONY_CORE="$MODS_DIR/Harmony 2.2.2-0/CitiesHarmony.Harmony.dll"
HARMONY_API="$MODS_DIR/81 Tiles 2 1.0.5/CitiesHarmony.API.dll"
PROTOBUF="$CSM_ASSEMBLIES/protobuf-net.dll"

echo "=== Building CSM (Linux) ==="

# Build command using dotnet msbuild properties to pass paths
dotnet build src/csm/CSM.csproj -c Release \
    -p:ManagedDir="$MANAGED_DIR" \
    -p:CsmDir="$CSM_ASSEMBLIES" \
    -p:HarmonyDll="$HARMONY_CORE" \
    -p:HarmonyApiDll="$HARMONY_API"

if [ $? -eq 0 ]; then
    echo "=== Build Successful! ==="
    
    # Optional: Install to game directory
    TARGET_MOD_DIR="$MODS_DIR/CSM_Built"
    echo "Installing to: $TARGET_MOD_DIR"
    mkdir -p "$TARGET_MOD_DIR"
    cp src/csm/bin/Release/net35/CSM.dll "$TARGET_MOD_DIR/"
    cp src/api/bin/Release/net35/CSM.API.dll "$TARGET_MOD_DIR/"
    cp src/basegame/bin/Release/net35/CSM.BaseGame.dll "$TARGET_MOD_DIR/"
    # Copy dependencies
    cp "$CSM_ASSEMBLIES"/*.dll "$TARGET_MOD_DIR/"
    
    echo "Done."
else
    echo "=== Build Failed! ==="
    exit 1
fi
