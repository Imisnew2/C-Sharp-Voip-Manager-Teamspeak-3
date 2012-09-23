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
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Teamspeak.ServerQuery.Data;

namespace Teamspeak.ServerQuery
{

public class TeamspeakConnection
{
    // ------------------------------------------------------------------------
    // -                               Constants                              -
    // ------------------------------------------------------------------------
    // --- Protected ---

    /// <summary>
    ///    Matches the greeting header that is sent from a Teamspeak 3 server.
    /// </summary>
    /// <example>TS3\n\r</example>
    protected static readonly Regex nGreetingHeaderRegex = new Regex(String.Format("^TS3{0}", TeamspeakMessage.SeperatorRegex));
    /// <summary>
    ///    Matches the greeting message that is sent from a Teamspeak 3 server.
    /// </summary>
    /// <example>Welcome to the TeamSpeak 3 ServerQuery interface, type \"help\" for a list of commands and \"help <command>\" for information on a specific command.\n\r</example>
    protected static readonly Regex nGreetingMessageRegex = new Regex(String.Format("^Welcome to the TeamSpeak 3 ServerQuery interface, type \\\"help\\\" for a list of commands and \\\"help <command>\\\" for information on a specific command.{0}", TeamspeakMessage.SeperatorRegex));

    /// <summary>
    ///    Matches notification responses from a Teamspeak 3 server.
    /// </summary>
    /// <example>notifyclientleftview cfid=1 ctid=0 reasonid=8 reasonmsg=leaving clid=5\n\r</example>
    protected static readonly Regex nNotificationRegex = new Regex(String.Format("^notify.+?{0}", TeamspeakMessage.SeperatorRegex));
    /// <summary>
    ///    Matches message responses from a Teamspeak 3 server.
    /// </summary>
    /// <example>version=3.0.6.1 build=1340956745 platform=Windows\n\rerror id=0 msg=ok\n\r</example>
    protected static readonly Regex nMessageRegex = new Regex(String.Format("^((.+?{0})*?)error id=.+?{0}", TeamspeakMessage.SeperatorRegex));
    /// <summary>
    ///    Matches banned responses from a Teamspeak 3 server.
    /// </summary>
    /// <example>error id=3331 msg=flood\\sban</example>
    /// <example>error id=3329 msg=connection\\sfailed,\\syou\\sare\\sbanned extra_msg=you\\smay\\sretry\\sin\\s600\\sseconds</example>
    protected static readonly Regex nBannedRegex = new Regex(String.Format("^(error id=(3331|3329).*?)(\\\\n\\\\r|)$"));


    // ------------------------------------------------------------------------
    // -                              Properties                              -
    // ------------------------------------------------------------------------
    // --- Public ---

    /// <summary>
    ///    The ip and port the connection is using to send information.
    /// </summary>
    public IPEndPoint LocalAddress { get; private set; }
    /// <summary>
    ///    The ip and port the connection is receiving information from.
    /// </summary>
    public IPEndPoint RemoteAddress { get; private set; }


    // ------------------------------------------------------------------------
    // -                               Variables                              -
    // ------------------------------------------------------------------------
    // --- Private ---

    // mLock: Guards the socket from multiple thread access.
    private Object mLock = new Object();
    
    // mTasks:     The tasks queue.
    // mTasksSema: Notifies when a task is ready to be executed. 
    // mTasksKill: Signals the task thread to be killed.
    private ConcurrentQueue<Task> mTasks     = new ConcurrentQueue<Task>();
    private SemaphoreSlim         mTasksSema = new SemaphoreSlim(0, Int32.MaxValue);
    private ManualResetEventSlim  mTasksKill = new ManualResetEventSlim(false);

    // mMessages:     The messages received.
    // mMessagesSema: Notifies when a message is received.
    // mMessagesKill: Signals to stop waiting for a response.
    private ConcurrentQueue<TeamspeakMessage> mMessages     = new ConcurrentQueue<TeamspeakMessage>();
    private SemaphoreSlim                     mMessagesSema = new SemaphoreSlim(0, Int32.MaxValue);
    private ManualResetEventSlim              mMessagesKill = new ManualResetEventSlim(false);

