using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Client
{
    public class SimpleTcpClientApp
    {
        private readonly ConcurrentDictionary<int, SimpleTcpClient_Client> _clients = new();
        private readonly IPEndPoint _endPoint;
        private readonly SocketThread _socketThread;

        public SimpleTcpClientApp(IPEndPoint endPoint)
        {
            _socketThread = new SocketThread();
            _endPoint = endPoint;
        }

        public void Start()
        {
            var thread = new Thread(_socketThread);
            thread.Start();
        }

        public bool Connect()
        {
            var connector = new SimpleTcpConnector(_endPoint.AddressFamily, _socketThread, this);
            var socket = connector.ConnectRaw(_endPoint);
            return socket != null;
        }

        public async Task ConnectAsync()
        {
            for (var i = 0; i < 30; i++)
            {
                var connector = new SimpleTcpConnector(_endPoint.AddressFamily, _socketThread, this);
                var socket = await connector.ConnectRawAsync(_endPoint);
                if (socket != null)
                    break;
            }
        }

        public void Stop()
        {
            foreach (var client in _clients.Values.ToArray())
                client.Close();
            _socketThread.Stop();
            _socketThread.WaitForEnd();
        }

        public void AddClient(SimpleTcpClient_Client client)
        {
            _clients.TryAdd(client.SocketId, client);
        }

        public void RemoveClient(SimpleTcpClient_Client client)
        {
            _clients.TryRemove(client.SocketId, out _);
        }

        public int GetClientNum()
        {
            return _clients.Count;
        }
    }
}
