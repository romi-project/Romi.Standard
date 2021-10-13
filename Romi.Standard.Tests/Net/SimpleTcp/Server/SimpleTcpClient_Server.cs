using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net.SimpleTcp.Server
{
    public class SimpleTcpClient_Server : SimpleTcpClient
    {
        public SimpleTcpClient_Server(Socket socket, SocketThread socketThread, SimpleTcpServerApp serverApp)
            : base(socket, socketThread)
        {
            ServerApp = serverApp;
        }

        public SimpleTcpServerApp ServerApp { get; }

        protected override void OnPacket(byte[] data)
        {
            //data.Print("SVR:R", this);
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);
            using var ws = new MemoryStream();
            using var bw = new BinaryWriter(ws);
            int op = br.ReadInt16();
            switch (op)
            {
                case 1:
                {
                    var bytes = br.ReadBytes(16);
                    bw.Write((short)0x01);
                    bw.Write(bytes);
                    ServerApp.BroadcastPacket(ws.ToArray());
                    break;
                }
                case 2:
                {
                    var equals = br.ReadBoolean();
                    if (equals)
                        Interlocked.Increment(ref ServerApp.TrueCount);
                    else
                        Interlocked.Increment(ref ServerApp.FalseCount);
                    break;
                }
            }
        }

        public override void OnConnect()
        {
            ServerApp.AddClient(this);
        }

        public override void OnClose()
        {
            ServerApp.RemoveClient(this);
        }

        public override void SendPacket(byte[] data)
        {
            //data.Print($"SVR:S", this);
            base.SendPacket(data);
        }
    }
}
