#region Copyright (c) 2012 Roman Atachiants
/*************************************************************************
 * 
 * Copyright (C) 2012 Roman Atachiants
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * OTHER DEALINGS IN THE SOFTWARE.
*************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections.Generic.Concurrent;

namespace ForieroEngine.ObjectPool
{
    /// <summary>
    /// This class represents a strongly-typed generic, concurrrent object pool.
    /// </summary>
    /// <typeparam name="T">The type of the items to manage in the pool.</typeparam>
    public class ConcurrentPool<T> : IRecycler<T>, IDisposable
        where T : class, IRecyclable
    {
        /// <summary>
        /// Creates an instance of T for the given recycler.
        /// </summary>
        /// <param name="recycler">The recycler that creates the instance.</param>
        /// <returns>The instance of T.</returns>
        public delegate T CreateInstanceDelegate(IRecycler recycler);


        private int InstancesInUseCount;
        private string Name = String.Empty;
        private ConcurrentQueue<T> Instances = new ConcurrentQueue<T>();
        private CreateInstanceDelegate Constructor = null;
        private ReleaseInstanceDelegate Releaser;

        #region Constructors
        /// <summary>
        /// Constructs a ConcurrentPool object.
        /// </summary>
        /// <param name="name">The name for the <see cref="ConcurrentPool&lt;T&gt;"/> instance.</param>
        public ConcurrentPool(string name) : this(name, null, 0) { }

        /// <summary>
        /// Constructs a ConcurrentPool object.
        /// </summary>
        /// <param name="name">The name for the ConcurrentPool instance.</param>
        /// <param name="constructor">The <see cref="CreateInstanceDelegate"/> delegate that is used to construct the <see cref="IRecyclable"/> instance.</param>
        public ConcurrentPool(string name, CreateInstanceDelegate constructor) : this(name, constructor, 0) { }

        /// <summary>
        /// Constructs a ConcurrentPool object.
        /// </summary>
        /// <param name="name">The name for the ConcurrentPool instance.</param>
        /// <param name="initialCapacity">Initial pool capacity.</param>
        public ConcurrentPool(string name, int initialCapacity) : this(name, null, initialCapacity) { }

        /// <summary>
        /// Constructs a ConcurrentPool object.
        /// </summary>
        /// <param name="name">The name for the ConcurrentPool instance.</param>
        /// <param name="constructor">The <see cref="CreateInstanceDelegate"/> delegate that is used to construct the <see cref="IRecyclable"/> instance.</param>
        /// <param name="initialCapacity">Initial pool capacity.</param>
        public ConcurrentPool(string name, CreateInstanceDelegate constructor, int initialCapacity)
        {
            this.Name = name;
            this.Constructor = constructor;
            this.Releaser = new ReleaseInstanceDelegate((this as IRecycler).Release);
            if (initialCapacity > 0)
            {
                // Create instances
                for (int i = 0; i < initialCapacity; ++i)
                    Instances.Enqueue(CreateInstance());
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the overall number of elements managed by this pool.
        /// </summary>
        public int Count
        {
            get { return InstancesInUseCount + this.Instances.Count; }
        }

        /// <summary>
        /// Gets the number of available elements currently contained in the pool.
        /// </summary>
        public int AvailableCount
        {
            get { return this.Instances.Count; }
        }

        /// <summary>
        /// Gets the number of elements currently in use and not available in this pool.
        /// </summary>
        public int InUseCount
        {
            get { return InstancesInUseCount; }
        }

        /// <summary>
        /// Gets a value that indicates whether the available pool is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Instances.IsEmpty; }
        }
        #endregion

        #region Virtual Members
        /// <summary>
        /// Allocates a new instance of T.
        /// </summary>
        /// <returns>Allocated instance of T.</returns>
        protected virtual T CreateInstance()
        {
            // If there is no constructor defined, create a new one.
            if (this.Constructor == null)
                this.Constructor = _ => Activator.CreateInstance<T>();

            // Create a new instance
            T instance = Constructor(this);
            instance.Bind(this.Releaser);
            return instance;
        }
        #endregion

        #region IRecycler<T> Members
        /// <summary>
        /// Acquires an instance of a recyclable object.
        /// </summary>
        /// <returns>The acquired instance.</returns>
        public virtual T Acquire()
        {
            // Increment the counter
            Interlocked.Increment(ref InstancesInUseCount);

            // Try to get the instance from the queue
            T instance;
            if (Instances.TryDequeue(out instance))
            {
                instance.OnAcquire();
                return instance;
            }

            // Dequeue failed, create a new instance.
            instance = CreateInstance();
            instance.OnAcquire();
            return instance;
        }

        /// <summary>
        /// Releases an instance of a recyclable object back to the pool.
        /// </summary>
        /// <param name="instance">The instance of IRecyclable to release.</param>
        public virtual void Release(T instance)
        {
            // Recycle the instance first.
            instance.Recycle();

            // Put it back to the pool.
            Instances.Enqueue(instance);

            // Decrement the counter
            Interlocked.Decrement(ref InstancesInUseCount);
        }
        #endregion

        #region IRecycler Members
        /// <summary>
        /// Acquires an instance of a recyclable object.
        /// </summary>
        /// <returns>The acquired instance.</returns>
        IRecyclable IRecycler.Acquire()
        {
            return this.Acquire();
        }

        /// <summary>
        /// Releases an instance of a recyclable object back to the pool.
        /// </summary>
        /// <param name="instance">The instance of IRecyclable to release.</param>
        void IRecycler.Release(IRecyclable instance)
        {
            this.Release(instance as T);
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the ByteSTream class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"> 
        /// If set to true, release both managed and unmanaged resources, othewise release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            while (this.Instances.Count > 0)
            {
                T instance;
                if (this.Instances.TryDequeue(out instance))
                    instance.Bind(null); // Bind it to null and release to the GC
            }

        }

        /// <summary>
        /// Finalizes this concurrent pool
        /// </summary>
        ~ConcurrentPool()
        {
            Dispose(false);
        }
        #endregion

        #region Object Members
        /// <summary>
        /// Constructs a user-friendly diagnostics string for this pool.
        /// </summary>
        /// <returns>The diagnostincs string.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion
    }
}
