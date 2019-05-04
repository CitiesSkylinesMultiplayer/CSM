#!/bin/bash
APP_SUPPORT_DIR=~/Library/Application\ Support
STEAM_DIR="$APP_SUPPORT_DIR"/Steam
ASSEMBLY_DIR="$STEAM_DIR"/steamapps/common/Cities_Skylines/Cities.app/Contents/Resources/Data/Managed/

# Whether you run this file from within the /scripts folder or the root Tango folder we will find the Root folder :)
SCRIPT_PATH="$( cd "$(dirname "$0")" ; pwd -P )"
ROOT_PATH="$(dirname $SCRIPT_PATH)"

# make assemblies folder if it doesn't exist yet. 
mkdir -p $ROOT_PATH/assemblies/

# clear assemblies folder if it has files in
rm -rf $ROOT_PATH/assemblies/*

# Copy build dependencies over so msbuild can locate from solution
cp "$ASSEMBLY_DIR"Assembly-CSharp.dll $ROOT_PATH/assemblies/
cp "$ASSEMBLY_DIR"ColossalManaged.dll $ROOT_PATH/assemblies/ 
cp "$ASSEMBLY_DIR"ICities.dll         $ROOT_PATH/assemblies/   
cp "$ASSEMBLY_DIR"UnityEngine.dll     $ROOT_PATH/assemblies/   
cp "$ASSEMBLY_DIR"UnityEngine.UI.dll  $ROOT_PATH/assemblies/

msbuild $ROOT_PATH/CSM.sln /restore /t:CSM /p:Configuration=Release /p:Platform='Any CPU' /p:OS=MacOS