using GTA;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StateZ
{
	[Serializable]
	public class Map : ICollection<MapProp>, IEnumerable<MapProp>, IEnumerable
	{
		public delegate void OnListChangedEvent(int count);

		public List<MapProp> Props;

		public int Count => Props.Count;

		public bool IsReadOnly
		{
			get;
		}

		[field: NonSerialized]
		public event OnListChangedEvent ListChanged;

		public Map()
		{
			Props = new List<MapProp>();
			IsReadOnly = false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<MapProp> GetEnumerator()
		{
			return Props.GetEnumerator();
		}

		public void Add(MapProp item)
		{
			Props.Add(item);
			this.ListChanged?.Invoke(Props.Count);
		}

		public void Clear()
		{
			while (Props.Count > 0)
			{
				MapProp prop = Props[0];
				prop.Delete();
				Props.Remove(prop);
			}
		}

		public bool Contains(MapProp item)
		{
			return Props.Contains(item);
		}

		public void CopyTo(MapProp[] array, int arrayIndex)
		{
			Props.CopyTo(array, arrayIndex);
		}

		public bool Remove(MapProp item)
		{
			if (Props.Remove(item))
			{
				this.ListChanged?.Invoke(Props.Count);
				return true;
			}
			return false;
		}

		public bool Contains(Prop prop)
		{
			return Props.Find((MapProp m) => m.Handle == prop.Handle) != null;
		}

		public void NotifyListChanged()
		{
			this.ListChanged?.Invoke(Count);
		}
	}
}
