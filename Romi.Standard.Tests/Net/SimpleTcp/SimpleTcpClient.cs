using System.IO;
using System.Net.Sockets;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp
{
    public abstract class SimpleTcpClient : StandardTcpClient
    {
        protected SimpleTcpClient(Socket socket, SocketThread socketThread)
            : base(socket, socketThread)
        {
        }

        public void SendPacket(byte[] content)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(content.Length);
            bw.Write(content);
            SendRawPacket(ms.ToArray());
        }
    }
}
