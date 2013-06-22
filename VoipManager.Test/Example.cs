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

using System;
using System.Linq;
using System.Net;
using VoipManager.Connections;
using VoipManager.Teamspeak3.Communication;
using VoipManager.Teamspeak3.Objects;

namespace VoipManager
{
    class Example
    {
        static void Main()
        {
            // Uses the default settings:
            // - Timeout from trying to connect after 10 seconds.
            var tConnection = new Teamspeak3Connection();

            tConnection.Connected    += (c) => Console.WriteLine("[Status] Connected!");
            tConnection.Disconnected += (c) => Console.WriteLine("[Status] Disconnected!");
            tConnection.SentRequest      += (c, r) => Console.WriteLine("[Query] Request Sent: {0}",      r.Command);
            tConnection.ReceivedResponse += (c, r) => Console.WriteLine("[Query] Response Received: {0}", r.Id);
            tConnection.Notified += (c, n) => Console.WriteLine("[Notification] Seen {0}",   n.Event);
            tConnection.Banned   += (c, r) => Console.WriteLine("[Banned] Crap: {0} -- {1}", r.Message, r.ExtraMessage);

            // Attempt to connect to the server at 127.0.0.1:10011.
            tConnection.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10011));

            // Select the virtual server to use and login.
            tConnection.Send(Teamspeak3Request.BuildLogin("serveradmin", "password"));
            tConnection.Send(Teamspeak3Request.BuildUsePort(9987));

            // Request the client list from the server and parse the response into clients.
            var tClientMessage = tConnection.Send(Teamspeak3Request.BuildClientList());
            var tClients = tClientMessage.Sections
                .SelectMany(x => x.Groups)
                .Select(x => new Teamspeak3Client(x))
                .ToList();

            // Requeust the channel list from the server and parse the response into channels.
            var tChannelMessage = tConnection.Send(Teamspeak3Request.BuildClientList());
            var tChannels = tChannelMessage.Sections
                .SelectMany(x => x.Groups)
                .Select(x => new Teamspeak3Channel(x))
                .ToList();

            // Register for notifications when clients join/leave the server.
            var tNotificationRequest = new Teamspeak3Request("servernotifyregister");
            tNotificationRequest.AddParameter("event","server");
            tConnection.Send(tNotificationRequest);

            // Example request with error checking.
            var tResponse = tConnection.Send(Teamspeak3Request.BuildWhoAmI());
            if (tResponse == null) {
                // The connection is closed or had an error?
                Console.WriteLine("Connection Failure!");
            }
            else if (tResponse.Id == 3331 || tResponse.Id == 3329) {
                // Yarg, we've been banned due to spamming!
                Console.WriteLine("Banned: {0} -- {1}", tResponse.Message, tResponse.ExtraMessage);
            }
            else if (tResponse.Id != 0) {
                // Ahhhh, what went wrong?
                Console.WriteLine("Error: {0}", tResponse.Message);
            }

            tConnection.Send(Teamspeak3Request.BuildLogout());
            tConnection.Send(Teamspeak3Request.BuildQuit());
            tConnection.Send(Teamspeak3Request.BuildWhoAmI());

            Console.ReadKey();
            tConnection.Dispose();
        }
    }
}
