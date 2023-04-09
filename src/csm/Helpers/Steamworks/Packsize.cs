// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

#define STEAMWORKS_LIN_OSX

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

// If we're running in the Unity Editor we need the editors platform.
#if UNITY_EDITOR_WIN
	#define VALVE_CALLBACK_PACK_LARGE
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
	#define VALVE_CALLBACK_PACK_SMALL

// Otherwise we want the target platform.
#elif UNITY_STANDALONE_WIN || STEAMWORKS_WIN
	#define VALVE_CALLBACK_PACK_LARGE
#elif UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_LIN_OSX
	#define VALVE_CALLBACK_PACK_SMALL

// We do not want to throw a warning when we're building in Unity but for an unsupported platform. So we'll silently let this slip by.
// It would be nice if Unity itself would define 'UNITY' or something like that...
#elif UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5 || UNITY_2017_1_OR_NEWER
	#define VALVE_CALLBACK_PACK_SMALL

// But we do want to be explicit on the Standalone build for XNA/Monogame.
#else
	#define VALVE_CALLBACK_PACK_LARGE
	#warning You need to define STEAMWORKS_WIN, or STEAMWORKS_LIN_OSX. Refer to the readme for more details.
#endif

namespace CSM.Helpers.Steamworks {
	public static class Packsize {
#if VALVE_CALLBACK_PACK_LARGE
		public const int value = 8;
#elif VALVE_CALLBACK_PACK_SMALL
		public const int value = 4;
#endif
	}
}

#endif // !DISABLESTEAMWORKS
