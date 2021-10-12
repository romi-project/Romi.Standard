using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Romi.Standard.Tests
{
    public static class TestUtility
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
    }
}
