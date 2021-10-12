using System.Net.Sockets;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Server
{
    public class SimpleTcpListener : Listener
    {
        private readonly SimpleTcpServerApp _serverApp;
        private readonly SocketThread _socketThread;

        public SimpleTcpListener(AddressFamily addressFamily, SocketThread socketThread, SimpleTcpServerApp serverApp)
            : base(addressFamily, socketThread)
        {
            _socketThread = socketThread;
            _serverApp = serverApp;
        }

        protected override void AcceptClient(Socket socket)
        {
            var client = new SimpleTcpClient_Server(socket, _socketThread, _serverApp);
            client.Reserve(SocketEventType.Connect);
        }
    }
}