    // mQueries: The queries that were sent.
    private ConcurrentQueue<TeamspeakQuery> mQueries = new ConcurrentQueue<TeamspeakQuery>();
        
    // mSocket:     Socket used to send/recv info with the Teamspeak 3 server.
    // mSocketHost: The connection information for the Teamspeak 3 server.
    private Socket     mSocket;
    private IPEndPoint mSocketHost;

    // mBufferByte: Plain byte info received from the Teamspeak 3 server.
    // mBufferText: UTF-8 encoded info received from the Teamspeak 3 server.
    private Byte[] mBufferByte;
    private String mBufferText;

    // mRecv: The task used to receive information from the Teamspeak 3 server.
    private Task mRecv;


    // ------------------------------------------------------------------------
    // -                                Methods                               -
    // ------------------------------------------------------------------------
    // --- Constructors ---

    /// <summary>
    ///    Creates a TeamspeakConnection associated with a specific Teamspeak 3
    ///    server.
    /// </summary>
    public TeamspeakConnection(String host, UInt16 port)
    {
        // Attempt to parse the host as an IPv4 address.
        IPAddress tIpAddress = IPAddress.None;
        if (!IPAddress.TryParse(host, out tIpAddress))
        {
            // Attempt to find the address using the host name.
            IPAddress[] tIpAddresses = Dns.GetHostAddresses(host);
            for (int i = 0; tIpAddress == null && i < tIpAddresses.Length; i++)
                if (tIpAddresses[i].AddressFamily == AddressFamily.InterNetwork)
                    tIpAddress = tIpAddresses[i];
        }
        // Set the address.
        RemoteAddress = new IPEndPoint(tIpAddress, port);


        // Start the processing tasks.
        ThreadPool.QueueUserWorkItem(o => {

            // Setup thread information.
            Thread.CurrentThread.Name = "Teamspeak 3 Server Query API - Task Thread";

            // Allows us to be signaled when we want to end the program or process a task.
            Task         tTask = null;
            WaitHandle[] tWait = new WaitHandle[2] { mTasksKill.WaitHandle, mTasksSema.AvailableWaitHandle };
            while (WaitHandle.WaitAny(tWait) == 1) {

                // Process the task.
                mTasksSema.Wait();
                mTasks.TryDequeue(out tTask);
                tTask.RunSynchronously();
            }
        });
    }


    // --- Protected ---

    /// <summary>
    ///    Enqueues a task to connect to a Teamspeak 3 server and begin sending
    ///    and receiving information.
    /// </summary>
    /// <returns>The connect task that was just scheduled.</returns>
    protected virtual Task ConnectTask()
    {
        Task tTask = new Task(() => { ConnectMethod(); });
        mTasks.Enqueue(tTask);
        mTasksSema.Release();
        return tTask;
    }

    /// <summary>
    ///    Enqueues a task to disconnect from a Teamspeak 3 server and stop
    ///    sending and receiving information.
    /// </summary>
    /// <returns>The disconnect task that was just scheduled.</returns>
    protected virtual Task DisconnectTask()
    {
        Task tTask = new Task(() => { DisconnectMethod(); });
        mTasks.Enqueue(tTask);
        mTasksSema.Release();
        return tTask;
    }

    /// <summary>
    ///    Enqueues a task to send a query to a Teamspeak 3 server, then wait
    ///    until the response is received before returning.
    /// </summary>
    /// <param name="query">The query to be enqueued to be sent.</param>
    /// <returns>The send task that was just scheduled.</returns>
    protected virtual Task<TeamspeakMessage> SendTask(TeamspeakQuery query)
    {
        Task<TeamspeakMessage> tTask = new Task<TeamspeakMessage>(() => { return SendMethod(query); });
        mTasks.Enqueue(tTask);
        mTasksSema.Release();
        return tTask;
    }


    // --- Private ---

