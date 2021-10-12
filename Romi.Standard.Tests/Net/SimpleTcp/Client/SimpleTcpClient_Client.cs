using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Client
{
    public class SimpleTcpClient_Client : SimpleTcpClient
    {
        private static readonly Random Random = new Random();
        private byte[] _randomBytes;

        public SimpleTcpClient_Client(Socket socket, SocketThread socketThread, SimpleTcpClientApp clientApp)
            : base(socket, socketThread)
        {
            ClientApp = clientApp;
        }

        public SimpleTcpClientApp ClientApp { get; }

        public override void OnConnect()
        {
            base.OnConnect();
            ClientApp.AddClient(this);
        }

        public override void OnClose()
        {
            base.OnClose();
            ClientApp.RemoveClient(this);
        }

        protected override void OnPacket(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            using var ws = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            int op = br.ReadInt16();
            switch (op)
            {
                case 0:
                {
                    Random.NextBytes(_randomBytes = new byte[16]);
                    bw.Write((short)0x01);
                    bw.Write(_randomBytes);
                    SendPacket(ws.ToArray());
                    break;
                }
                case 1:
                {
                    var bytes = br.ReadBytes(16);
                    var equals = _randomBytes.SequenceEqual(bytes);
                    bw.Write((short)0x02);
                    bw.Write(equals);
                    SendPacket(ws.ToArray());
                    break;
                }
            }
        }
    }
}
