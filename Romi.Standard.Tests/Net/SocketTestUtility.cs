using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net
{
    public static class SocketTestUtility
    {
        private static readonly Random Random = new();
        private const int MaxTry = 200;

        public static int GetFreePort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = properties.GetActiveTcpListeners();
            var connections = properties.GetActiveTcpConnections();
            var usingPorts = new HashSet<int>();
            foreach (var i in listeners.Select(item => item.Port))
                usingPorts.Add(i);
            foreach (var i in connections.Select(item => item.LocalEndPoint.Port))
                usingPorts.Add(i);
            for (var i = 0; i < MaxTry; ++i)
            {
                var port = Random.Next(1025, 65535);
                if (!usingPorts.Contains(port))
                    return port;
            }
            throw new TimeoutException("Failed to get a free port in specified time.");
        }

        public static int GetLocalListeningPort()
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var listeners = properties.GetActiveTcpListeners();
            try
            {
                return listeners.First().Port;
            }
            catch
            {
                throw new NotSupportedException("None of listening TCP port to test.");
            }
        }

        public static byte[] MakeStartupPacket()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write((short)0x00);
            return ms.ToArray();
        }

        public static void Print(this byte[] data, string dir, Client client)
        {
            Console.WriteLine($"[{dir}] [{client.RemoteAddress}] {BitConverter.ToString(data).Replace('-', ' ')}");
        }
    }
}
