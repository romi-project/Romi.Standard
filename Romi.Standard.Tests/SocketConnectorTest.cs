using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests
{
    public class SocketConnectorTest
    {
        private SocketThread _socketThread;

        [SetUp]
        public void Setup()
        {
            var thread = new Thread(_socketThread = new SocketThread());
            thread.Start();
        }

        [Test]
        public void ConnectFailTest()
        {
            var local = IPAddress.Loopback;
            var conn = new TestConnector(local.AddressFamily, _socketThread);
            var socket = conn.ConnectRaw(new IPEndPoint(local, 12));
            Assert.IsNull(socket);
            Assert.AreEqual(conn.ClosedCount, 1);
            Assert.Pass();
        }

        [Test]
        public void ConnectSuccessTest()
        {
            var local = IPAddress.Loopback;
            var conn = new TestConnector(local.AddressFamily, _socketThread);
            var port = TestUtility.GetLocalListeningPort();
            var socket = conn.ConnectRaw(new IPEndPoint(local, port));
            Assert.IsNotNull(socket);
            Assert.AreEqual(conn.ClosedCount, 0);
            Assert.AreEqual(conn.ConnectedCount, 1);
            socket.Close();
            Assert.Pass();
        }

        [TearDown]
        public void CleanUp()
        {
            _socketThread.Stop();
        }
    }

    public class TestConnector : Connector
    {
        public volatile int ConnectedCount = 0;
        public volatile int ClosedCount = 0;

        public TestConnector(AddressFamily addressFamily, SocketThread socketThread)
            : base(new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp), socketThread)
        {
        }

        protected override void ConnectClient(Socket socket)
        {
            Interlocked.Increment(ref ConnectedCount);
            socket.NoDelay = true;
        }

        public override void OnClose()
        {
            base.OnClose();
            Interlocked.Increment(ref ClosedCount);
        }
    }
}
