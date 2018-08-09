using System;
using System.Collections;
using System.Collections.Generic;

namespace StateZ
{
	[Serializable]
	public class VehicleCollection : IList<VehicleData>, ICollection<VehicleData>, IEnumerable<VehicleData>, IEnumerable
	{
		public delegate void ListChangedEvent(VehicleCollection sender);

		private readonly List<VehicleData> _vehicles;

		public int Count => _vehicles.Count;

		public bool IsReadOnly => ((ICollection<VehicleData>)_vehicles).IsReadOnly;

		public VehicleData this[int index]
		{
			get
			{
				return _vehicles[index];
			}
			set
			{
				_vehicles[index] = value;
			}
		}

		[field: NonSerialized]
		public event ListChangedEvent ListChanged;

		public VehicleCollection()
		{
			_vehicles = new List<VehicleData>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<VehicleData> GetEnumerator()
		{
			return _vehicles.GetEnumerator();
		}

		public void Add(VehicleData item)
		{
			_vehicles.Add(item);
			this.ListChanged?.Invoke(this);
		}

		public void Clear()
		{
			_vehicles.Clear();
			this.ListChanged?.Invoke(this);
		}

		public bool Contains(VehicleData item)
		{
			return _vehicles.Contains(item);
		}

		public void CopyTo(VehicleData[] array, int arrayIndex)
		{
			_vehicles.CopyTo(array, arrayIndex);
		}

		public bool Remove(VehicleData item)
		{
			bool remove = _vehicles.Remove(item);
			this.ListChanged?.Invoke(this);
			return remove;
		}

		public int IndexOf(VehicleData item)
		{
			return _vehicles.IndexOf(item);
		}

		public void Insert(int index, VehicleData item)
		{
			_vehicles.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_vehicles.RemoveAt(index);
		}
	}
}
