using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using StateZ.Extensions;
using StateZ.PlayerManagement;
using StateZ.Static;

namespace StateZ.Scripts
{
	public class Loot247 : Script, ISpawner
	{
		public const float InteractDistance = 1.5f;

		private readonly List<Blip> _blips = new List<Blip>();

		private readonly List<Prop> _lootedShelfes = new List<Prop>();

		private readonly int[] _propHashes;

		public static Loot247 Instance
		{
			get;
			private set;
		}

		public bool Spawn
		{
			get;
			set;
		}

		private static Vector3 PlayerPosition => Database.PlayerPosition;

		private static Ped PlayerPed => Database.PlayerPed;

		public Loot247()
		{
			Instance = this;
			_propHashes = new int[5]
			{
				Game.GenerateHash("v_ret_247shelves01"),
				Game.GenerateHash("v_ret_247shelves02"),
				Game.GenerateHash("v_ret_247shelves03"),
				Game.GenerateHash("v_ret_247shelves04"),
				Game.GenerateHash("v_ret_247shelves05")
			};
			this.Tick += (EventHandler)OnTick;
			this.Aborted += ((EventHandler)delegate
			{
				Clear();
			});
		}

		private void OnTick(object sender, EventArgs e)
		{
			SpawnBlips();
			ClearBlips();
			LootShops();
		}

		private void LootShops()
		{
			if (Spawn && !PlayerPed.IsPlayingAnim("oddjobs@shop_robbery@rob_till", "loop"))
			{
				IEnumerable<Prop> props = World.GetNearbyProps(PlayerPosition, 15f).Where(IsShelf);
				Prop closest = World.GetClosest<Prop>(PlayerPosition, props.ToArray());
				if (!(closest == null))
				{
					float dist = closest.Position.VDist(PlayerPosition);
					if (!(dist > 1.5f))
					{
						Game.DisableControlThisFrame(2, GTA.Control.Context);
						UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to loot the shelf.", false);
						if (Game.IsDisabledControlJustPressed(2, GTA.Control.Context))
						{
							_lootedShelfes.Add(closest);
							bool rand = Database.Random.NextDouble() > 0.30000001192092896;
							PlayerInventory.Instance.PickupItem(rand ? PlayerInventory.Instance.ItemFromName("Packaged Food") : PlayerInventory.Instance.ItemFromName("Clean Water"), ItemType.Item);
							PlayerPed.Task.PlayAnimation("oddjobs@shop_robbery@rob_till", "loop");
							Ped playerPed = new Ped(PlayerPed.Handle);
							Vector3 val = closest.Position - PlayerPosition;
							playerPed.Heading = val.ToHeading();
						}
					}
				}
			}
		}

		private bool IsShelf(Prop arg)
		{
			int[] propHashes = _propHashes;
			Model model = arg.Model;
			return propHashes.Contains(model.Hash) && !_lootedShelfes.Contains(arg);
		}

		private void ClearBlips()
		{
			if (!Spawn)
			{
				Clear();
			}
		}

		private void SpawnBlips()
		{
			if (Spawn && _blips.Count < Database.Shops247Locations.Length)
			{
				Vector3[] shops247Locations = Database.Shops247Locations;
				foreach (Vector3 location in shops247Locations)
				{
					Blip blip = World.CreateBlip(location);
					blip.Sprite = GTA.BlipSprite.Store;
					blip.Name = "Store";
					blip.IsShortRange = true;
					_blips.Add(blip);
				}
			}
		}

		public void Clear()
		{
			while (_blips.Count > 0)
			{
				Blip blip = _blips[0];
				blip.Remove();
				_blips.RemoveAt(0);
			}
		}
	}
}
