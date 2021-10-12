using System.Collections.Generic;
using System.Net;
using System.Threading;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Server
{
    public class SimpleTcpServerApp
    {
        private readonly List<SimpleTcpClient_Server> _clients = new();
        private readonly Listener _listener;
        private readonly IPEndPoint _endPoint;
        private readonly SocketThread _socketThread;
        public volatile int TrueCount = 0;
        public volatile int FalseCount = 0;

        public SimpleTcpServerApp(IPEndPoint endPoint)
        {
            _socketThread = new SocketThread();
            _endPoint = endPoint;
            _listener = new SimpleTcpListener(_endPoint.AddressFamily, _socketThread, this);
        }

        public void Start()
        {
            var thread = new Thread(_socketThread);
            thread.Start();
            _listener.Bind(_endPoint);
            _listener.Listen();
        }

        public void Stop()
        {
            _listener.Close();
            foreach (var client in _clients.ToArray())
                client.Close();
            _socketThread.Stop();
            _socketThread.WaitForEnd();
        }

        public void AddClient(SimpleTcpClient_Server client)
        {
            _clients.Add(client);
        }

        public void RemoveClient(SimpleTcpClient_Server client)
        {
            _clients.Remove(client);
        }

        public int GetClientNum()
        {
            return _clients.Count;
        }

        public void BroadcastPacket(byte[] content)
        {
            foreach (var client in _clients.ToArray())
                client.SendPacket(content);
        }
    }
}
