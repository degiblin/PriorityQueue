using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue
{
	public class PriorityQueue<T> : IEnumerable<T>, ICollection, IEnumerable where T:IComparable
	{
		#region Attributes

		public enum Type {MAX, MIN}
		private T[] _nodes;

		#endregion

		#region Properties

		/// <summary>
		/// The current maximum capacity of the PQ
		/// </summary>
		public int Capacity { get; private set; }
		/// <summary>
		/// Whether it is a Max or Min PQ
		/// </summary>
		public Type HeapProperty { get; private set; }
		public T[] Heap { get { return _nodes; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new Priority Queue
		/// </summary>
		/// <param name="Capacity">How many Items are in the PQ</param>
		/// <param name="HeapProperty">Whether it is a Max or Min PQ</param>
		public PriorityQueue(int max_size = 31, Type HeapProperty = Type.MAX)
		{
			this.Capacity = max_size + 1;
			this.HeapProperty = HeapProperty;
			this._nodes = new T[this.Capacity];
		}
		/// <summary>
		/// Creates a new Priority Queue
		/// </summary>
		/// <param name="list">A data structure that implements IEnumerable</param>
		/// <param name="HeapProperty">Whether it is a Max or Min PQ</param>
		public PriorityQueue(IEnumerable<T> list, Type HeapProperty = Type.MAX)
		{
			this.Capacity = 2;
			while (this.Capacity <= list.Count<T>())
				this.Capacity = this.Capacity * 2;
			this._nodes = new T[this.Capacity];
			this.Count = 0;
			this.HeapProperty = HeapProperty;

			foreach (T item in list)
			{
				this._nodes[Count++] = item;
			}

			BuildHeap();
		}

		#endregion

		#region Heap Maintenance Methods

		private void BuildHeap()
		{
			for (int i = Count / 2; i > 0; i--)
			{
				Heapify(i);
			}
		}
		private void Heapify(int i)
		{
			if (HeapProperty == Type.MAX)
			{
				MaxHeapify(i);
			}
			else
			{
				MinHeapify(i);
			}
		}
		private void MaxHeapify(int i)
		{
			int l = Left(i);
			int r = Right(i);
			int largest;

			if (l <= Count && _nodes[l].CompareTo(_nodes[i]) > 0)
			{
				largest = l;
			}
			else
			{
				largest = i;
			}

			if (r <= Count && _nodes[largest].CompareTo(_nodes[r]) > 0)
			{
				largest = r;
			}

			if (largest != i)
			{
				Exchange(_nodes, i, largest);
				MaxHeapify(largest);
			}
		}
		private void MinHeapify(int i)
		{
			int l = Left(i);
			int r = Right(i);
			int smallest;

			if (l <= Count && _nodes[l].CompareTo(_nodes[i]) < 0)
			{
				smallest = l;
			}
			else
			{
				smallest = i;
			}

			if (r <= Count && _nodes[smallest].CompareTo(_nodes[r]) < 0)
			{
				smallest = r;
			}

			if (smallest != i)
			{
				Exchange(_nodes, i, smallest);
				MinHeapify(smallest);
			}
		}
		private void ChangeKey(int i, T key)
		{
			if (HeapProperty == Type.MAX)
			{
				_nodes[i] = key;
				while (i > 1 && _nodes[Parent(i)].CompareTo(_nodes[i]) < 0)
				{
					Exchange(_nodes, i, Parent(i));
					i = Parent(i);
				}
			}
			else
			{
				_nodes[i] = key;
				while (i > 1 && _nodes[Parent(i)].CompareTo(_nodes[i]) > 0)
				{
					Exchange(_nodes, i, Parent(i));
					i = Parent(i);
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Look at the first item in the PQ
		/// </summary>
		/// <returns>Either the Maximum or Minimum depending on the Type of the PQ</returns>
		public T Peek()
		{
			Heapify(1);
			return _nodes[1];
		}
		/// <summary>
		/// Inserts a new key into the PQ
		/// </summary>
		/// <param name="key">the key to insert</param>
		public void Insert(T key)
		{
			if (Count >= Capacity)
				IncreaseCapaticy();
			Count = Count + 1;

			_nodes[Count] = default(T);

			ChangeKey(Count, key);
		}
		/// <summary>
		/// Removes and returns the top item in the PQ
		/// </summary>
		/// <returns>Either the Maximum or Minimum depending on the Type of the PQ</returns>
		public T Extract()
		{
			Exchange(_nodes, 1, Count);
			Count = Count - 1;
			Heapify(1);
			return _nodes[Count + 1];
		}
		/// <summary>
		/// Sorts the PQ and returns an array
		/// </summary>
		/// <returns>Sorted array</returns>
		public T[] ToSortedArray()
		{
			T[] temp = new T[Count];
			int i = 0;
			int tempCount = Count;

			//Ensure that the heap property holds
			Heapify(1);

			//Extract until there is nothing left in the heap
			while(Count > 0)
			{
				temp[i] = Extract();
				i++;
			}

			//reset the heap
			Count = tempCount;
			Heapify(1);

			//return the sorted array
			return temp;
		}

		#endregion

		#region Generic Tool Methods

		private int Parent(int i)
		{
			return i / 2;
		}
		private int Left(int i)
		{
			return 2 * i;
		}
		private int Right(int i)
		{
			return (2 * i) + 1;
		}
		private void Exchange(T[] A, int i, int j)
		{
			if (i >= A.Length || j >= A.Length || i == 0 || j == 0)
			{
				throw new IndexOutOfRangeException("One of the values is out of range or equals 0");
			}
			else
			{
				A[0] = A[i];
				A[i] = A[j];
				A[j] = A[0];
			}
		}
		private void IncreaseCapaticy()
		{
			while (Capacity <= Count)
				Capacity = Capacity * 2;

			T[] temp = new T[Capacity];
			for(int i = 0; i < _nodes.Length; i++)
			{
				temp[i] = _nodes[i];
			}
			_nodes = temp;
		}

		#endregion

		#region ICollection Implementation

		public int Count { get; private set; }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		public void CopyTo(Array A, int i)
		{
			if (Count < (A.Length - i))
			{
				_nodes.CopyTo(A, i);
			}
		} 

		#endregion

		#region IEnumerator Implementation

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return (IEnumerator<T>)GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}
		public PQEnum<T> GetEnumerator()
		{
			return new PQEnum<T>(_nodes);
		}

		#endregion
	}

	public class PQEnum<T> : IEnumerator<T>
	{
		private T[] _nodes;
		private int position;
		private T currentObj;


		public PQEnum(T[] list)
		{
			_nodes = list;
			position = -1;
			currentObj = default(T);
		}

		public bool MoveNext()
		{
			if (++position >= _nodes.Length)
			{
				return false;
			}
			else
			{
				currentObj = _nodes[position];
				return true;
			}
		}

		public void Reset()
		{
			position = -1;
		}

		void IDisposable.Dispose() { }

		public T Current
		{
			get { return currentObj; }
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

	}
}
