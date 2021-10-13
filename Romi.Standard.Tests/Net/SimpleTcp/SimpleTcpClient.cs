using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp
{
    public abstract class SimpleTcpClient : StandardTcpClient
    {
        private static volatile int _sn;
        public readonly int SocketId = Interlocked.Increment(ref _sn);

        protected SimpleTcpClient(Socket socket, SocketThread socketThread)
            : base(socket, socketThread)
        {
        }

        public virtual void SendPacket(byte[] content)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(content.Length); // header 4 bytes
            bw.Write(content);        // content
            SendRawPacket(ms.ToArray());
        }

        public override void OnConnect()
        {
            Console.WriteLine($"Connected with {RemoteAddress}");
        }

        public override void OnClose()
        {
            //if (CloseReason.Error)
                Console.WriteLine($"Disconnected with {RemoteAddress} / Reason: {CloseReason}");
        }
    }
}
