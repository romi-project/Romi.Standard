using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using Romi.Standard.Sockets.Net;

namespace Romi.Standard.Tests.Net
{
    public class ListenerTest
    {
        private SocketThread _socketThread;

        [SetUp]
        public void Setup()
        {
            var thread = new Thread(_socketThread = new SocketThread());
            thread.Start();
        }

        [Test]
        public void ListenAndBindTest()
        {
            var local = IPAddress.Loopback;
            var listener = new TestListener(local.AddressFamily, _socketThread);
            var port = SocketTestUtility.GetFreePort();
            try
            {
                listener.Bind(new IPEndPoint(local, port));
                listener.Listen();
                listener.Close();
                listener.Wait();
                Assert.AreEqual(listener.AcceptedCount, 0);
                Assert.AreEqual(listener.ClosedCount, 1);
                Assert.Pass();
            }
            finally
            {
                listener.Close();
            }
        }

        [Test]
        public void ListenAndAcceptTest()
        {
            var local = IPAddress.Loopback;
            var listener = new TestListener(local.AddressFamily, _socketThread);
            var connector = new TestConnector(local.AddressFamily, _socketThread);
            var port = SocketTestUtility.GetFreePort();
            try
            {
                var endPoint = new IPEndPoint(local, port);
                listener.Bind(endPoint);
                listener.Listen();
                var socket = connector.ConnectRaw(endPoint);
                Assert.IsNotNull(socket);
                listener.Wait();
                Assert.AreEqual(listener.AcceptedCount, 1);
                Assert.AreEqual(listener.ClosedCount, 0);
                listener.Close();
                listener.Wait();
                Assert.AreEqual(listener.ClosedCount, 1);
                Assert.AreEqual(connector.ConnectedCount, 1);
                connector.Close();
                Assert.Pass();
            }
            finally
            {
                listener.Close();
                connector.Close();
            }
        }

        [TearDown]
        public void CleanUp()
        {
            _socketThread.Stop();
        }
    }

    public class TestListener : Listener
    {
        private readonly ManualResetEventSlim _eventWaitHandle = new();
        public volatile int AcceptedCount = 0;
        public volatile int ClosedCount = 0;

        public TestListener(AddressFamily addressFamily, SocketThread socketThread)
            : base(addressFamily, socketThread)
        {
        }

        public override void OnAccept()
        {
            Interlocked.Increment(ref AcceptedCount);
            _eventWaitHandle.Set();
        }

        public override void OnClose()
        {
            Interlocked.Increment(ref ClosedCount);
            _eventWaitHandle.Set();
        }

        public void Wait()
        {
            _eventWaitHandle.Wait();
            _eventWaitHandle.Reset();
        }

        protected override void AcceptClient(Socket socket)
        {
            socket.LingerState = new LingerOption(true, 0);
            socket.NoDelay = true;
        }
    }
}
