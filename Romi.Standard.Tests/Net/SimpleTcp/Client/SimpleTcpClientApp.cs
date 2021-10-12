using System.Collections.Generic;
using System.Net;
using System.Threading;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Client
{
    public class SimpleTcpClientApp
    {
        private readonly List<SimpleTcpClient_Client> _clients = new();
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

        public void Stop()
        {
            foreach (var client in _clients.ToArray())
                client.Close();
            _socketThread.Stop();
            _socketThread.WaitForEnd();
        }

        public void AddClient(SimpleTcpClient_Client client)
        {
            _clients.Add(client);
        }

        public void RemoveClient(SimpleTcpClient_Client client)
        {
            _clients.Remove(client);
        }

        public int GetClientNum()
        {
            return _clients.Count;
        }
    }
}
