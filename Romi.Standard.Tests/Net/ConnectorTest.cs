using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net
{
    public class ConnectorTest
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
            Assert.AreEqual(1, conn.ClosedCount);
            Assert.Pass();
        }

        [Test]
        public void ConnectSuccessTest()
        {
            var local = IPAddress.Loopback;
            var conn = new TestConnector(local.AddressFamily, _socketThread);
            var port = SocketTestUtility.GetLocalListeningPort();
            var socket = conn.ConnectRaw(new IPEndPoint(local, port));
            Assert.IsNotNull(socket);
            Assert.AreEqual(0, conn.ClosedCount);
            Assert.AreEqual(1, conn.ConnectedCount);
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
            : base(addressFamily, socketThread)
        {
        }

        protected override void ConnectClient(Socket socket)
        {
            Interlocked.Increment(ref ConnectedCount);
            socket.NoDelay = true;
        }

        public override void OnClose()
        {
            Interlocked.Increment(ref ClosedCount);
        }
    }
}
