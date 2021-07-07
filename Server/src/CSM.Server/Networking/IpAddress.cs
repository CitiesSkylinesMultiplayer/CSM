﻿using System;
using System.Net;
using System.Net.Sockets;

namespace CSM.Networking
{
    public static class IpAddress
    {
        private static string _localIp;
        private static string _externalIp;

        public static string GetLocalIpAddress()
        {
            if (_localIp != null)
            {
                return _localIp;
            }

            try
            {
                //Create a new socket
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    //Connect to 8.8.8.8 (Google IP)
                    socket.Connect("8.8.8.8", 65530);
                    //Get the IPEndPoint
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    //Get the IP Address (Internal) from the IPEndPoint
                    _localIp = endPoint.Address.ToString();
                    return _localIp;
                }
            }
            catch (Exception)
            {
                //On error return "Not found"
                return "Not found";
            }
        }

        public static string GetExternalIpAddress()
        {
            if (_externalIp != null)
            {
                return _externalIp;
            }

            try
            {
                //Get the External IP (IPv4) Address from internet
                _externalIp = new WebClient().DownloadString("http://api.ipify.org"); // HTTPS doesn't work
                return _externalIp;
            }
            catch (Exception)
            {
                //On error return "Not found"
                return "Not found";
            }
        }
    }
}
