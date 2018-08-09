using GTA.Math;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StateZ.DataClasses
{
	public class SpawnBlocker : IList<Vector3>, ICollection<Vector3>, IEnumerable<Vector3>, IEnumerable
	{
		private readonly List<Vector3> _blockers = new List<Vector3>();

		public int Count => _blockers.Count;

		public bool IsReadOnly => ((ICollection<Vector3>)_blockers).IsReadOnly;

		public Vector3 this[int index]
		{
			get
			{
				return _blockers[index];
			}
			set
			{
				_blockers[index] = value;
			}
		}

		public IEnumerator<Vector3> GetEnumerator()
		{
			return _blockers.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_blockers).GetEnumerator();
		}

		public void Add(Vector3 item)
		{
			_blockers.Add(item);
		}

		public void Clear()
		{
			_blockers.Clear();
		}

		public bool Contains(Vector3 item)
		{
			return _blockers.Contains(item);
		}

		public void CopyTo(Vector3[] array, int arrayIndex)
		{
			_blockers.CopyTo(array, arrayIndex);
		}

		public bool Remove(Vector3 item)
		{
			return _blockers.Remove(item);
		}

		public int IndexOf(Vector3 item)
		{
			return _blockers.IndexOf(item);
		}

		public void Insert(int index, Vector3 item)
		{
			_blockers.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_blockers.RemoveAt(index);
		}

		public int FindIndex(Predicate<Vector3> match)
		{
			if (match != null)
			{
				for (int i = 0; i < Count; i++)
				{
					if (match(this[i]))
					{
						return i;
					}
				}
				return -1;
			}
			return -1;
		}
	}
}
