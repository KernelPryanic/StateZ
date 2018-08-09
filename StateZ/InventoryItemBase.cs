using NativeUI;
using System;

namespace StateZ
{
	[Serializable]
	public class InventoryItemBase : IIdentifier
	{
		[NonSerialized]
		public UIMenuItem MenuItem;

		public int Amount
		{
			get;
			set;
		}

		public int MaxAmount
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public InventoryItemBase(int amount, int maxAmount, string id, string description)
		{
			Amount = amount;
			MaxAmount = maxAmount;
			Id = id;
			Description = description;
		}

		public void CreateMenuItem()
		{
			MenuItem = new UIMenuItem(Id, Description);
		}
	}
}
