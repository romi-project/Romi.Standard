using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using Romi.Standard.Sockets.Net;
using Romi.Standard.Tests.Net.SimpleTcp.Client;
using Romi.Standard.Tests.Net.SimpleTcp.Server;

namespace Romi.Standard.Tests.Net.SimpleTcp
{
    public class ServerTest
    {
        private IPEndPoint _endPoint;
        private SimpleTcpServerApp _server;

        [SetUp]
        public void SetUp()
        {
            _endPoint = new IPEndPoint(IPAddress.Loopback, SocketTestUtility.GetFreePort());
            _server = new SimpleTcpServerApp(_endPoint);
            _server.Start();
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

        [Test, Timeout(30000)]
        public void VeryManyClientTest()
        {
            TestNumberClient(200);
            Assert.Pass();
        }

        private void TestNumberClient(int clientNumber)
        {
            var clients = new TcpClient[clientNumber];
            for (var i = 0; i < clients.Length; i++)
            {
                clients[i] = new TcpClient();
                clients[i].Connect(_endPoint);
            }
            while(clients.Length != _server.GetClientNum())
                Thread.Sleep(50);
            foreach (var t in clients)
            {
                t.Close();
            }
            while(0 != _server.GetClientNum())
                Thread.Sleep(50);
        }

        [TearDown]
        public void CleanUp()
        {
            _server.Stop();
        }
    }
}
