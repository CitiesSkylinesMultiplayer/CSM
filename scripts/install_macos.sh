#!/bin/bash
APP_SUPPORT_DIR=~/Library/Application\ Support
STEAM_DIR="$APP_SUPPORT_DIR"/Steam
ASSEMBLY_DIR="$STEAM_DIR"/steamapps/common/Cities_Skylines/Cities.app/Contents/Resources/Data/Managed/
MOD_DIR="$APP_SUPPORT_DIR"/Colossal\ Order/Cities_Skylines/Addons/Mods/CSM

# Parse incoming arguments from stdin
PARAMS=""
while (( "$#" )); do
  case "$1" in
    -f|--flag-with-argument)
      FARG=$2
      shift 2
      ;;
    -o|--output_directory)
      BUILD_FOLDER=$2
      shift 2
      ;;
    --) # end argument parsing
      shift
      break
      ;;
    -*|--*=) # unsupported flags
      echo "Error: Unsupported flag $1" >&2
      exit 1
      ;;
    *) # preserve positional arguments
      PARAMS="$PARAMS $1"
      shift
      ;;
  esac
done
eval set -- "$PARAMS"

rm -rf "$MOD_DIR"/*

cp "$BUILD_FOLDER"/"LiteNetLib.dll"       "$MOD_DIR"
cp "$BUILD_FOLDER"/"protobuf-net.dll"     "$MOD_DIR"
cp "$BUILD_FOLDER"/"CSM.dll"              "$MOD_DIR"
cp "$BUILD_FOLDER"/"Open.Nat.dll"         "$MOD_DIR"
cp "$BUILD_FOLDER"/"System.Threading.dll" "$MOD_DIR"
cp "$BUILD_FOLDER"/"0Harmony.dll"         "$MOD_DIR"
cp "$BUILD_FOLDER"/"NLog.dll"         	  "$MOD_DIR" 