    /// <summary>
    ///    Establishes a connection to a Teamspeak 3 server, checks the headers
    ///    to make sure it's a Teamspeak 3 server, then begins sending and
    ///    receiving information.
    /// </summary>
    private Boolean ConnectMethod()
    {
        ManualResetEventSlim tDone = new ManualResetEventSlim(false);
        SocketAsyncEventArgs tArgs = new SocketAsyncEventArgs();
        lock (mLock) {

            // Check to make sure we're not connected.
            if (mSocket == null || !mSocket.Connected) {

                // Create a new socket.
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.ExclusiveAddressUse = true;
                mSocket.LingerState         = new LingerOption(false, 0);
                mSocket.NoDelay             = true;
                mSocket.SendBufferSize      = 8192;
                mSocket.ReceiveBufferSize   = 8192;
                mSocket.Ttl                 = 32;

                // Setup the necessary variables.
                mBufferByte = new Byte[mSocket.ReceiveBufferSize];
                mBufferText = String.Empty;

                // Create a new socket async args.
                tArgs.UserToken      = tDone;
                tArgs.RemoteEndPoint = RemoteAddress;
                tArgs.Completed      += ConnectCompleted;
                mSocket.ConnectAsync(tArgs);
            }
            // Otherwise, simply exit.
            else {
                tArgs.UserToken = false;
                tDone.Set();
            }
        }
        tDone.Wait();
        return (Boolean)tArgs.UserToken;
    }

    /// <summary>
    ///    Attempts to receive data from the server.
    /// </summary>
    /// <returns>
    ///     true if the server sent information; otherwise false.
    /// </returns>
    private Boolean ReceiveMethod()
    {
        ManualResetEventSlim tDone = new ManualResetEventSlim();
        SocketAsyncEventArgs tArgs = new SocketAsyncEventArgs();
        lock (mLock) {
            
            // Check to make sure we're connected.
            if (mSocket != null && mSocket.Connected) {

                // Setup the arguments for a receive.
                tArgs.SetBuffer(mBufferByte, 0, mBufferByte.Length);
                tArgs.UserToken = tDone;
                tArgs.Completed += ReceiveCompleted;
                mSocket.ReceiveAsync(tArgs);
            }
            // Otherwise, simply exit.
            else {
                tArgs.UserToken = false;
                tDone.Set();
            }
        }
        tDone.Wait();
        return (Boolean)tArgs.UserToken;
    }

    /// <summary>
    ///    Sends a query to a Teamspeak 3 server then waits for the response.
    /// </summary>
    /// <param name="query">
    ///    The query to send to the Teamspeak 3 server.
    /// </param>
    /// <returns>
    ///     The response received from the Teamspeak 3 server.
    /// </returns>
    private TeamspeakMessage SendMethod(TeamspeakQuery query)
    {
        ManualResetEventSlim tDone = new ManualResetEventSlim();
        SocketAsyncEventArgs tArgs = new SocketAsyncEventArgs();
        lock (mLock) {
            
            // Check to make sure we're connected.
            if (mSocket != null && mSocket.Connected) {

                // Save the query that we are sending.
                OnQuerySent(query);

                // Setup the arguments for a send.
                Byte[] tBuffer = Encoding.UTF8.GetBytes(query.ToString());
                tArgs.SetBuffer(tBuffer, 0, tBuffer.Length);
                tArgs.UserToken = tDone;
                tArgs.Completed += SendCompleted;
                mSocket.SendAsync(tArgs);
            }
            // Otherwise, simply exit.
            else {
                tArgs.UserToken = null;
                tDone.Set();
            }
        }
        tDone.Wait();
        return (TeamspeakMessage)tArgs.UserToken;
    }
    

