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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Teamspeak.ServerQuery.Utils
{
    internal class BlockingQueue<T> : ConcurrentQueue<T>
    {
        private ReaderWriterLockSlim mPeek;
        private Semaphore            mCount;
        private ManualResetEvent     mBlock;
        private WaitHandle[]         mDequeue;

        /// <summary>
        ///    Sets whether to block when dequeuing.
        /// </summary>
        public bool Blocking {
            set {
                lock (mBlock)
                    if (value) mBlock.Reset();
                    else       mBlock.Set();
            }
        }


        /// <summary>
        ///    Initializes a new instance of the BlockingQueue&lt;T&gt; class
        ///    that is empty and has the default initial capacity.
        /// </summary>
        public BlockingQueue() : base() { Initialize(); }
        /// <summary>
        ///    Initializes a new instance of the BlockingQueue&lt;T&gt; class
        ///    that contains elements copied from the specified collection.
        /// </summary>
        /// 
        /// <param name="collection">
        ///    The collection whose elements are copied to the new BlockingQueue&lt;T&gt;.
        /// </param>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///    The collection argument is null.
        /// </exception>
        public BlockingQueue(IEnumerable<T> collection) : base(collection) { Initialize(); }


        /// <summary>
        ///    Allocates the resources to be used by the BlockingQueue&lt;T&gt;.
        /// </summary>
        private void Initialize()
        {
            mPeek    = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            mCount   = new Semaphore(0, int.MaxValue);
            mBlock   = new ManualResetEvent(false);
            mDequeue = new WaitHandle[2] { mCount, mBlock };
        }

        /// <summary>
        ///    Adds an object to the end of the BlockingQueue&lt;T&gt;.
        /// </summary>
        /// 
        /// <param name="item">
        ///    The object to add to the BlockingQueue&lt;T&gt;. The value can be null for
        ///    reference types.
        /// </param>
        new public void Enqueue(T item)
        {
            base.Enqueue(item);
            mCount.Release();
        }
        /// <summary>
        ///    Attempts to remove and return the object at the beginning of the
        ///    BlockingQueue&lt;T&gt;.
        /// </summary>
        /// 
        /// <param name="result">
        ///    When this method returns, if the operation was successful, result contains
        ///    the object removed. If no object was available to be removed, the value is
        ///    unspecified.
        /// </param>
        /// 
        /// <returns>
        ///    true if an element was removed and returned from the beginning of the
        ///    BlockingQueue&lt;T&gt; successfully; otherwise, false.
        /// </returns>
        new public bool TryDequeue(out T result)
        {
            WaitHandle.WaitAny(mDequeue);
            mPeek.EnterWriteLock();
            try     { return base.TryDequeue(out result); }
            finally { mPeek.ExitWriteLock();              }
        }
        /// <summary>
        ///    Attempts to return an object from the beginning of the BlockingQueue&lt;T&gt;
        ///    without removing it.
        /// </summary>
        /// 
        /// <param name="result">
        ///    When this method returns, result contains an object from the beginning of
        //     the BlockingQueue&lt;T&gt; or an unspecified value if the operation failed.
        /// </param>
        /// 
        /// <returns>
        ///    true if and object was returned successfully; otherwise, false.
        /// </returns>
        new public bool TryPeek(out T result)
        {
            WaitHandle.WaitAny(mDequeue);
            mPeek.EnterReadLock();
            mCount.Release();
            try     { return base.TryPeek(out result); }
            finally { mPeek.ExitReadLock();            }
        }
    }
}
