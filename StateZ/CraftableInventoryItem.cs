using System;

namespace StateZ
{
	[Serializable]
	public class CraftableInventoryItem : InventoryItemBase, ICraftable, IValidatable
	{
		public CraftableItemComponent[] RequiredComponents
		{
			get;
			set;
		}

		public Func<bool> Validation
		{
			get;
			set;
		}

		public CraftableInventoryItem(int amount, int maxAmount, string id, string description, Func<bool> validation)
			: base(amount, maxAmount, id, description)
		{
			Validation = validation;
		}
	}
}
