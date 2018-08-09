using System;

namespace StateZ
{
	[Serializable]
	public class UsableInventoryItem : InventoryItemBase, ICraftable
	{
		public UsableItemEvent[] ItemEvents
		{
			get;
			set;
		}

		public CraftableItemComponent[] RequiredComponents
		{
			get;
			set;
		}

		public UsableInventoryItem(int amount, int maxAmount, string id, string description, UsableItemEvent[] itemEvents)
			: base(amount, maxAmount, id, description)
		{
			ItemEvents = itemEvents;
		}
	}
}
