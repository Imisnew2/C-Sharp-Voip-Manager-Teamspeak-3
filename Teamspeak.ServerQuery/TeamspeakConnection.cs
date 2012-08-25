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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Teamspeak.ServerQuery.Data;
using Teamspeak.ServerQuery.Utils;

namespace Teamspeak.ServerQuery
{
    public class TeamspeakConnection
    {
        #region Properties



        /// <summary>
        ///    Whether a connection to a Teamspeak 3 server is established.
        /// </summary>
        public Boolean IsConnected {
            get         { lock (mLock) return mIsSending && mIsReceiving; }
            private set { lock (mLock) mIsSending = mIsReceiving = value; }
        }
        /// <summary>
        ///    Whether the connection is logged in to a server.
        /// </summary>
        public Boolean IsLoggedIn {
            get;
            private set;
        }

        /// <summary>
        ///    The ip and port the connection is using to send information.
        /// </summary>
        public IPEndPoint LocalAddress {
            get {
                lock (mLock) {
                    if (mSocket != null)
                        return (IPEndPoint)mSocket.LocalEndPoint;
                    return null;
                }
        } }
        /// <summary>
        ///    The ip and port the connection is receiving information from.
        /// </summary>
        public IPEndPoint RemoteAddress {
            get {
                lock (mLock) {
                    if (mSocket != null)
                        return (IPEndPoint)mSocket.RemoteEndPoint;
                    return null;
                }
            }
        }



        #endregion
        #region Variables



        // mLock:
        //   Makes sure threads do not access the socket or information related
        //   to connecting while the socket is being created or destroyed.
        private Object mLock = new Object();
        
        // mSocket:
        //   The socket used to connect to the Teamspeak 3 server with.
        // mRecvTask:
        //   The task associated with receiving data from the server (loops until connection is broken).
        // mIsSending:
        //   Whether sending is allowed to occur (false if connection was closed).
        // mIsReceiving:
        //   Whether receiving is allowed to occur (false if connection was closed).
        private Socket  mSocket;
        private Task    mRecvTask;
        private Boolean mIsSending;
        private Boolean mIsReceiving;

        // mTextBuffer:
        //   The buffer that contains the text representation of the information received from the server.
        // mByteBuffer:
        //   The buffer that contains the byte representation of the information received from the server.
        private String  mTextBuffer;
        private Byte[]  mByteBuffer;
        
        // mServer:
        //   Contains host and port information of a Teamspeak 3 server used to
        //   connect to a Teamspeak 3 server.
        // mLogin:
        //   Contains a username and password used to authenticate with on a
        //   Teamspeak 3 server.
        // mUse:
        //   Contains either a virtual server port or virtual server id used to
        //   select a virtual server to send commands to on a Teamspeak 3
        //   server.
        private IPEndPoint     mServer;
        private TeamspeakQuery mLogin;
        private TeamspeakQuery mUse;

        // mQueries:
        //   The queue that holds the queries that have been sent but have not been responded to yet.
        // mMessages:
        //   The queue that holds the messages of queries that have been responded to.
        private BlockingQueue<TeamspeakQuery>   mQueries  = new BlockingQueue<TeamspeakQuery>();
        private BlockingQueue<TeamspeakMessage> mMessages = new BlockingQueue<TeamspeakMessage>();

        // mTasks:
        //   Makes sure that all tasks are run in the order they are specified.
        private BlockingQueue<Task> mTasks = new BlockingQueue<Task>();



        #endregion
        #region Constants



        // mGrtHeaderRegex:
        //   The regex that matches the greeting header that is sent from a Teamspeak 3 server.
        // mGrtMessageRegex:
        //   The regex that matches the greeting message that is sent from a Teamspeak 3 server.
        // mBannedRegex:
        //   The regex that matches banned responses from a Teamspeak 3 server.
        // mMessageRegex:
        //   The regex that matches message responses from a Teamspeak 3 server.
        // mNotificationRegex:
        //   The regex that matches notification responses from a Teamspeak 3 server.
        private static readonly Regex  mGrtHeaderRegex    = new Regex(String.Format("^TS3{0}", TeamspeakMessage.SeperatorRegex));
        private static readonly Regex  mGrtMessageRegex   = new Regex(String.Format("^Welcome to the TeamSpeak 3 ServerQuery interface, type \\\"help\\\" for a list of commands and \\\"help <command>\\\" for information on a specific command.{0}", TeamspeakMessage.SeperatorRegex));
        private static readonly Regex  mBannedRegex       = new Regex(String.Format("^(error id=(3331|3329).*?)(\\\\n\\\\r|)$"));
        private static readonly Regex  mMessageRegex      = new Regex(String.Format("^((.+?{0})*?)error id=.+?{0}", TeamspeakMessage.SeperatorRegex));
        private static readonly Regex  mNotificationRegex = new Regex(String.Format("^notify.+?{0}",                TeamspeakMessage.SeperatorRegex));



