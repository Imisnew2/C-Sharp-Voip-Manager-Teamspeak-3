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
using VoipManager.Teamspeak3.Communication;

namespace VoipManager.Connections
{
    public sealed class Teamspeak3Connection : BaseConnection<Teamspeak3Request, Teamspeak3Response>, IDisposable
    {
        #region Constants / Regexes

        /// <example>TS3\n\r</example>
        private static readonly Regex mGreetingHeaderRegex = new Regex(String.Format("^TS3{0}", Teamspeak3Message.SeperatorRegex));

        /// <example>Welcome to the TeamSpeak 3 ServerQuery interface, type \"help\" for a list of commands and \"help command\" for information on a specific command.\n\r</example>
        private static readonly Regex mGreetingMessageRegex = new Regex(String.Format("^Welcome to the TeamSpeak 3 ServerQuery interface, type \\\"help\\\" for a list of commands and \\\"help <command>\\\" for information on a specific command.{0}", Teamspeak3Message.SeperatorRegex));

        #endregion

        private class Teamspeak3Task {
            public Task task  { get; set; }
            public Task ready { get; set; }
        }

        private Teamspeak3Settings mSettings;

        private Boolean mIsDisposed;
        private BlockingCollection<Teamspeak3Task> mTasks;

        private String mText;
        private ManualResetEventSlim mGrtHdrRecved = new ManualResetEventSlim(false);
        private ManualResetEventSlim mGrtMsgRecved = new ManualResetEventSlim(false);

        public Teamspeak3Connection(Teamspeak3Settings settings)
        {
            mSettings = settings;

            mIsDisposed = false;
            mTasks      = new BlockingCollection<Teamspeak3Task>(new ConcurrentQueue<Teamspeak3Task>());

            Task.Factory.StartNew(() => {
                while (!mIsDisposed) {

                    var ts3Task = mTasks.Take();
                    ts3Task.task.Start();
                    ts3Task.ready.RunSynchronously();
                }
                mTasks.Dispose();
            });
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Helpers                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private Boolean ParseGreetingHeader()
        {
            Match tMatch = mGreetingHeaderRegex.Match(mText);
            if (tMatch.Success) {
                mText = mText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                mGrtHdrRecved.Set();
                return true;
            }
            return false;
        }

        private Boolean ParseGreetingMessage()
        {
            Match tMatch = mGreetingMessageRegex.Match(mText);
            if (tMatch.Success) {
                mText = mText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                mGrtMsgRecved.Set();
                return true;
            }
            return false;
        }

        private Teamspeak3Message ParseMessage()
        {
            Match tMatch = Teamspeak3Message.MessageRegex.Match(mText);
            if (tMatch.Success) {
                mText = mText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                return new Teamspeak3Message(tMatch.Value);
            }
            return null;
        }

        private Teamspeak3Message ParseBanned()
        {
            Match tMatch = Teamspeak3Message.BannedRegex.Match(mText);
            if (tMatch.Success) {
                mText = mText.Remove(tMatch.Index, tMatch.Length).TrimStart();
                return new Teamspeak3Message(tMatch.Value);
            }
            return null;
        }

        private Teamspeak3Notification ParseNotification()
        {
            Match tMatch = Teamspeak3Notification.NotificationRegex.Match(mText);
            if (tMatch.Success) {
                mText = mText.Remove(tMatch.Index, tMatch.Length).TrimStart();
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
            mText = String.Empty;

            // Reset whether we've received the headers.
            mGrtHdrRecved.Reset();
            mGrtMsgRecved.Reset();
        }

        protected override void CleanupChild()
        {
            // We may have exited before these were set, so set them now.
            mGrtHdrRecved.Set();
            mGrtMsgRecved.Set();
        }

        protected override Boolean SendCompleted(Teamspeak3Request request)
        {
            OnSent(request);
            return !request.Command.Equals("quit", StringComparison.OrdinalIgnoreCase);
        }

        protected override Boolean ReceiveCompleted(Byte[] bytes, Int32 count)
        {
            Boolean tTryAgain;
            Teamspeak3Message tMessage;
            Teamspeak3Notification tNotification;

            // Decode the response into text and attempt to find a response.
            mText += Encoding.UTF8.GetString(bytes, 0, count);

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
                    OnReceived(tMessage);
                    tTryAgain = true;
                }
                else if ((tMessage = ParseBanned()) != null) {
                    OnReceived(tMessage);
                    OnBanned(tMessage);
                    return false;
                }
                // Loop as long as we are removing text from mText.
            } while (tTryAgain);

            return true;
        }

        protected override Boolean ConnectCompleted()
        {
            // Wait 'x' seconds before disconnecting because we failed to receive the header greeting/message.
            DateTime tTimeout = DateTime.Now.AddMilliseconds(mSettings.ConnectTimeout);
            return mGrtHdrRecved.Wait(tTimeout - DateTime.Now) && mGrtMsgRecved.Wait(tTimeout - DateTime.Now);
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                         Public Facing API                         *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public void Connect(EndPoint host)
        {
            ConnectAsync(host).Wait();
        }

        public void Disconnect()
        {
            DisconnectAsync().Wait();
        }

        public Teamspeak3Response Send(Teamspeak3Request request)
        {
            Task<Teamspeak3Response> t = SendAsync(request);
            t.Wait();
            return t.Result;
        }

        public Task ConnectAsync(EndPoint host)
        {
            Task task  = new Task(() => InternalConnect(host));
            Task ready = new Task(() => task.Wait());

            mTasks.Add(new Teamspeak3Task() {
                task  = task,
                ready = ready
            });
            return task;
        }

        public Task DisconnectAsync()
        {
            Task task  = new Task(() => InternalDisconnect());
            Task ready = new Task(() => task.Wait());

            mTasks.Add(new Teamspeak3Task() {
                task  = task,
                ready = ready
            });
            return task;
        }

        public Task<Teamspeak3Response> SendAsync(Teamspeak3Request request)
        {
            ManualResetEventSlim added = new ManualResetEventSlim(false);

            Task task  = new Task<Teamspeak3Response>(() => InternalSend(request, added));
            Task ready = new Task(() => {
                if (mSettings.ParallelRequests) added.Wait();
                else                            task.Wait();
            });

            mTasks.Add(new Teamspeak3Task() {
                task  = task,
                ready = ready
            });
            return (Task<Teamspeak3Response>)task;
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                             IDisposed                             *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public void Dispose()
        {
            Task task = new Task(() => {
                InternalDisconnect();
                mIsDisposed = false;
            });
            Task ready = Task.Factory.StartNew(() => task.Wait());

            mTasks.Add(new Teamspeak3Task() {
                task  = task,
                ready = ready
            });
            task.Wait();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Events                               *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        public delegate void BannedHandler(Teamspeak3Connection connection, Teamspeak3Message message);
        public delegate void NotificationHandler(Teamspeak3Connection connection, Teamspeak3Notification notification);

        public event BannedHandler Banned;
        public event NotificationHandler Notified;

        private void OnBanned(Teamspeak3Message message)
        {
            if (Banned != null) {
                Banned(this, message);
            }
        }
        private void OnNotified(Teamspeak3Notification notification)
        {
            if (Notified != null) {
                Notified(this, notification);
            }
        }
    }
}
