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
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using VoipManager.Communication;

namespace VoipManager.Connections
{
    public abstract class BaseConnection<TRequest, TResponse>
        where TRequest  : IRequest
        where TResponse : IResponse
    {
        private Object mActionLock = new Object();
        private Object mSocketLock = new Object();

        private Task mSend;
        private Task mRecv;
        private Task mFinal = Task.Factory.StartNew(() => { });

        private Socket                  mSocket;
        private CancellationTokenSource mCancel;

        private ManualResetEventSlim mSendFinished = new ManualResetEventSlim(false);
        private ManualResetEventSlim mRecvFinished = new ManualResetEventSlim(false);
        private SocketAsyncEventArgs mSendSocketEventArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs mRecvSocketEventArgs = new SocketAsyncEventArgs();

        private BlockingCollection<TRequest>  mWaiting;
        private BlockingCollection<TResponse> mReceived;
        private ReaderWriterLockSlim          mDisposeLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                               Send                                *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void SendTask()
        {
            Boolean tAsync;

            Byte[]   tBytes;
            TRequest tRequest;

            do {
                try {
                    // Get the next request to send.
                    tRequest = mWaiting.Take(mCancel.Token);
                    tBytes   = tRequest.Raw;
                } catch (OperationCanceledException) {
                    break;
                }

                // Setup the send request.
                mSendSocketEventArgs.UserToken = tRequest;
                mSendSocketEventArgs.SetBuffer(tBytes, 0, tBytes.Length);
                mSendSocketEventArgs.Completed += SendHelper;

                lock (mSocketLock) {
                    // Check if we've canceled while we were waiting.
                    if (mCancel.Token.IsCancellationRequested) {
                        break;
                    }

                    // Perform the send.
                    mSendFinished.Reset();
                    tAsync = mSocket.SendAsync(mSendSocketEventArgs);
                }

                // Call the handler if the send completed synchronously.
                if (!tAsync) {
                    SendHelper(mSocket, mSendSocketEventArgs);
                }
                mSendFinished.Wait();

            } while (!mCancel.IsCancellationRequested);
        }

        private void SendHelper(Object sender, SocketAsyncEventArgs args)
        {
            Boolean tAsync = true;
            args.Completed -= SendHelper;
            
            lock (mSocketLock) {
                // Check if we've canceled while we were waiting.
                if (mCancel.Token.IsCancellationRequested) {
                    mSendFinished.Set();
                    return;
                }

                // Exit if we had a socket error.
                if (args.SocketError != SocketError.Success) {
                    mCancel.Cancel();
                    mSendFinished.Set();
                    return;
                }

                // While we haven't sent all the bytes, keep trying to send.
                if (args.BytesTransferred != args.Count) {
                    args.SetBuffer(args.Offset + args.BytesTransferred, args.Count - args.BytesTransferred);
                    tAsync = mSocket.SendAsync(args);
                }
                // Exit if the child class says we should.
                else {
                    if (!SendCompleted((TRequest)args.UserToken)) {
                        mCancel.Cancel();
                    }
                }
            }

            // Call the handler if the send completed synchronously.
            if (!tAsync) {
                SendHelper(mSocket, mRecvSocketEventArgs);
                return;
            }
            mSendFinished.Set();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Receive                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void RecvTask()
        {
            Boolean tAsync;

            mRecvSocketEventArgs.SetBuffer(new Byte[mSocket.ReceiveBufferSize], 0, mSocket.ReceiveBufferSize);

            do {
                // Setup the receive.
                mRecvSocketEventArgs.Completed += RecvHelper;

                lock (mSocketLock) {
                    // Check if we've canceled while we were waiting.
                    if (mCancel.Token.IsCancellationRequested) {
                        break;
                    }

                    // Perform the receive.
                    mRecvFinished.Reset();
                    tAsync = mSocket.ReceiveAsync(mRecvSocketEventArgs);
                }

                // Call the handler if the receive completed synchronously.
                if (!tAsync) {
                    RecvHelper(mSocket, mRecvSocketEventArgs);
                }
                mRecvFinished.Wait();

            } while (!mCancel.IsCancellationRequested);
        }

        private void RecvHelper(Object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= RecvHelper;

            lock (mSocketLock) {
                // Check if we've canceled while we were waiting.
                if (mCancel.Token.IsCancellationRequested) {
                    mRecvFinished.Set();
                    return;
                }

                // Exit if we had a socket error.
                if (args.SocketError != SocketError.Success) {
                    mCancel.Cancel();
                    mRecvFinished.Set();
                    return;
                }

                // Exit if the connection has closed.
                if (args.BytesTransferred == 0) {
                    mCancel.Cancel();
                    mRecvFinished.Set();
                    return;
                }

                // Exit if the child class says we should.
                if (!ReceiveCompleted(args.Buffer, args.BytesTransferred)) {
                    mCancel.Cancel();
                }
            }
            mRecvFinished.Set();
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Manager                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        private void FinalTask()
        {
            // Wait for the send/recv tasks to finish.
            Task.WaitAll(mSend, mRecv);
            
            try { mDisposeLock.EnterWriteLock();

                // Close down the socket while we have access to do that.
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
                mSocket = null;

                // Dispose of the resources correctly.
                mCancel.Dispose();
                mWaiting.Dispose();
                mReceived.Dispose();

                // Let the child classes cleanup resources as well.
                CleanupChild();
            } finally {
                mDisposeLock.ExitWriteLock();
            }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                    Connect / Disconnect / Send                    *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        protected void InternalConnect(EndPoint host)
        {
            try { mDisposeLock.EnterReadLock();

                if (mSocket != null) {
                    return;
                }

            } finally { mDisposeLock.ExitReadLock(); }
            mFinal.Wait();

            // Allocate the new socket.
            mSocket = new Socket(host.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
                ExclusiveAddressUse = true,                       // Don't allow other sockets to use this address.
                LingerState         = new LingerOption(false, 0), // Don't stay open after close is called.
                NoDelay             = true,                       // Don't clump data together.
                SendBufferSize      = 8192,
                ReceiveBufferSize   = 8192,
                Ttl                 = 32
            };

            try {
                mSocket.Connect(host);

                // Setup the resources.
                mCancel   = new CancellationTokenSource();
                mWaiting  = new BlockingCollection<TRequest>(new ConcurrentQueue<TRequest>());
                mReceived = new BlockingCollection<TResponse>(new ConcurrentQueue<TResponse>());
                InitializeChild();

                // Start the send, receive, and manager tasks.
                mSend  = Task.Factory.StartNew(() => SendTask());
                mRecv  = Task.Factory.StartNew(() => RecvTask());
                mFinal = Task.Factory.StartNew(() => FinalTask());

                // Exit if the child class says this isn't a valid connection.
                if (!ConnectCompleted()) {
                    InternalDisconnect();
                    return;
                }

                OnConnected();
            }
            catch {
                mSocket = null;
                throw;
            }
        }

        protected void InternalDisconnect()
        {
            try { mDisposeLock.EnterReadLock();

                if (mSocket == null) {
                    return;
                }
                mSocket.Shutdown(SocketShutdown.Both);

                OnDisconnected();

            } finally { mDisposeLock.ExitReadLock(); }

            // Wait for the connection to close before being done with the disconnect.
            mFinal.Wait();
        }

        protected TResponse InternalSend(TRequest request, ManualResetEventSlim added)
        {
            try { mDisposeLock.EnterReadLock();

                if (mSocket == null) {
                    added.Set();
                    return default(TResponse);
                }

                // Let the outside know we've added the request to the queue.
                mWaiting.Add(request);
                added.Set();

                TResponse tResponse = mReceived.Take(mCancel.Token);
                if (tResponse.Request == null) {
                    tResponse.Request = request;
                }
                return tResponse;

            }
            catch (OperationCanceledException) {
                return default(TResponse);
            }
            finally { mDisposeLock.ExitReadLock(); }
        }

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                             Abstract                              *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        protected abstract void InitializeChild();

        protected abstract void CleanupChild();

        protected abstract Boolean SendCompleted(TRequest request);

        protected abstract Boolean ReceiveCompleted(Byte[] bytes, Int32 count);

        protected abstract Boolean ConnectCompleted();

        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
         *                              Events                               *
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
        protected void OnConnected()
        {
            if (Connected != null) {
                Connected(this);
            }
        }
        protected void OnDisconnected()
        {
            if (Disconnected != null) {
                Disconnected(this);
            }
        }
        protected void OnSent(TRequest request)
        {
            if (SentRequest != null) {
                SentRequest(this, request);
            }
        }
        protected void OnReceived(TResponse response)
        {
            // Wait until the request has been set before firing the event.
            mReceived.Add(response);
            while (response.Request == null) {
                Thread.Yield();
            }

            if (ReceivedResponse != null) {
                ReceivedResponse(this, response);
            }
        }

        public event StatusHandler Connected;
        public event StatusHandler Disconnected;
        public event RequestHandler SentRequest;
        public event ResponseHandler ReceivedResponse;

        public delegate void StatusHandler(BaseConnection<TRequest, TResponse> connection);
        public delegate void RequestHandler(BaseConnection<TRequest, TResponse> connection, TRequest request);
        public delegate void ResponseHandler(BaseConnection<TRequest, TResponse> connection, TResponse response);
    }
}