        #endregion

        /// <summary>
        ///    Initializes a new instance of the TeamspeakConnection class, starting up
        ///    the task manager.
        /// </summary>
        public TeamspeakConnection()
        {
            ThreadPool.QueueUserWorkItem((x) => {
                Task tTask = null;
                while (mTasks.TryDequeue(out tTask))
                    tTask.RunSynchronously();
            });
        }
        /// <summary>
        ///    Makes sure the connection gets closed and the thread we're using for the
        ///    task manager gets shut down when the object is dereferenced.
        /// </summary>
        ~TeamspeakConnection()
        {
            lock (mLock)
                if (IsConnected)
                    DisconnectTask().Wait();
            mTasks.Blocking = false;
        }

        /// <summary>
        ///    Sets the server information used when connecting to a Teamspeak 3
        ///    server.
        /// </summary>
        /// 
        /// <param name="host">
        ///    The hostname or IPv4 address of a Teamspeak 3 Server.
        /// </param>
        /// <param name="port">
        ///    The query port for the specified host.
        /// </param>
        public void SetServerInfo(String host, UInt16 port = 10011)
        {
            lock (mLock) {
                mServer = null;

                // Invalid values = set server info to null.
                if (!String.IsNullOrWhiteSpace(host)) {
                    
                    // Attempt to parse out the IPv4 address.
                    IPAddress tIPAddress = IPAddress.None;
                    if (!IPAddress.TryParse(host, out tIPAddress)) {

                        // Attempt to find the address using the host name.
                        IPAddress[] tAddresses = Dns.GetHostAddresses(host);
                        foreach (IPAddress tAddress in tAddresses)
                            if (tAddress.AddressFamily == AddressFamily.InterNetwork) {
                                tIPAddress = tAddress;
                                break;
                            }
                    }

                    // Set the server information with the IP address.
                    if (tIPAddress != null)
                        mServer = new IPEndPoint(tIPAddress, port);
                }
            }
        }
        /// <summary>
        ///    Sets the login information used when connecting to a Teamspeak 3 server.
        /// </summary>
        /// 
        /// <param name="user">
        ///    The username used to authenticate with.
        /// </param>
        /// <param name="pass">
        ///    The password for the specified username.
        /// </param>
        public void SetLoginInfo(String user, String pass)
        {
            lock (mLock) {
                mLogin = null;

                // Invalid values = set login info to null.
                if (!String.IsNullOrWhiteSpace(user) && !String.IsNullOrWhiteSpace(pass))
                    mLogin = TeamspeakQuery.BuildLogin(user, pass);
            }
        }
        /// <summary>
        ///    Sets the virtual server information used when connecting to a Teamspeak
        ///    3 server.
        /// </summary>
        /// 
        /// <param name="type">
        ///    Whether to select the virtual server using the virtual server id or the
        ///    virtual server port.
        /// </param>
        /// <param name="value">
        ///    If type was UseType.Id, the ID of the virtual server; otherwise the port
        ///    of the virtual server.
        /// </param>
        public void SetUseInfo(TeamspeakQuery.UseType type, Int32? value)
        {
            lock (mLock) {
                mUse = null;

                // Invalid values = set use info to null.
                if (value.HasValue) {
                    if (type == TeamspeakQuery.UseType.Id)
                        mUse =  TeamspeakQuery.BuildUseId(value.Value);
                    if (type == TeamspeakQuery.UseType.Port)
                        mUse =  TeamspeakQuery.BuildUsePort(value.Value);
                }
            }
        }

        #region Protected Methods



