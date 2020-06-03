using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Collections.Generic.Concurrent
{
    
	public class ConcurrentQueue<T> : IEnumerable<T>, ICollection//, ISerializable, IDeserializationCallback
	{
		class Node
		{
			public T Value;
			public Node Next;
		}

		Node _head = new Node ();
		Node _tail;
		int _count;

		/// <summary>
		/// </summary>
		public ConcurrentQueue ()
		{
			_tail = _head;
		}

		public ConcurrentQueue (IEnumerable<T> enumerable)
			: this ()
		{
			foreach (T item in enumerable)
				Enqueue (item);
		}

		public void Enqueue (T item)
		{
			var node = new Node { Value = item };

			Node oldTail = null;

			bool update = false;
			while (!update) {
				oldTail = _tail;
				var oldNext = oldTail.Next;

				// Did tail was already updated ?
				if (_tail == oldTail) {
					if (oldNext == null) {
						// The place is for us
						update = Interlocked.CompareExchange (ref _tail.Next, node, null) == null;
					} else {
						// another Thread already used the place so give him a hand by putting tail where it should be
						Interlocked.CompareExchange (ref _tail, oldNext, oldTail);
					}
				}
			}
			// At this point we added correctly our node, now we have to update tail. If it fails then it will be done by another thread
			Interlocked.CompareExchange (ref _tail, node, oldTail);

			Interlocked.Increment (ref _count);
		}


		/// <summary>
		/// </summary>
		/// <returns></returns>
		public bool TryDequeue (out T value)
		{
			value = default(T);
			bool advanced = false;
			while (!advanced) {
				Node oldHead = _head;
				Node oldTail = _tail;
				Node oldNext = oldHead.Next;

				if (oldHead == _head) {
					// Empty case ?
					if (oldHead == oldTail) {
						// This should be false then
						if (oldNext != null) {
							// If not then the linked list is mal formed, update tail
							Interlocked.CompareExchange (ref _tail, oldNext, oldTail);
						}
						value = default(T);
						return false;
					} else {
						value = oldNext.Value;
						advanced = Interlocked.CompareExchange (ref _head, oldNext, oldHead) == oldHead;
					}
				}
			}

			Interlocked.Decrement (ref _count);
			return true;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public bool TryPeek (out T value)
		{
			if (IsEmpty) {
				value = default(T);
				return false;
			}

			Node first = _head.Next;
			value = first.Value;
			return true;
		}

		public void Clear ()
		{
			_count = 0;
			_tail = _head = new Node ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return InternalGetEnumerator ();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator ()
		{
			return InternalGetEnumerator ();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return InternalGetEnumerator ();
		}

		IEnumerator<T> InternalGetEnumerator ()
		{
			Node myHead = _head;
			while ((myHead = myHead.Next) != null) {
				yield return myHead.Value;
			}
		}

		void ICollection.CopyTo (Array array, int index)
		{
			T[] dest = array as T[];
			if (dest == null)
				return;
			CopyTo (dest, index);
		}

		public void CopyTo (T[] dest, int index)
		{
			IEnumerator<T> e = InternalGetEnumerator ();
			int i = index;
			while (e.MoveNext ()) {
				dest [i++] = e.Current;
			}
		}

		public T[] ToArray ()
		{
			T[] dest = new T[_count];
			CopyTo (dest, 0);
			return dest;
		}

		//public void GetObjectData (SerializationInfo info, StreamingContext context)
		//{
		//	throw new NotImplementedException ();
		//}

		bool ICollection.IsSynchronized {
			get { return true; }
		}

		//public void OnDeserialization (object sender)
		//{
		//	throw new NotImplementedException ();
		//}

		readonly object _syncRoot = new object ();

		object ICollection.SyncRoot {
			get { return _syncRoot; }
		}

		public int Count {
			get {
				return _count;
			}
		}

		public bool IsEmpty {
			get {
				return _count == 0;
			}
		}
	}
}

