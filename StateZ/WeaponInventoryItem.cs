using GTA.Native;
using System;

namespace StateZ
{
	[Serializable]
	public class WeaponInventoryItem : InventoryItemBase, IWeapon, ICraftable
	{
		public int Ammo
		{
			get;
			set;
		}

		public WeaponHash Hash
		{
			get;
			set;
		}

		public CraftableItemComponent[] RequiredComponents
		{
			get;
			set;
		}

		public WeaponComponent[] Components
		{
			get;
			set;
		}

		public WeaponInventoryItem(int amount, int maxAmount, string id, string description, int ammo, WeaponHash weaponHash, WeaponComponent[] weaponComponents)
			: base(amount, maxAmount, id, description)
		{
			Ammo = ammo;
			Hash = weaponHash;
			Components = weaponComponents;
		}
	}
}
