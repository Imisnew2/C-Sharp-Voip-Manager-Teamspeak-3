/* ************************************************************************** *
 * Teamspeak 3 - Server Query API.
 * Copyright (C) 2012  Cameron Gunnin
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
using System.Net.Sockets;
using Teamspeak.ServerQuery;
using Teamspeak.ServerQuery.Data;
using Teamspeak.ServerQuery.Objects;

namespace Teamspeak.Main
{
    // An example of how to use the Teamspeak 3 - Server Query API.
    public class Driver
    {
        static void Main(String[] args)
        {
            // ---------------------------- Setup -----------------------------
            // We pass in the hostname/ip and port when creating the Teamspeak
            // Connection object. An exception is thrown if the hostname could
            // not be resolved.
            TeamspeakMessage    tMessage    = null;
            TeamspeakConnection tConnection = null;
            try {
                tConnection = new TeamspeakConnection("localhost", 10011);
            }
            catch (SocketException) {
                // Oh no! Hostname could not be resolved.
                System.Environment.Exit(2);
            }



            // ---------------------------- Events ----------------------------
            // Only four events get fired as of yet. They are fired when a
            // query is sent, a response is received, if we are been banned, or
            // if a notification is received.
            tConnection.QuerySent            += (c, q)    => { Console.WriteLine("[QuerySent]       Command={0}", q.Command); };
            tConnection.MessageReceived      += (c, q, r) => { Console.WriteLine("[MessageReceived] Command={0}, Id={1}, Msg={2}", q.Command, r.Id, r.Message); };
            tConnection.NotificationReceived += (c, n)    => { Console.WriteLine("[Notification]    Event={0}", n.Event); };
            tConnection.Banned               += (c, r)    => { Console.WriteLine("[Banned]          Id={0}, Msg={1}, Ext={2}", r.Id, r.Message, r.ExtraMessage); };



            // ----------------- Synchronous and Asynchronous -----------------
            // Depending on whether we want to continue after we've sent a
            // command or wait for the response, we can use either Async or
            // Sync respectively. Static classes contained in the
            // TeamspeakConnection namespace, Async and Sync can be used
            // interchangably as synchronous or asynchronous operations are
            // desired.
            //
            // For example, I can open the connection asynchronously, which
            // will allow us to proceed without waiting for the connection to
            // finish. I can then fire off 3-4 commands whose responses don't
            // matter at the moment (they could be getting handled via events).
            //
            // Then, I can send a version command, which will block until the
            // connection has finished establishing itself and the query has
            // been responded to. This allows us to only wait when necessary
            // and queue up other commands as desired.

            // --- Connecting asynchronously. ---
            Console.WriteLine("[Connect]         Connecting to the server.");
            TeamspeakConnection.Async.Connect(tConnection);
            TeamspeakConnection.Async.Send(tConnection, TeamspeakQuery.BuildLogin("serveradmin", "zjThAVrP"));
            TeamspeakConnection.Async.Send(tConnection, TeamspeakQuery.BuildUsePort(9987));
            
            // --- Sending off some commands that aren't needed right away. ---
            Console.WriteLine("[Commands]        Asynchronously sending some commands.");
            TeamspeakConnection.Async.Send(tConnection, TeamspeakQuery.BuildBanList());
            TeamspeakConnection.Async.Send(tConnection, TeamspeakQuery.BuildClientList());
            TeamspeakConnection.Async.Send(tConnection, TeamspeakQuery.BuildChannelList());

            // -- But I really need to know what version I'm running so I can do some stuff. ---
            Console.WriteLine("[Version]         Retreiving the version of the server.");
            tMessage = TeamspeakConnection.Sync.Send(tConnection, TeamspeakQuery.BuildVersion());
            if (tMessage != null && tMessage.Id == "0")
                Console.WriteLine("[Version]         Version={0}, Build={1}, Platform={2}",
                    tMessage.Section.Group["version"],
                    tMessage.Section.Group["build"],
                    tMessage.Section.Group["platform"]);
            Console.WriteLine("\nPress Any Key to Continue\n");
            Console.ReadKey(true);



            // ------------------------ Notifications -------------------------
            // The Teamspeak 3 server query also supports notifications,
            // although you have to register for them. Once registered, they
            // will fire the NotificationReceived event. This is how you would
            // register for notifications involving the server (which includes
            // players joining/leaving).
            TeamspeakQuery tQuery = new TeamspeakQuery("servernotifyregister");
            tQuery.AddParameter("event", "server");
            TeamspeakConnection.Sync.Send(tConnection, tQuery);
            Console.WriteLine("\nPress Any Key to Continue\n");
            Console.ReadKey(true);



            // ---------------------- Response Handling -----------------------
            // Teamspeak 3 server responses can be logically separated into 3
            // parts: A section, a group, and a key-value pair. Each response,
            // depending on the command it came from, can have from 0 to *
            // sections, groups, and key-value pairs.
            // 
            // Each response also has an error id, message, and, depending on
            // the command it came from, an extra message. These used to tell
            // if a problem occurred on the server when processing the command.
            // For example, if you tried to move a client who was not present
            // on the server, you would get "Invalid Client Id" as a response.
            //
            // Assuming your command executed without any problems, the
            // response is broken up into the logical sections for you.
            // Depending on how you want to interpret the response, simply pass
            // the group (the smallest part of the response) to the constructor
            // of a Teamspeak Object.

            // --- We're trying to find a client named "Imisnew2" on the server. ---
            Console.WriteLine("[FindClient]      Searching for a specific client.");
            tMessage = TeamspeakConnection.Sync.Send(tConnection, TeamspeakQuery.BuildClientFind("Imisnew2"));
            if (tMessage != null && tMessage.Id == "0") {

                // --- Print out how many clients we found. The ClientFind finds uses a pattern instead of exact matching. ---
                Console.WriteLine("[FindClient]      Found {0} clients who matched the pattern.", tMessage.Sections.Sum(x => x.Groups.Count));

                // --- Create a TeamspeakClient object for each group of our response and print out the info. ---
                foreach (TeamspeakSection tSection in tMessage.Sections)
                    foreach (TeamspeakGroup tGroup in tSection.Groups) {
                        TeamspeakClient tClient = new TeamspeakClient(tGroup);
                        Console.WriteLine("[FindClient]      Client: Name={0}, Id={1}.", tClient.Basic.Name, tClient.Basic.Id);
                    }
            }
            Console.WriteLine("\nPress Any Key to Continue\n");
            Console.ReadKey(true);

            // --- Here, we're simply building a list of all the virtual servers running. ---
            Console.WriteLine("[ServerList]      Searching for virtual servers.");
            tMessage = TeamspeakConnection.Sync.Send(tConnection, TeamspeakQuery.BuildServerList());
            if (tMessage != null && tMessage.Id == "0") {

                // --- Print out how many virtual servers we found. ---
                Console.WriteLine("[ServerList]      Found {0} virtual servers.", tMessage.Sections.Sum(x => x.Groups.Count));

                // --- Create a TeamspeakServer object for each group of our response and print out the info. ---
                foreach (TeamspeakSection tSection in tMessage.Sections)
                    foreach (TeamspeakGroup tGroup in tSection.Groups) {
                        TeamspeakServer tServer = new TeamspeakServer(tGroup);
                        Console.WriteLine("[ServerList]      Channel: Name={0}, Clients Online={1}.", tServer.Normal.Name, tServer.Normal.ClientsOnline);
                    }
            }
            Console.WriteLine("\nPress Any Key to Continue\n");
            Console.ReadKey(true);



            // -------------------- Closing the Connection --------------------
            // When closing the connection, we don't need to logout and quit
            // manually. The TeamspeakConnection will automatically handle this
            // for us and send the appropriate commands to correctly shutdown
            // the connection. Also, it is possible to immediately reopen the
            // connection if necessary.
            Console.WriteLine("[Disconnect]      Closing the connection.");
            TeamspeakConnection.Sync.Send(tConnection, TeamspeakQuery.BuildLogout());
            TeamspeakConnection.Sync.Disconnect(tConnection);



            // --- Done! ---
            Console.WriteLine();
            Console.WriteLine("Press Any Key to Close");
            Console.ReadKey(true);

            /* Possible Sample Output.
             * [Connect]         Connecting to the server.
             * [Commands]        Asynchronously sending some commands.
             * [Version]         Retreiving the version of the server.
             * [QuerySent]       Command=login
             * [MessageReceived] Command=login, Id=0, Msg=ok
             * [QuerySent]       Command=use
             * [MessageReceived] Command=use, Id=0, Msg=ok
             * [QuerySent]       Command=banlist
             * [MessageReceived] Command=banlist, Id=1281, Msg=database empty result set
             * [QuerySent]       Command=clientlist
             * [MessageReceived] Command=clientlist, Id=0, Msg=ok
             * [QuerySent]       Command=channellist
             * [MessageReceived] Command=channellist, Id=0, Msg=ok
             * [QuerySent]       Command=version
             * [MessageReceived] Command=version, Id=0, Msg=ok
             * [Version]         Version=3.0.6.1, Build=1340956745, Platform=Windows
             * 
             * Press Any Key to Continue
             * 
             * [QuerySent]       Command=servernotifyregister
             * [MessageReceived] Command=servernotifyregister, Id=0, Msg=ok
             * 
             * Press Any Key to Continue
             * 
             * [FindClient]      Searching for a specific client.
             * [QuerySent]       Command=clientfind
             * [MessageReceived] Command=clientfind, Id=0, Msg=ok
             * [FindClient]      Found 1 clients who matched the pattern.
             * [FindClient]      Client: Name=Imisnew2, Id=1.
             * 
             * Press Any Key to Continue
             * 
             * [ServerList]      Searching for virtual servers.
             * [QuerySent]       Command=serverlist
             * [MessageReceived] Command=serverlist, Id=0, Msg=ok
             * [ServerList]      Found 1 virtual servers.
             * [ServerList]      Channel: Name=TeamSpeak ]I[ Server, Clients Online=2.
             * 
             * Press Any Key to Continue
             * 
             * [Disconnect]      Closing the connection.
             * [QuerySent]       Command=logout
             * [MessageReceived] Command=logout, Id=0, Msg=ok
             * [QuerySent]       Command=quit
             * [MessageReceived] Command=quit, Id=0, Msg=ok
             * 
             * Press Any Key to Close
             */
        }
    }
}