        /// <summary>
        ///    Enqueues a task to connect to a Teamspeak 3 server and begin sending and
        ///    receiving information.
        /// </summary>
        /// 
        /// <returns>
        ///    The connect task that was just scheduled.
        /// </returns>
        protected Task ConnectTask()
        {
            Task tTask = new Task(() => { ConnectMethod(); });
            mTasks.Enqueue(tTask);
            return tTask;
        }
        /// <summary>
        ///    Enqueues a task to disconnect from a Teamspeak 3 server and stop sending
        ///    and receiving information.
        /// </summary>
        /// 
        /// <returns>
        ///    The disconnect task that was just scheduled.
        /// </returns>
        protected Task DisconnectTask()
        {
            Task tTask = new Task(() => { DisconnectMethod(); });
            mTasks.Enqueue(tTask);
            return tTask;
        }
        /// <summary>
        ///    Enqueues a task to send a query to a Teamspeak 3 server, then waiting
        ///    until the response is received before returning.
        /// </summary>
        /// 
        /// <param name="query">
        ///    The query to be enqueued to be sent.
        /// </param>
        /// 
        /// <returns>
        ///    The send task that was just scheduled.
        /// </returns>
        protected Task<TeamspeakMessage> SendTask(TeamspeakQuery query)
        {
            Task<TeamspeakMessage> tTask = new Task<TeamspeakMessage>(() => { return SendMethod(query); });
            mTasks.Enqueue(tTask);
            return tTask;
        }



        #endregion
        #region Private Methods


        
        /// <summary>
        ///    Establishes a connection to a Teamspeak 3 server, checks the headers to
        ///    make sure it's a Teamspeak 3 server, then begins sending and receiving
        ///    information.
        /// </summary>
        private void ConnectMethod()
        {
            // Wait on the last connection to finish up.
            if (!IsConnected && mRecvTask != null)
                mRecvTask.Wait();

            // Protect the integrity of IsConnected.
            lock (mLock) {
                if (!IsConnected && mServer != null)
                    try {
                        // Create the socket and connect to the server.
                        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        mSocket.ExclusiveAddressUse = true;
                        mSocket.LingerState         = new LingerOption(false, 0);
                        mSocket.NoDelay             = true;
                        mSocket.SendBufferSize      = 8192;
                        mSocket.ReceiveBufferSize   = 8192;
                        mSocket.Ttl                 = 32;
                        mSocket.Connect(mServer);
                        IsConnected = true;

                        // Initialize the receiving variables.
                        mTextBuffer = String.Empty;
                        mByteBuffer = new Byte[mSocket.ReceiveBufferSize];

                        // Check to see if this is a valid server.
                        if (IsConnected = ReceiveGreetingHeader() && ReceiveGreetingMessage()) {
                        
                            // If we weren't banned, begin receiving information.
                            mRecvTask = Task.Factory.StartNew(() => {
                                while (mIsReceiving) {
                                    ReceiveData();
                                    ParseData();
                                }

                                // Clean up the socket information.
                                mQueries.Blocking  = false;
                                mMessages.Blocking = false;
                                lock (mLock) {
                                    mQueries  = new BlockingQueue<TeamspeakQuery>();
                                    mMessages = new BlockingQueue<TeamspeakMessage>();
                                    mSocket.Close();
                                }
                            });

                            // Handle the first queries manually.
                            if (mLogin != null) SendMethod(mLogin);
                            if (mUse   != null) SendMethod(mUse);
                        } else { mSocket.Close(); }
                    } catch (SocketException) { mSocket.Close(); }
            }
        }
        /// <summary>
        ///    Disconnects a connection from a Teamspeak 3 server by sending the
        ///    appropriate commands rather than haphazardly closing the socket.
        ///    Waits until the associated threads are shut down as well.
        /// </summary>
        private void DisconnectMethod()
        {
            // Protect the integrity of IsConnected.
            lock (mLock) {
                if (IsConnected) {

                    // Send the queries to logout and quit.
                    if (IsLoggedIn)
                        SendMethod(TeamspeakQuery.BuildLogout());
                    SendMethod(TeamspeakQuery.BuildQuit());
                }
            }

            // Wait on this connection to finish up.
            if (mRecvTask != null)
                mRecvTask.Wait();
        }
        /// <summary>
        ///    Sends a query to a Teamspeak 3 server then waits for the response.
        /// </summary>
        /// 
        /// <param name="query">
        ///    The query to send to the Teamspeak 3 server.
        /// </param>
        /// 
        /// <returns>
        ///    The response received from the Teamspeak 3 server.
        /// </returns>
        private TeamspeakMessage SendMethod(TeamspeakQuery query)
        {
            TeamspeakMessage tMssg = null;
            
            lock (mLock)
                if (IsConnected) {
                    Byte[]      tBuffer = Encoding.UTF8.GetBytes(query.ToString()); ;
                    Int32       tSize   = 0;
                    SocketError tError  = SocketError.Success;

                    // Attempt to send the information.
                    tSize = mSocket.Send(tBuffer, 0, tBuffer.Length, SocketFlags.None, out tError);
                    while (tSize < tBuffer.Length && tError == SocketError.Success)
                        tSize += mSocket.Send(tBuffer, tSize, tBuffer.Length - tSize, SocketFlags.None, out tError);

                    // Fire the event and return the response if the sending was successful.
                    if (tError == SocketError.Success) {
                        OnQuerySent(query);
                        mQueries.Enqueue(query);

                        // Attempt to receive the response.
                        mMessages.TryDequeue(out tMssg);
                    }
                    // Otherwise, stop sending things
                    else mIsSending = false;
                }

            return tMssg;
        }

