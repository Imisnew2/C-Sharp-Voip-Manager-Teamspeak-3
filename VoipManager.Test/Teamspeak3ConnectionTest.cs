/* ************************************************************************** *
 * Voip Manager.
 * Copyright (C) 2012-2013  Cameron Gunnin
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * ************************************************************************** */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VoipManager.Test
{
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using VoipManager.Connections;
    using VoipManager.Teamspeak3.Communication;

    [TestClass]
    public class TestTeamspeak3Connection
    {
        private Boolean?             IsConnected;
        private Teamspeak3Connection Connection;

        [TestInitialize]
        public void InitConnect()
        {
            Connection = new Teamspeak3Connection();
            Connection.Connected    += (c) => IsConnected = true;
            Connection.Disconnected += (c) => IsConnected = false;
            Assert.IsNull(IsConnected);
        }

        [TestMethod]
        public void TestConnect()
        {
            Connection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            Assert.IsTrue(IsConnected == true);
        }

        [TestMethod]
        public void TestConnectAsync()
        {
            Connection.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011)).Wait();
            Assert.IsTrue(IsConnected == true);
        }

        [TestMethod]
        public void TestRedundantConnect()
        {
            Connection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            Connection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            Connection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            TestConnect();
        }

        [TestMethod]
        public void TestRedundantConnectAsync()
        {
            Connection.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            Connection.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            Connection.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));
            TestConnectAsync();
        }

        [TestMethod]
        public void TestDisconnect()
        {
            Connection.Disconnect();
            Assert.IsNull(IsConnected);

            TestConnect();
            Connection.Disconnect();
            Assert.IsFalse(IsConnected == true);
        }

        [TestMethod]
        public void TestDisconnectAsync()
        {
            Connection.DisconnectAsync().Wait();
            Assert.IsNull(IsConnected);

            TestConnectAsync();
            Connection.DisconnectAsync().Wait();
            Assert.IsFalse(IsConnected == true);
        }

        [TestMethod]
        public void TestRedundantDisconnect()
        {
            TestConnect();
            Connection.Disconnect();
            Connection.Disconnect();
            Connection.Disconnect();
            Assert.IsFalse(IsConnected == true);
        }

        [TestMethod]
        public void TestRedundantDisconnectAsync()
        {
            TestConnectAsync();
            Connection.DisconnectAsync();
            Connection.DisconnectAsync();
            Connection.DisconnectAsync().Wait();
            Assert.IsFalse(IsConnected == true);
        }

        [TestMethod]
        public void TestSend()
        {
            var msg = Connection.Send(Teamspeak3Request.BuildWhoAmI());
            Assert.IsNull(msg);
            Assert.IsNull(IsConnected);

            TestConnect();
            msg = Connection.Send(Teamspeak3Request.BuildWhoAmI());
            Assert.IsTrue(IsConnected == true);
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.Id == 0);
            Assert.IsTrue(msg.Message == "ok");
            Assert.IsNotNull(msg.Section);
            Assert.IsNotNull(msg.Section.Group);
            Assert.IsTrue(msg.Section.Group.Keys.Count() > 0);
        }

        [TestMethod]
        public void TestSendAsync()
        {
            var msgTask = Connection.SendAsync(Teamspeak3Request.BuildWhoAmI());
            msgTask.Wait();
            var msg = msgTask.Result;
            Assert.IsNull(msg);
            Assert.IsNull(IsConnected);

            TestConnect();
            msgTask = Connection.SendAsync(Teamspeak3Request.BuildWhoAmI());
            msgTask.Wait();
            msg = msgTask.Result;
            Assert.IsTrue(IsConnected == true);
            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.Id == 0);
            Assert.IsTrue(msg.Message == "ok");
            Assert.IsNotNull(msg.Section);
            Assert.IsNotNull(msg.Section.Group);
            Assert.IsTrue(msg.Section.Group.Keys.Count() > 0);
        }

        [TestMethod]
        public void TestSendOrder()
        {
            TestConnect();
            var tRequests = new BlockingCollection<Teamspeak3Request>();
            var tMessages = new BlockingCollection<Teamspeak3Message>();
            Connection.SentRequest      += (c, r) => tRequests.Add(r);
            Connection.ReceivedResponse += (c, r) => tMessages.Add(r);

            var tSents = new List<Teamspeak3Request>();
            tSents.Add(Teamspeak3Request.BuildWhoAmI());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildWhoAmI());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildWhoAmI());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildWhoAmI());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildVersion());
            tSents.Add(Teamspeak3Request.BuildWhoAmI());
            tSents.Add(Teamspeak3Request.BuildWhoAmI());

            var tTasks = new List<Task<Teamspeak3Message>>();
            foreach (var sent in tSents) {
                tTasks.Add(Connection.SendAsync(sent));
            }

            var tRecvs = new List<Teamspeak3Message>();
            foreach (var task in tTasks) {
                task.Wait();
                tRecvs.Add(task.Result);
            }

            CollectionAssert.AreEquivalent(tSents, tRequests);
            CollectionAssert.AreEquivalent(tRecvs, tMessages);
        }

        [TestMethod]
        public void TestSendBeforeConnect()
        {
            var msgTask1 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask2 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask3 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            Assert.IsNull(IsConnected);

            TestConnect();
            Assert.IsNull(msgTask1.Result);
            Assert.IsNull(msgTask2.Result);
            Assert.IsNull(msgTask3.Result);
            Assert.IsTrue(IsConnected == true);

            msgTask1 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            msgTask2 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            msgTask3 = Connection.SendAsync(Teamspeak3Request.BuildVersion());

            Task.WaitAll(msgTask1, msgTask2, msgTask3);
            Assert.IsNotNull(msgTask1.Result);
            Assert.IsNotNull(msgTask2.Result);
            Assert.IsNotNull(msgTask3.Result);
        }

        [TestMethod]
        public void TestSendAfterDisconnect()
        {
            TestConnect();
            var msgTask1 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask2 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask3 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            Assert.IsTrue(IsConnected == true);

            Connection.Disconnect();
            Assert.IsNotNull(msgTask1.Result);
            Assert.IsNotNull(msgTask2.Result);
            Assert.IsNotNull(msgTask3.Result);

            msgTask1 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            msgTask2 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            msgTask3 = Connection.SendAsync(Teamspeak3Request.BuildVersion());

            Task.WaitAll(msgTask1, msgTask2, msgTask3);
            Assert.IsNull(msgTask1.Result);
            Assert.IsNull(msgTask2.Result);
            Assert.IsNull(msgTask3.Result);
        }

        [TestMethod]
        [ExpectedException(typeof (ObjectDisposedException))]
        public void TestDisposed()
        {
            TestSend();
            var msgTask1 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask2 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask3 = Connection.SendAsync(Teamspeak3Request.BuildVersion());
            var msgTask4 = Connection.SendAsync(Teamspeak3Request.BuildVersion());

            Connection.Dispose();
            Assert.IsNotNull(msgTask1.Result);
            Assert.IsNotNull(msgTask2.Result);
            Assert.IsNotNull(msgTask3.Result);
            Assert.IsNotNull(msgTask4.Result);

            TestConnect();
        }
    }
}