    /// <summary>
    ///    Callback for ConnectMethod. Checks to see if the connection was
    ///    successful and sets up to begin sending and receiving data.
    /// </summary>
    /// <param name="sender">Who initiated the method.</param>
    /// <param name="e">How the method performed.</param>
    private void ConnectCompleted(Object sender, SocketAsyncEventArgs e)
    {
        // Setup the completed method.
        ManualResetEventSlim tDone = (ManualResetEventSlim)e.UserToken;
        e.UserToken = false;
        e.Completed -= ConnectCompleted;

        // Check to see if there was an error and if the greeting is correct.
        if (CheckSocketError(e.SocketError) && ReceiveGreetingHeader() && ReceiveGreetingMessage()) {

            // Set the local end point.
            LocalAddress = (IPEndPoint)mSocket.LocalEndPoint;

            // Setup the queries and messages queue.
            mQueries = new ConcurrentQueue<TeamspeakQuery>();
            mMessages = new ConcurrentQueue<TeamspeakMessage>();
            mMessagesSema.Dispose();
            mMessagesSema = new SemaphoreSlim(0, Int32.MaxValue);
            mMessagesKill.Reset();

            // Start parsing information from the server.
            mRecv = Task.Factory.StartNew(() => { while (ReceiveMethod()); });

            // Notify that we were successful.
            e.UserToken = true;
        }

        // Set whether the connect was successful.
        tDone.Set();
    }

    /// <summary>
    ///    Callback for ConnectMethod. Checks to see if receive had an error
    ///    and parses the information received.
    /// </summary>
    /// <param name="sender">Who initiated the method.</param>
    /// <param name="e">How the method performed.</param>
    private void ReceiveCompleted(Object sender, SocketAsyncEventArgs e)
    {
        // Setup the completed method.
        ManualResetEventSlim tDone = (ManualResetEventSlim)e.UserToken;
        e.UserToken = false;
        e.Completed -= ReceiveCompleted;
            
        // Check to see if there was an error and that the connection wasn't closed.
        if (CheckSocketError(e.SocketError)) {
                
            // Encode and parse the data we received.
            mBufferText += Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred);
            Match tMatch = Match.Empty;
            do {
                // Looking for a notification.
                if ((tMatch = nNotificationRegex.Match(mBufferText)).Success) {
                    mBufferText = mBufferText.Remove(0, tMatch.Length);
                    OnNotificationReceived(new TeamspeakNotification(tMatch.Value));
                }
                // Looking for a message.
                else if ((tMatch = nMessageRegex.Match(mBufferText)).Success) {
                    mBufferText = mBufferText.Remove(0, tMatch.Length);
                    OnMessageReceived(new TeamspeakMessage(tMatch.Value));
                }
                // Looking for a banned message.
                else if ((tMatch = nBannedRegex.Match(mBufferText)).Success) {
                    mBufferText = mBufferText.Remove(0, tMatch.Length);
                    OnBanned(new TeamspeakMessage(tMatch.Groups[1].Value));
                }
            } while (tMatch.Success);

            // Notify that we were successful.
            e.UserToken = true;
        }

        // If the server sent back 0 bytes, then the connection was closed.
        if (e.BytesTransferred == 0)
            CheckSocketError(SocketError.Shutdown);
            
        // Notify that we're done.
        tDone.Set();
    }

    /// <summary>
    ///    Callback for SendMethod. Checks to see if send had an error and
    ///    waits for the response to the send.
    /// </summary>
    /// <param name="sender">Who initiated the method.</param>
    /// <param name="e">How the method performed.</param>
    private void SendCompleted(Object sender, SocketAsyncEventArgs e)
    {
        // Setup the completed method.
        ManualResetEventSlim tDone = (ManualResetEventSlim)e.UserToken;
        e.UserToken = null;
        e.Completed -= ReceiveCompleted;

        // Check to see if there was an error.
        if (CheckSocketError(e.SocketError)) {

            // Wait for the query's response.
            WaitHandle[] tWait = new WaitHandle[2] { mMessagesKill.WaitHandle, mMessagesSema.AvailableWaitHandle };
            if (WaitHandle.WaitAny(tWait) == 1) {

                // Notify that we were successful (by setting the response).
                mMessagesSema.Wait();
                TeamspeakMessage tMessage = null;
                mMessages.TryDequeue(out tMessage);
                e.UserToken = tMessage;
            }
        }

        // Notify that we're done.
        tDone.Set();
    }