        /// <summary>
        ///    Attempts to receive some data from the server that was connected to.
        ///    Then, check if the response matches up with the greeting header of a
        ///    Teamspeak 3 server.
        /// </summary>
        /// 
        /// <returns>
        ///    true if the response matched the Teamspeak 3 greeting header;
        ///    otherwise, false.
        /// </returns>
        private Boolean ReceiveGreetingHeader()
        {
            Match tMatch = Match.Empty;

            // Allow ourselves to time-out, just in case.
            mSocket.ReceiveTimeout = 10000;

            // Check to see if the greeting header we received is valid.
            if (mTextBuffer.Length != 0 || ReceiveData()) {
                tMatch = mGrtHeaderRegex.Match(mTextBuffer);
                mTextBuffer = mTextBuffer.Remove(0, tMatch.Length);
            }

            // Set our timeout back to normal.
            mSocket.ReceiveTimeout = 0;

            return tMatch.Success;
        }
        /// <summary>
        ///    Attempts to receive some data from the server that was connected to.
        ///    Then, check if the response matches up with the greeting message of a
        ///    Teamspeak 3 server.
        /// </summary>
        /// 
        /// <returns>
        ///    true if the response matched the Teamspeak 3 greeting message;
        ///    otherwise, false.
        /// </returns>
        private Boolean ReceiveGreetingMessage()
        {
            Match tMatch = Match.Empty;

            // Allow ourselves to time-out, just in case.
            mSocket.ReceiveTimeout = 10000;

            // Check to see if the greeting message we received is valid.
            if (mTextBuffer.Length != 0 || ReceiveData()) {
                tMatch = mGrtMessageRegex.Match(mTextBuffer);
                mTextBuffer = mTextBuffer.Remove(0, tMatch.Length);
            }

            // Set our timeout back to normal.
            mSocket.ReceiveTimeout = 0;

            return tMatch.Success;
        }
        /// <summary>
        ///    Attempts to receive data from the server.
        /// </summary>
        /// 
        /// <returns>
        ///    true if the server sent information; otherwise false.
        /// </returns>
        private Boolean ReceiveData()
        {
            // Attempt to get more information from the socket.
            SocketError tError = SocketError.Success;
            Int32       tSize  = mSocket.Receive(mByteBuffer, 0, mByteBuffer.Length, SocketFlags.None, out tError);
            mTextBuffer += Encoding.UTF8.GetString(mByteBuffer, 0, tSize);

            // Connection was closed.
            if (tSize == 0 || tError != SocketError.Success)
                mIsReceiving = IsLoggedIn = false;

            // Return whether the receive was successful.
            return mIsReceiving;
        }
        /// <summary>
        ///    Parses all the identifiable data contained in the global buffer of text
        ///    received from the server into notifications, messages, or banned
        ///    messages.
        /// </summary>
        private void ParseData()
        {
            Match tMatch = Match.Empty;

            // While we have matches, parse the buffer.
            do {
                // Looking for a notification.
                if ((tMatch = mNotificationRegex.Match(mTextBuffer)).Success) {
                    mTextBuffer = mTextBuffer.Remove(0, tMatch.Length);
                    TeamspeakNotification tNotification = new TeamspeakNotification(tMatch.Value);
                    OnNotificationReceived(tNotification);
                }
                // Looking for a message.
                else if ((tMatch = mMessageRegex.Match(mTextBuffer)).Success) {
                    mTextBuffer = mTextBuffer.Remove(0, tMatch.Length);
                    TeamspeakMessage tMessage = new TeamspeakMessage(tMatch.Value);
                    OnMessageReceived(tMessage);
                    mMessages.Enqueue(tMessage);
                }
                // Looking for a banned message.
                else if ((tMatch = mBannedRegex.Match(mTextBuffer)).Success) {
                    mTextBuffer = mTextBuffer.Remove(0, tMatch.Length);
                    TeamspeakMessage tMessage = new TeamspeakMessage(tMatch.Groups[1].Value);
                    OnBanned(tMessage);
                    mMessages.Enqueue(tMessage);
                }
            } while (tMatch.Success);
        }



