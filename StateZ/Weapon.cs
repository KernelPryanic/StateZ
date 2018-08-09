using GTA.Native;
using System;

namespace StateZ
{
	[Serializable]
	public class Weapon : IWeapon
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

		public WeaponComponent[] Components
		{
			get;
			set;
		}

		public Weapon(int ammo, WeaponHash hash, WeaponComponent[] components)
		{
			Ammo = ammo;
			Hash = hash;
			Components = components;
		}
	}
}
