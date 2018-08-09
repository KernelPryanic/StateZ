using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using StateZ.Extensions;
using StateZ.PlayerManagement;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.Scripts
{
	public class MapInteraction : Script
	{
		private const int AmmoPerPart = 10;

		private readonly float _enemyRangeForSleeping = 50f;

		private readonly int _sleepHours = 8;

		private readonly UIMenu _weaponsMenu = new UIMenu("Weapon Crate", "SELECT AN OPTION");

		private readonly UIMenu _storageMenu;

		private readonly UIMenu _myWeaponsMenu;

		private readonly UIMenu _craftWeaponsMenu = new UIMenu("Work Bench", "SELECT AN OPTION");

		private readonly Dictionary<WeaponGroup, int> _requiredAmountDictionary;

		private static Ped PlayerPed => Database.PlayerPed;

		private static Player Player => Database.Player;

		public MapInteraction()
		{
			PlayerMap.Interacted += MapOnInteracted;
			MenuConrtoller.MenuPool.Add(_weaponsMenu);
			MenuConrtoller.MenuPool.Add(_craftWeaponsMenu);
			_storageMenu = MenuConrtoller.MenuPool.AddSubMenu(_weaponsMenu, "Storage");
			_myWeaponsMenu = MenuConrtoller.MenuPool.AddSubMenu(_weaponsMenu, "Give");
			_enemyRangeForSleeping = this.Settings.GetValue<float>("map_interaction", "enemy_range_for_sleeping", _enemyRangeForSleeping);
			_sleepHours = this.Settings.GetValue<int>("map_interaction", "sleep_hours", _sleepHours);
			this.Settings.SetValue<float>("map_interaction", "enemy_range_for_sleeping", _enemyRangeForSleeping);
			this.Settings.SetValue<int>("map_interaction", "sleep_hours", _sleepHours);
			_requiredAmountDictionary = new Dictionary<WeaponGroup, int>
			{
				{
                    GTA.WeaponGroup.AssaultRifle,
					2
				},
				{
					GTA.WeaponGroup.Shotgun,
					5
				},
				{
                    GTA.WeaponGroup.MG,
					3
				},
				{
                    GTA.WeaponGroup.PetrolCan,
					1
				}
			};
			this.Aborted += (EventHandler)OnAborted;
		}

		private static void OnAborted(object sender, EventArgs eventArgs)
		{
			PlayerPed.IsVisible = true;
			PlayerPed.FreezePosition = false;
			Player.CanControlCharacter = true;
			if (!PlayerPed.IsDead)
			{
				Game.FadeScreenIn(0);
			}
		}

		private void MapOnInteracted(MapProp mapProp, InventoryItemBase inventoryItem)
		{
			BuildableInventoryItem item = inventoryItem as BuildableInventoryItem;
			if (item != null)
			{
				switch (item.Id)
				{
				case "Tent":
					Sleep(mapProp.Position);
					break;
				case "Weapons Crate":
					UseWeaponsCrate(mapProp);
					break;
				case "Work Bench":
					CraftAmmo();
					break;
				}
				if (item.IsDoor)
				{
					Prop prop = new Prop(mapProp.Handle);
					prop.SetStateOfDoor(!prop.GetDoorLockState(), DoorState.Closed);
				}
			}
		}

		private void CraftAmmo()
		{
			_craftWeaponsMenu.Clear();
			WeaponGroup[] weaponGroups3 = (WeaponGroup[])Enum.GetValues(typeof(WeaponGroup));
			weaponGroups3 = (from w in weaponGroups3
			where (int)w != 1595662460 && (int)w != -1609580060 && (int)w != -728555052 && (int)w != -942583726
			select w).ToArray();
			List<WeaponGroup> list = weaponGroups3.ToList();
			list.Add(GTA.WeaponGroup.AssaultRifle);
            weaponGroups3 = list.ToArray();
			WeaponGroup[] array = weaponGroups3;
			for (int i = 0; i < array.Length; i++)
			{
				WeaponGroup weaponGroup = array[i];
				UIMenuItem menuItem = new UIMenuItem(string.Format("{0}", ((int)weaponGroup == 970310034) ? "Assult Rifle" : ((object)weaponGroup).ToString()), $"Craft ammo for {weaponGroup}");
				menuItem.SetLeftBadge(UIMenuItem.BadgeStyle.Ammo);
				int required = GetRequiredPartsForWeaponGroup(weaponGroup);
				menuItem.Description = $"Required Weapon Parts: ~y~{required}~s~";
				_craftWeaponsMenu.AddItem(menuItem);
				menuItem.Activated += delegate
				{
					InventoryItemBase inventoryItemBase = PlayerInventory.Instance.ItemFromName("Weapon Parts");
					if (inventoryItemBase != null)
					{
						if (inventoryItemBase.Amount >= required)
						{
							WeaponHash[] array2 = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
							WeaponHash val = Array.Find(array2, (WeaponHash h) => PlayerPed.Weapons.HasWeapon(h) && PlayerPed.Weapons[h].Group == weaponGroup);
							GTA.Weapon val2 = PlayerPed.Weapons[val];
							if (val2 != null)
							{
								int num = 10 * required;
								if (val2.Ammo + num <= val2.MaxAmmo)
								{
									PlayerPed.Weapons.Select(val2);
									if (val2.Ammo + num > val2.MaxAmmo)
									{
										val2.Ammo = val2.MaxAmmo;
									}
									else
									{
										GTA.Weapon obj = val2;
										obj.Ammo = obj.Ammo + num;
									}
									PlayerInventory.Instance.AddItem(inventoryItemBase, -required, ItemType.Resource);
								}
							}
						}
						else
						{
							UI.Notify("Not enough weapon parts.");
						}
					}
				};
			}
			_craftWeaponsMenu.Visible = !_craftWeaponsMenu.Visible;
		}

		private int GetRequiredPartsForWeaponGroup(WeaponGroup group)
		{
			return (!_requiredAmountDictionary.ContainsKey(group)) ? 1 : _requiredAmountDictionary[group];
		}

		private void UseWeaponsCrate(MapProp prop)
		{
			if (prop?.Weapons != null)
			{
				_weaponsMenu.OnMenuChange += delegate(UIMenu oldMenu, UIMenu newMenu, bool forward)
				{
					if (newMenu == _storageMenu)
					{
						TradeOffWeapons(prop, prop.Weapons, _storageMenu, true);
					}
					else if (newMenu == _myWeaponsMenu)
					{
						List<Weapon> playerWeapons = new List<Weapon>();
						WeaponHash[] source = (WeaponHash[])Enum.GetValues(typeof(WeaponHash));
						WeaponComponent[] weaponComponents = (WeaponComponent[])Enum.GetValues(typeof(WeaponComponent));
						source.ToList().ForEach(delegate(WeaponHash hash)
						{
							if ((int)hash != -1569615261 && PlayerPed.Weapons.HasWeapon(hash))
							{
								GTA.Weapon val = PlayerPed.Weapons[hash];
								WeaponComponent[] components = (from c in weaponComponents
								where PlayerPed.Weapons[hash].IsComponentActive(c)
								select c).ToArray();
								Weapon item = new Weapon(val.Ammo, hash, components);
								playerWeapons.Add(item);
							}
						});
						TradeOffWeapons(prop, playerWeapons, _myWeaponsMenu, false);
					}
				};
				_weaponsMenu.Visible = !_weaponsMenu.Visible;
			}
		}

		private static void TradeOffWeapons(MapProp item, List<Weapon> weapons, UIMenu currentMenu, bool giveToPlayer)
		{
			UIMenuItem back = new UIMenuItem("Back");
			back.Activated += delegate(UIMenu sender, UIMenuItem selectedItem)
			{
				sender.GoBack();
			};
			currentMenu.Clear();
			currentMenu.AddItem(back);
			Action notify = delegate
			{
				PlayerMap.Instance.NotifyListChanged();
			};
			weapons.ForEach(delegate(Weapon weapon)
			{
				UIMenuItem uIMenuItem = new UIMenuItem($"{(object)weapon.Hash}");
				currentMenu.AddItem(uIMenuItem);
				uIMenuItem.Activated += delegate
				{
					currentMenu.RemoveItemAt(currentMenu.CurrentSelection);
					currentMenu.RefreshIndex();
					if (giveToPlayer)
					{
						PlayerPed.Weapons.Give(weapon.Hash, 0, true, true);
						PlayerPed.Weapons[weapon.Hash].Ammo = weapon.Ammo;
						weapon.Components.ToList().ForEach(delegate(WeaponComponent component)
						{
							PlayerPed.Weapons[weapon.Hash].SetComponent(component, true);
						});
						item.Weapons.Remove(weapon);
						notify();
					}
					else
					{
						PlayerPed.Weapons.Remove(weapon.Hash);
						item.Weapons.Add(weapon);
						notify();
					}
				};
			});
			currentMenu.RefreshIndex();
		}

		private void Sleep(Vector3 position)
		{
			Ped[] peds = World.GetNearbyPeds(position, _enemyRangeForSleeping).Where(IsEnemy).ToArray();
			if (!peds.Any())
			{
				TimeSpan time = World.CurrentDayTime + new TimeSpan(0, _sleepHours, 0, 0);
				PlayerPed.IsVisible = false;
				Player.CanControlCharacter = false;
				PlayerPed.FreezePosition = true;
				Game.FadeScreenOut(2000);
				Script.Wait(2000);
				World.CurrentDayTime = time;
				PlayerPed.IsVisible = true;
                Player.CanControlCharacter = true;
				PlayerPed.FreezePosition = false;
				PlayerPed.ClearBloodDamage();
				Weather[] weatherTypes2 = (Weather[])Enum.GetValues(typeof(Weather));
				weatherTypes2 = (from w in weatherTypes2
				where (int)w != 11 && (int)w != 13 && (int)w != 10 && (int)w != 12 && (int)w != -1
				select w).ToArray();
				Weather randWeather = weatherTypes2[Database.Random.Next(weatherTypes2.Length)];
				World.Weather = randWeather;
				Script.Wait(2000);
				Game.FadeScreenIn(2000);
			}
			else
			{
				UI.Notify("There are ~r~enemies~s~ nearby.");
				UI.Notify("Marking them on your map.");
				Array.ForEach(peds, AddBlip);
			}
		}

		private static void AddBlip(Ped ped)
		{
			if (!ped.CurrentBlip.Exists())
			{
				Blip blip = ped.AddBlip();
				blip.Name = "Enemy Ped";
				EntityEventWrapper entWrapper = new EntityEventWrapper(ped);
				entWrapper.Died += delegate(EntityEventWrapper sender, Entity entity)
				{
					Blip currentBlip2 = entity.CurrentBlip;
					if (currentBlip2.Handle != 0)
					{
						currentBlip2.Remove();
					}
					sender.Dispose();
				};
				entWrapper.Aborted += delegate(EntityEventWrapper sender, Entity entity)
				{
					Blip currentBlip = entity.CurrentBlip;
					if (currentBlip.Handle != 0)
					{
						currentBlip.Remove();
					}
				};
			}
		}

		private static bool IsEnemy(Ped ped)
		{
			return (ped.IsHuman && !ped.IsDead && (int)ped.GetRelationshipWithPed(PlayerPed) == 5) || ped.IsInCombatAgainst(PlayerPed);
		}
	}
}