        #endregion

        public static class Async
        {
            /// <summary>
            ///    Attempts to connect asynchronously to a Teamspeak 3 server. Starts
            ///    sending and receiving information immediately after opening the
            ///    connection.
            /// </summary>
            public static void Connect(TeamspeakConnection conn)
            {
                conn.ConnectTask();
            }
            /// <summary>
            ///    Attempts to disconnect asynchronously from a Teamspeak 3 server by
            ///    sending the appropriate commands then shutting down the connection.
            /// </summary>
            public static void Disconnect(TeamspeakConnection conn)
            {
                conn.DisconnectTask();
            }
            /// <summary>
            ///    Sends a query asynchronously to a Teamspeak 3 server.
            /// </summary>
            /// 
            /// <param name="query">
            ///    The query to send asynchronously.
            /// </param>
            public static void Send(TeamspeakConnection conn, TeamspeakQuery query)
            {
                conn.SendTask(query);
            }
        }
        public static class Sync
        {
            /// <summary>
            ///    Attempts to connect synchronously to a Teamspeak 3 server. Starts
            ///    sending and receiving information immediately after opening the
            ///    connection.
            /// </summary>
            public static void Connect(TeamspeakConnection conn)
            {
                conn.ConnectTask().Wait();
            }
            /// <summary>
            ///    Attempts to disconnect synchronously from a Teamspeak 3 server by
            ///    sending the appropriate commands then shutting down the connection.
            /// </summary>
            public static void Disconnect(TeamspeakConnection conn)
            {
                conn.DisconnectTask().Wait();
            }
            /// <summary>
            ///    Sends a query synchronously to a Teamspeak 3 server and returns the
            ///    message that was sent back to us.
            /// </summary>
            /// 
            /// <param name="query">
            ///    The query to send.
            /// </param>
            /// 
            /// <returns>
            ///    The message that was received, describing how the query was executed.
            /// </returns>
            public static TeamspeakMessage Send(TeamspeakConnection conn, TeamspeakQuery query)
            {
                Task<TeamspeakMessage> tTask = conn.SendTask(query);
                tTask.Wait();
                return tTask.Result;
            }
        }

        #region Event Stuff



        // Delegates.
        public delegate void TeamspeakQueryHandler(TeamspeakConnection connection, TeamspeakQuery query);
        public delegate void TeamspeakMessageHandler(TeamspeakConnection connection, TeamspeakQuery query, TeamspeakMessage response);
        public delegate void TeamspeakNotificationHandler(TeamspeakConnection connection, TeamspeakNotification notification);
        public delegate void TeamspeakBannedHandler(TeamspeakConnection connection, TeamspeakMessage response);

        // Events.
        public event TeamspeakQueryHandler        QuerySent;
        public event TeamspeakMessageHandler      MessageReceived;
        public event TeamspeakNotificationHandler NotificationReceived;
        public event TeamspeakBannedHandler       Banned;

        // Safe Events.
        protected void OnQuerySent(TeamspeakQuery query)
        {
            if (QuerySent != null)
                QuerySent(this, query);
        }
        protected void OnMessageReceived(TeamspeakMessage response)
        {
            TeamspeakQuery tQuery = null;
            if (mQueries.TryDequeue(out tQuery)) {
                if (response.Id == "0")
                    if      (tQuery.Command == "login")  IsLoggedIn = true;
                    else if (tQuery.Command == "logout") IsLoggedIn = false;
                if (MessageReceived != null)
                    MessageReceived(this, tQuery, response);
            }
        }
        protected void OnNotificationReceived(TeamspeakNotification notification)
        {
            if (NotificationReceived != null)
                NotificationReceived(this, notification);
        }
        protected void OnBanned(TeamspeakMessage response)
        {
            mIsReceiving = mIsSending = false;
            if (Banned != null)
                Banned(this, response);
        }



        #endregion
    }
}
