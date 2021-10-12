using System.Net.Sockets;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Client
{
    public class SimpleTcpConnector : Connector
    {
        private readonly SimpleTcpClientApp _clientApp;
        private readonly SocketThread _socketThread;

        public SimpleTcpConnector(AddressFamily addressFamily, SocketThread socketThread, SimpleTcpClientApp clientApp)
            : base(addressFamily, socketThread)
        {
            _socketThread = socketThread;
            _clientApp = clientApp;
        }

        protected override void ConnectClient(Socket socket)
        {
            var client = new SimpleTcpClient_Client(socket, _socketThread, _clientApp);
            client.Reserve(SocketEventType.Connect);
        }
    }
}
