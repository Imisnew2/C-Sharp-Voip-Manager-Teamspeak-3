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
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace VoipManager.Connections
{
    using VoipManager.Teamspeak3.Communication;

    public sealed class Teamspeak3Connection : BaseConnection<Teamspeak3Request, Teamspeak3Response>, IDisposable
    {
        #region Constants / Regexes

        // Examples:
        // TS3\n\r
        // Welcome to the TeamSpeak 3 ServerQuery interface, type \"help\" for a list of commands and \"help command\" for information on a specific command.\n\r
        private static readonly Regex mGreetingHeaderRegex  = new Regex(String.Format("^TS3{0}", Teamspeak3Message.SeperatorRegex));
        private static readonly Regex mGreetingMessageRegex = new Regex(String.Format("^Welcome to the TeamSpeak 3 ServerQuery interface, type \\\"help\\\" for a list of commands and \\\"help <command>\\\" for information on a specific command.{0}", Teamspeak3Message.SeperatorRegex));

        #endregion

        private Int32 mConnectTimeout;

        private BlockingCollection<Task> mTasks;
        private ManualResetEventSlim     mGrtHdrRecved;
        private ManualResetEventSlim     mGrtMsgRecved;
        
        private BlockingCollection<Teamspeak3Request> mRequests;
        private BlockingCollection<Teamspeak3Message> mResponses;

        private String  mRawText;
        private Boolean mIsBanned;

        private Object  mDisposedLock;
        private Boolean mHasBeenDisposed;
        private Boolean mMarkedForDisposal;

        private CancellationTokenSource mCancel;

        public Teamspeak3Connection(Int32 connectTimeout = 10000)
        {
            base.SentRequest      += (c, r) => OnSent(r);
            base.ReceivedResponse += (c, r) => OnReceived(r);

            mConnectTimeout = connectTimeout;

            mTasks        = new BlockingCollection<Task>();
            mGrtHdrRecved = new ManualResetEventSlim(false);
            mGrtMsgRecved = new ManualResetEventSlim(false);

            mDisposedLock      = new Object();
            mMarkedForDisposal = false;
            mHasBeenDisposed   = false;

            Task.Factory.StartNew(() => {

                while (!mHasBeenDisposed) {
                    mTasks.Take().RunSynchronously();
                }
                mTasks.Dispose();
                mGrtHdrRecved.Dispose();
                mGrtMsgRecved.Dispose();
            });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Helpers                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private Boolean ParseGreetingHeader()
        {
            Match tMatch = mGreetingHeaderRegex.Match(mRawText);
            if (tMatch.Success) {
                mRawText = mRawText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                mGrtHdrRecved.Set();
                return true;
            }
            return false;
        }

        private Boolean ParseGreetingMessage()
        {
            Match tMatch = mGreetingMessageRegex.Match(mRawText);
            if (tMatch.Success) {
                mRawText = mRawText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                mGrtMsgRecved.Set();
                return true;
            }
            return false;
        }

        private Teamspeak3Message ParseMessage()
        {
            Match tMatch = Teamspeak3Message.MessageRegex.Match(mRawText);
            if (tMatch.Success) {
                mRawText = mRawText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                return new Teamspeak3Message(tMatch.Value);
            }
            return null;
        }

        private Teamspeak3Message ParseBanned()
        {
            Match tMatch = Teamspeak3Message.BannedRegex.Match(mRawText);
            if (tMatch.Success) {
                mRawText = mRawText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                return new Teamspeak3Message(tMatch.Value);
            }
            return null;
        }

        private Teamspeak3Notification ParseNotification()
        {
            Match tMatch = Teamspeak3Notification.NotificationRegex.Match(mRawText);
            if (tMatch.Success) {
                mRawText = mRawText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                return new Teamspeak3Notification(tMatch.Value);
            }
            return null;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                       Teamspeak 3 Specific                        *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        protected override void InitializeChild()
        {
            // Clear out the text buffer.
            mRawText  = String.Empty;
            mIsBanned = false;

            // Setup the 
            mRequests  = new BlockingCollection<Teamspeak3Request>();
            mResponses = new BlockingCollection<Teamspeak3Message>();

            // Reset whether we've received the headers.
            mGrtHdrRecved.Reset();
            mGrtMsgRecved.Reset();

            // Setup the cancellation token.
            mCancel = new CancellationTokenSource();
        }

        protected override void CleanupChild()
        {
            // We may have exited before these were set, so set them now.
            mGrtHdrRecved.Set();
            mGrtMsgRecved.Set();

            // Cancel the token.
            mCancel.Cancel();
        }

        protected override Boolean SendCompleted(Teamspeak3Request request)
        {
            OnSent(request);
            mRequests.Add(request);
            return !request.Command.Equals("quit", StringComparison.OrdinalIgnoreCase);
        }

        protected override Boolean ReceiveCompleted(Byte[] bytes, Int32 count)
        {
            Boolean tTryAgain;
            Teamspeak3Message tMessage;
            Teamspeak3Notification tNotification;

            // Decode the response into text and attempt to find a response.
            mRawText += Encoding.UTF8.GetString(bytes, 0, count);

            do {
                tTryAgain = false;
                // Check the greeting header if we haven't found it already.
                if (!mGrtHdrRecved.IsSet) {
                    tTryAgain = ParseGreetingHeader();
                }
                // Check the greeting message if we haven't found it already.
                else if (!mGrtMsgRecved.IsSet) {
                    tTryAgain = ParseGreetingMessage();
                }
                // Check to see if there is a notification (single line).
                else if ((tNotification = ParseNotification()) != null) {
                    OnNotified(tNotification);
                    tTryAgain = true;
                }
                // Otherwise, check to see if there is a message (multiple lines).
                else if ((tMessage = ParseMessage()) != null) {
                    mResponses.Add(tMessage);
                    tTryAgain = true;
                }
                else if ((tMessage = ParseBanned()) != null) {
                    mResponses.Add(tMessage);
                    mIsBanned = true;
                    return false;
                }
                // Loop as long as we are removing text from mText.
            } while (tTryAgain);

            return true;
        }

        protected override Boolean ConnectCompleted()
        {
            // Wait 'x' seconds before disconnecting because we failed to receive the header greeting/message.
            DateTime tTimeoutDate = DateTime.Now.AddMilliseconds(mConnectTimeout);
            TimeSpan tTimeoutSpan = tTimeoutDate - DateTime.Now;
            if (tTimeoutSpan.TotalMilliseconds > 0 && mGrtHdrRecved.Wait(tTimeoutSpan)) {
                tTimeoutSpan = tTimeoutDate - DateTime.Now;
                if (tTimeoutSpan.TotalMilliseconds > 0 && mGrtMsgRecved.Wait(tTimeoutSpan)) {
                    return true;
                }
            }
            return false;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                         Public Facing API                         *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public void Connect(EndPoint host)
        {
            CheckDisposed();
            ConnectAsync(host).Wait();
        }

        public void Disconnect()
        {
            CheckDisposed();
            DisconnectAsync().Wait();
        }

        public Teamspeak3Message Send(Teamspeak3Request request)
        {
            CheckDisposed();
            var tSendTask = SendAsync(request);
            tSendTask.Wait();
            return tSendTask.Result;
        }

        public Task ConnectAsync(EndPoint host)
        {
            CheckDisposed();
            var tConnectTask = new Task(() => InternalConnect(host));

            mTasks.Add(tConnectTask);
            return tConnectTask;
        }

        public Task DisconnectAsync()
        {
            CheckDisposed();
            var tDisconnectTask = new Task(() => InternalDisconnect());

            mTasks.Add(tDisconnectTask);
            return tDisconnectTask;
        }

        public Task<Teamspeak3Message> SendAsync(Teamspeak3Request request)
        {
            CheckDisposed();
            var tSendTask = new Task<Teamspeak3Message>(() => {

                Teamspeak3Message tResponse = null;
                try {
                    // Send is finished once we can "take" the request.
                    if (InternalSend(request)) {
                        mRequests.Take(mCancel.Token);

                        // Receive is finished once we can "take" the response.
                        tResponse = mResponses.Take(mCancel.Token);
                        if (tResponse != null) {
                            OnReceived(tResponse);
                        }
                    }
                }
                catch (OperationCanceledException) { }

                // We're banned if "mIsBanned" is set.
                if (mIsBanned) {
                    mIsBanned = false;
                    OnBanned(tResponse);
                }
                return tResponse;
            });

            mTasks.Add(tSendTask);
            return tSendTask;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                             IDisposed                             *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public void Dispose()
        {
            Task tDisposeTask;
            lock (mDisposedLock) {

                mMarkedForDisposal = true;
                tDisposeTask = new Task(() => {

                    InternalDisconnect();
                    mHasBeenDisposed = false;
                });

                mTasks.Add(tDisposeTask);
            }
            tDisposeTask.Wait();
        }

        private void CheckDisposed()
        {
            lock (mDisposedLock) {
                if (mMarkedForDisposal) {
                    throw new ObjectDisposedException(this.GetType().Name);
                }
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Events                               *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        new public delegate void RequestHandler(Teamspeak3Connection connection, Teamspeak3Request request);
        new public delegate void ResponseHandler(Teamspeak3Connection connection, Teamspeak3Message message);
        public     delegate void BannedHandler(Teamspeak3Connection connection, Teamspeak3Message message);
        public     delegate void NotificationHandler(Teamspeak3Connection connection, Teamspeak3Notification notification);

        new public event RequestHandler      SentRequest;
        new public event ResponseHandler     ReceivedResponse;
        public     event BannedHandler       Banned;
        public     event NotificationHandler Notified;
        
        new private void OnSent(Teamspeak3Request request)
        {
            if (SentRequest != null) {
                SentRequest(this, request);
            }
        }
        new private void OnReceived(Teamspeak3Response response)
        {
            if (ReceivedResponse != null) {
                ReceivedResponse(this, (Teamspeak3Message)response);
            }
        }
        private     void OnBanned(Teamspeak3Message message)
        {
            if (Banned != null) {
                Banned(this, message);
            }
        }
        private     void OnNotified(Teamspeak3Notification notification)
        {
            if (Notified != null) {
                Notified(this, notification);
            }
        }
    }
}
