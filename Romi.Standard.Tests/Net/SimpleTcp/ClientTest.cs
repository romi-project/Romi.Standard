using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Romi.Standard.Sockets.Net;
using Romi.Standard.Tests.Net.SimpleTcp.Client;
using Romi.Standard.Tests.Net.SimpleTcp.Server;

namespace Romi.Standard.Tests.Net.SimpleTcp
{
    public class ClientTest
    {
        private IPEndPoint _endPoint;
        private SimpleTcpServerApp _server;
        private SimpleTcpClientApp _client;

        [SetUp]
        public void SetUp()
        {
            _endPoint = new IPEndPoint(IPAddress.Loopback, SocketTestUtility.GetFreePort());
            _server = new SimpleTcpServerApp(_endPoint);
            _server.Start();
            _client = new SimpleTcpClientApp(_endPoint);
            _client.Start();
        }

        [Test, Timeout(30000)]
        public void SingleClientTest()
        {
            TestNumberClient(1);
            Assert.Pass();
        }

        [Test, Timeout(30000)]
        public void MultiClientTest()
        {
            TestNumberClient(5);
            Assert.Pass();
        }

        [Test, Timeout(60000)]
        public void VeryManyClientTest()
        {
            TestNumberClient(200);
            Assert.Pass();
        }

        [Test, Timeout(60000)]
        public void VeryVeryManyClientTest()
        {
            TestNumberClient(500);
            Assert.Pass();
        }

        [Test, Timeout(30000)]
        public void VeryVeryVeryManyClientTest()
        {
            TestNumberClient(1000);
            Assert.Pass();
        }

        private IEnumerable<Task> ConnectionTasks(int clientCount)
        {
            for (var i = 0; i < clientCount; i++)
                yield return _client.ConnectAsync();
        }

        private void TestNumberClient(int clientCount)
        {
            Task.WhenAll(ConnectionTasks(clientCount));
            while (_server.GetClientNum() < clientCount)
                Thread.Sleep(50);
            Assert.AreEqual(clientCount, _server.GetClientNum());
            _server.BroadcastPacket(SocketTestUtility.MakeStartupPacket());
            while (_server.TrueCount != clientCount * 1)
                Thread.Sleep(50);
            while (_server.FalseCount != clientCount * (clientCount - 1))
                Thread.Sleep(50);
            Assert.Pass();
        }

        [TearDown]
        public void CleanUp()
        {
            _server.Stop();
            _client.Stop();
        }
    }
}
