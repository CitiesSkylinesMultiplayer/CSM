// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

#define STEAMWORKS_LIN_OSX

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
	#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using System.Runtime.InteropServices;
using IntPtr = System.IntPtr;

namespace CSM.Helpers.Steamworks {

	//-----------------------------------------------------------------------------
	// Purpose: called when the user tries to join a game from their friends list
	//			rich presence will have been set with the "connect" key which is set here
	//-----------------------------------------------------------------------------
	[StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
	[CallbackIdentity(Constants.k_iSteamFriendsCallbacks + 37)]
	public struct GameRichPresenceJoinRequested_t
	{
		public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 37;

		public CSteamID
			m_steamIDFriend; // the friend they did the join via (will be invalid if not directly via a friend)

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_cchMaxRichPresenceValueLength)]
		private byte[] m_rgchConnect_;

		public string m_rgchConnect
		{
			get { return InteropHelp.ByteArrayToStringUTF8(m_rgchConnect_); }
			set { InteropHelp.StringToByteArrayUTF8(value, m_rgchConnect_, Constants.k_cchMaxRichPresenceValueLength); }
		}
	}
}

#endif // !DISABLESTEAMWORKS
