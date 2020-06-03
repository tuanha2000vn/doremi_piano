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

namespace ForieroEngine.ObjectPool
{
    /// <summary>
    /// Represents an object that implements IRecyclable contract, allowing the object instance to be reused.
    /// </summary>
    public abstract class RecyclableObject : IRecyclable, IDisposable
    {
        #region IRecyclable Members
        private ReleaseInstanceDelegate Release = null;

        /// <summary>
        /// A fild that contains the value representing whether the object is acquired or not.
        /// </summary>
        protected bool ObjectAcquired = false;

        /// <summary>
        /// Recycles (resets) the object to the original state.
        /// </summary>
        public abstract void Recycle();

        /// <summary>
        /// Binds an <see cref="ReleaseInstanceDelegate"/> delegate which releases the <see cref="IRecyclable"/> object
        /// instance back to the pool.
        /// </summary>
        /// <param name="releaser">The <see cref="ReleaseInstanceDelegate"/> delegate to bind.</param>
        public void Bind(ReleaseInstanceDelegate releaser)
        {
            this.Release = releaser;
        }

        /// <summary>
        /// Invoked when a pool acquires the instance. 
        /// </summary>
        public void OnAcquire()
        {
            // Flag this as acquired
            this.ObjectAcquired = true;
        }

        /// <summary>
        /// Gets whether this <see cref="RecyclableObject"/> object is pooled or not.
        /// </summary>
        public bool IsPooled
        {
            get { return Release != null; }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Object is still registered for finalization
            if (Release != null && this.ObjectAcquired)
            {
                // Release back to the pool.
                this.ObjectAcquired = false;
                this.Release(this);
                GC.SuppressFinalize(this);
                GC.ReRegisterForFinalize(this);
            }
            else
            {
                // Otherwise, the object is actually going to die.
                Dispose(true);

                // No need to finalize it
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Attempts to release this instance back to the pool. If the instance is not pooled, nothing will be done.
        /// </summary>
        public void TryRelease()
        {
            // Release back to the pool.
            if (Release != null && this.ObjectAcquired)
            {
                this.ObjectAcquired = false;
                this.Release(this);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the object and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"> 
        /// If set to true, release both managed and unmanaged resources, othewise release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {

        }

        /// <summary>
        /// Finalizer for the recyclable object.
        /// </summary>
        ~RecyclableObject()
        {
            //Console.WriteLine("Finalize()");
            if (Release != null && this.ObjectAcquired)
            {
                // Release back to the pool and register back to the finalizer thread.
                this.ObjectAcquired = false;
                this.Release(this);
                GC.ReRegisterForFinalize(this);
            }
            else
            {
                // Otherwise, the object is actually going to die.
                Dispose(false);
            }
        }
        #endregion

    }

    /// <summary>
    /// The <see cref="ReleaseInstanceDelegate"/> delegate is used as a callback to return an object back to the pool.
    /// </summary>
    public delegate void ReleaseInstanceDelegate(IRecyclable @object);
}