    /// <summary>
    ///    Disconnects a connection from a Teamspeak 3 server by sending the
    ///    appropriate commands rather than haphazardly closing the socket.
    ///    Waits until the associated threads are shut down as well.
    /// </summary>
    private void DisconnectMethod()
    {
        // Close the connection.
        SendMethod(TeamspeakQuery.BuildQuit());

        // Wait on this connection to finish up.
        if (mRecv != null)
            mRecv.Wait();
    }
    
    /// <summary>
    ///    Attempts to receive some data from the server that was connected to.
    ///    Then, check if the response matches up with the greeting header of a
    ///    Teamspeak 3 server.
    /// </summary>
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
        if (mBufferText.Length != 0 || ReceiveMethod()) {
            tMatch = nGreetingHeaderRegex.Match(mBufferText);
            mBufferText = mBufferText.Remove(0, tMatch.Length);
        }

        // Set our timeout back to normal.
        mSocket.ReceiveTimeout = 0;

        return tMatch.Success;
    }

    /// <summary>
    ///    Attempts to receive some data from the server that was connected to.
    ///    Then, check if the response matches up with the greeting message of
    ///    a Teamspeak 3 server.
    /// </summary>
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
        if (mBufferText.Length != 0 || ReceiveMethod()) {
            tMatch = nGreetingMessageRegex.Match(mBufferText);
            mBufferText = mBufferText.Remove(0, tMatch.Length);
        }

        // Set our timeout back to normal.
        mSocket.ReceiveTimeout = 0;

        return tMatch.Success;
    }


    /// <summary>
    ///    Closes the socket if there is an error.
    /// </summary>
    /// <param name="error">The result of the previous socket function.</param>
    /// <returns>
    ///     true if the function was successful, otherwise false.
    /// </returns>
    private Boolean CheckSocketError(SocketError error)
    {
        // Check to see if there was an error.
        if (error != SocketError.Success) {
            lock (mLock) {
                mSocket.Close();
                mSocket = null;
            }
        }

        // Return whether we were successful.
        return error == SocketError.Success;
    }


    // ------------------------------------------------------------------------
    // -                                Classes                               -
    // ------------------------------------------------------------------------

    public static class Async
    {
        /// <summary>
        ///    Attempts to connect asynchronously to a Teamspeak 3 server.
        ///    Starts sending and receiving information immediately after
        ///    opening the connection.
        /// </summary>
        public static void Connect(TeamspeakConnection conn)
        {
            conn.ConnectTask();
        }
        /// <summary>
        ///    Attempts to disconnect asynchronously from a Teamspeak 3 server
        ///    by sending the appropriate commands then shutting down the
        ///    connection.
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
        ///    Attempts to connect synchronously to a Teamspeak 3 server.
        ///    Starts sending and receiving information immediately after
        ///    opening the connection.
        /// </summary>
        public static void Connect(TeamspeakConnection conn)
        {
            conn.ConnectTask().Wait();
        }
        /// <summary>
        ///    Attempts to disconnect synchronously from a Teamspeak 3 server
        ///    by sending the appropriate commands then shutting down the
        ///    connection.
        /// </summary>
        public static void Disconnect(TeamspeakConnection conn)
        {
            conn.DisconnectTask().Wait();
        }
        /// <summary>
        ///    Sends a query synchronously to a Teamspeak 3 server and returns
        ///    the message that was sent back to us.
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


    // ------------------------------------------------------------------------
    // -                                Events                                -
    // ------------------------------------------------------------------------

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
        mQueries.Enqueue(query);
    }
    protected void OnMessageReceived(TeamspeakMessage response)
    {
        TeamspeakQuery tQuery = null;
        if (mQueries.TryDequeue(out tQuery)) {
            if (MessageReceived != null)
                MessageReceived(this, tQuery, response);
        }
        mMessages.Enqueue(response);
        mMessagesSema.Release();
    }
    protected void OnNotificationReceived(TeamspeakNotification notification)
    {
        if (NotificationReceived != null)
            NotificationReceived(this, notification);
    }
    protected void OnBanned(TeamspeakMessage response)
    {
        if (Banned != null)
            Banned(this, response);
        mMessages.Enqueue(response);
        mMessagesSema.Release();
    }
}

} // Teamspeak.ServerQuery