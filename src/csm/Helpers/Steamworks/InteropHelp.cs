// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2022 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

#define STEAMWORKS_LIN_OSX

#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

using System.Text;

namespace CSM.Helpers.Steamworks {
	public class InteropHelp {

		public static string ByteArrayToStringUTF8(byte[] buffer) {
			int length = 0;
			while (length < buffer.Length && buffer[length] != 0) {
				length++;
			}

			return Encoding.UTF8.GetString(buffer, 0, length);
		}

		public static void StringToByteArrayUTF8(string str, byte[] outArrayBuffer, int outArrayBufferSize)
		{
			outArrayBuffer = new byte[outArrayBufferSize];
			int length = Encoding.UTF8.GetBytes(str, 0, str.Length, outArrayBuffer, 0);
			outArrayBuffer[length] = 0;
		}
	}
}

#endif // !DISABLESTEAMWORKS
