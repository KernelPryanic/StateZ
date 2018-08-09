using System;

namespace StateZ
{
	[Serializable]
	public class CraftableItemComponent
	{
		public InventoryItemBase Resource
		{
			get;
			set;
		}

		public int RequiredAmount
		{
			get;
			set;
		}

		public CraftableItemComponent(InventoryItemBase resource, int requiredAmount)
		{
			Resource = resource;
			RequiredAmount = requiredAmount;
		}
	}
}
