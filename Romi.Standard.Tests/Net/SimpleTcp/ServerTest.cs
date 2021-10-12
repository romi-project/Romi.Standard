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

        [Test]
        public void SingleClientTest()
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(_endPoint);
            Thread.Sleep(1000);
            Assert.AreEqual(1, _server.GetClientNum());
            tcpClient.Close();
            Thread.Sleep(1000);
            Assert.AreEqual(0, _server.GetClientNum());
            Assert.Pass();
        }

        [TearDown]
        public void CleanUp()
        {
            _server.Stop();
        }
    }














}
