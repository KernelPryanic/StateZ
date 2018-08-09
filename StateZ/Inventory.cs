using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using StateZ.Static;

namespace StateZ
{
	[Serializable]
	public class Inventory
	{
		public delegate void CraftedItemEvent(InventoryItemBase item);

		public delegate void ItemUsedEvent(InventoryItemBase item, ItemType type);

		public delegate void AddedItemEvent(InventoryItemBase item, int newAmount);

		public const float InteractDist = 2.5f;

		public List<InventoryItemBase> Items = new List<InventoryItemBase>();

		public List<InventoryItemBase> Resources = new List<InventoryItemBase>();

		[NonSerialized]
		public readonly MenuType MenuType;

		[NonSerialized]
		public UIMenu InventoryMenu;

		[NonSerialized]
		public UIMenu ResourceMenu;

		private static Vector3 PlayerPosition => Database.PlayerPosition;

		private static Ped PlayerPed => Database.PlayerPed;

		public bool DeveloperMode
		{
			get;
			set;
		}

		[field: NonSerialized]
		public event CraftedItemEvent TryCraft;

		[field: NonSerialized]
		public event ItemUsedEvent ItemUsed;

		[field: NonSerialized]
		public event AddedItemEvent AddedItem;

		public Inventory(MenuType menuType, bool ignoreContainers = false)
		{
			MenuType = menuType;
			InventoryItemBase alcohol = new InventoryItemBase(0, 20, "Alcohol", "A colorless volatile flammable liquid.");
			InventoryItemBase battery = new InventoryItemBase(0, 25, "Battery", "A resource that can provide an electrical charge.");
			InventoryItemBase binding = new InventoryItemBase(0, 25, "Binding", "A strong adhesive.");
			InventoryItemBase bottle = new InventoryItemBase(0, 10, "Bottle", "A container used for storing drinks or other liquids..");
			InventoryItemBase cloth = new InventoryItemBase(0, 25, "Cloth", "Woven or felted fabric.");
			InventoryItemBase dirtyWater = new InventoryItemBase(0, 25, "Dirty Water", "Liquid obtained from an undrinkable source of water.");
			InventoryItemBase metal = new InventoryItemBase(0, 25, "Metal", "It's freaking metal.");
			InventoryItemBase wood = new InventoryItemBase(0, 25, "Wood", "It's freaking wood.");
			InventoryItemBase plastic = new InventoryItemBase(0, 25, "Plastic", "A synthetic material made from a wide range of organic polymers.");
			InventoryItemBase rawMeat = new InventoryItemBase(0, 15, "Raw Meat", "Can be cooked to create ~g~Cooked Meat~s~.");
			InventoryItemBase matches = new InventoryItemBase(0, 10, "Matches", "Can be used to create fire.");
			InventoryItemBase weaponParts = new InventoryItemBase(25, 25, "Weapon Parts", "Used to create weapon components, and weapons. (Weapons crafting coming soon)");
			InventoryItemBase vehicleParts = new InventoryItemBase(0, 25, "Vehicle Parts", "USed to repair vehicles.");
			UsableInventoryItem bandage = new UsableInventoryItem(0, 10, "Bandage", "A strip of material used to bind a wound or to protect an injured part of the body.", new UsableItemEvent[2]
			{
				new UsableItemEvent(ItemEvent.GiveHealth, 25),
				new UsableItemEvent(ItemEvent.GiveArmor, 15)
			})
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(binding, 1),
					new CraftableItemComponent(alcohol, 2),
					new CraftableItemComponent(cloth, 2)
				}
			};
			CraftableInventoryItem supp = new CraftableInventoryItem(0, 5, "Suppressor", "Can be used to suppress a rifle, pistol, shotgun, or SMG.", delegate
			{
				GTA.Weapon val = PlayerPed.Weapons.Current;
				if ((int)val.Hash != -1569615261)
				{
					WeaponComponent[] obj = new WeaponComponent[6];
                    obj[0] = WeaponComponent.AtArSupp;
                    obj[1] = WeaponComponent.AtArSupp02;
                    obj[2] = WeaponComponent.AtPiSupp;
                    obj[3] = WeaponComponent.AtPiSupp02;
                    obj[4] = WeaponComponent.AtSrSupp;
                    obj[5] = WeaponComponent.AtSrSupp03;
					WeaponComponent[] array = (WeaponComponent[])obj;
					WeaponComponent[] array2 = array;
					foreach (WeaponComponent val2 in array2)
					{
						if (Function.Call<bool>(GTA.Native.Hash._CAN_WEAPON_HAVE_COMPONENT, (InputArgument[])new InputArgument[2]
						{
							(uint)(int)val.Hash,
							(uint)(int)val2
						}) && !val.IsComponentActive(val2))
						{
							val.SetComponent(val2, true);
							return true;
						}
					}
					UI.Notify("You can't equip this right now.");
					return false;
				}
				UI.Notify("No weapon selected!");
				return false;
			})
			{
				RequiredComponents = new CraftableItemComponent[2]
				{
					new CraftableItemComponent(weaponParts, 2),
					new CraftableItemComponent(binding, 1)
				}
			};
			BuildableInventoryItem sandbag = new BuildableInventoryItem(0, 5, "Sand Block", "Used to provide cover in combat situations", "prop_mb_sandblock_02", GTA.BlipSprite.Standard, GTA.BlipColor.White, Vector3.Zero, false, false, true)
			{
				RequiredComponents = new CraftableItemComponent[4]
				{
					new CraftableItemComponent(metal, 3),
					new CraftableItemComponent(binding, 2),
					new CraftableItemComponent(cloth, 1),
					new CraftableItemComponent(wood, 2)
				}
			};
			BuildableInventoryItem workbench = new BuildableInventoryItem(0, 2, "Work Bench", "Can be used to craft ammunition.", "prop_tool_bench02", GTA.BlipSprite.AmmuNation, GTA.BlipColor.Yellow, Vector3.Zero, true, false, true)
			{
				RequiredComponents = new CraftableItemComponent[4]
				{
					new CraftableItemComponent(metal, 15),
					new CraftableItemComponent(wood, 5),
					new CraftableItemComponent(plastic, 5),
					new CraftableItemComponent(binding, 5)
				}
			};
			BuildableInventoryItem gate = new BuildableInventoryItem(0, 3, "Gate", "A metal gate that can be opened by vehicles or peds.", "prop_gate_prison_01", GTA.BlipSprite.Standard, GTA.BlipColor.White, Vector3.Zero, true, true, true)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(metal, 5),
					new CraftableItemComponent(binding, 3),
					new CraftableItemComponent(battery, 1)
				}
			};
			WeaponInventoryItem molotov = new WeaponInventoryItem(0, 25, "Molotov", "A bottle-based improvised incendiary weapon.", 1, GTA.Native.WeaponHash.Molotov, null)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(alcohol, 1),
					new CraftableItemComponent(cloth, 1),
					new CraftableItemComponent(bottle, 1)
				}
			};
			WeaponInventoryItem knife = new WeaponInventoryItem(0, 1, "Knife", "An improvised knife.", 1, GTA.Native.WeaponHash.Knife, null)
			{
				RequiredComponents = new CraftableItemComponent[2]
				{
					new CraftableItemComponent(metal, 3),
					new CraftableItemComponent(binding, 1)
				}
			};
			WeaponInventoryItem flashlight = new WeaponInventoryItem(0, 5, "Flashlight", "A battery-operated portable light.", 1, GTA.Native.WeaponHash.Flashlight, null)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(battery, 4),
					new CraftableItemComponent(plastic, 4),
					new CraftableItemComponent(binding, 4)
				}
			};
			FoodInventoryItem cookedMeat = new FoodInventoryItem(0, 15, "Cooked Meat", "Can be creating from cooking raw meat.", "mp_player_inteat@burger", "mp_player_int_eat_burger", GTA.AnimationFlags.UpperBodyOnly, -1, FoodType.Food, 0.25f)
			{
				RequiredComponents = new CraftableItemComponent[2]
				{
					new CraftableItemComponent(rawMeat, 1),
					new CraftableItemComponent(alcohol, 2)
				},
				NearbyResource = NearbyResource.CampFire
			};
			FoodInventoryItem packagedFood = new FoodInventoryItem(0, 15, "Packaged Food", "Usually obtained from stores around Los Santos.", "mp_player_inteat@pnq", "loop", GTA.AnimationFlags.UpperBodyOnly, -1, FoodType.SpecialFood, 0.3f);
			FoodInventoryItem water = new FoodInventoryItem(0, 15, "Clean Water", "Can be made from dirty water or obtained from stores around Los Santos.", "mp_player_intdrink", "loop_bottle", GTA.AnimationFlags.UpperBodyOnly, -1, FoodType.Water, 0.35f)
			{
				RequiredComponents = new CraftableItemComponent[1]
				{
					new CraftableItemComponent(dirtyWater, 1)
				},
				NearbyResource = NearbyResource.CampFire
			};
			BuildableInventoryItem tent = new BuildableInventoryItem(1, 5, "Tent", "A portable shelter made of cloth, supported by one or more poles and stretched tight by cords or loops attached to pegs driven into the ground.", "prop_skid_tent_01", GTA.BlipSprite.CaptureHouse, 0, Vector3.Zero, true, false, true)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(wood, 3),
					new CraftableItemComponent(cloth, 4),
					new CraftableItemComponent(binding, 3)
				}
			};
			BuildableInventoryItem campFire = new BuildableInventoryItem(1, 5, "Camp Fire", "An open-air fire in a camp, used for cooking and as a focal point for social activity.", "prop_beach_fire", GTA.BlipSprite.Standard, 0, Vector3.Zero, false, false, true)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(wood, 3),
					new CraftableItemComponent(alcohol, 1),
					new CraftableItemComponent(matches, 1)
				}
			};
			BuildableInventoryItem wall = new BuildableInventoryItem(0, 15, "Wall", "A wooden wall that can be used for creating shelters.", "prop_fncconstruc_01d", GTA.BlipSprite.Standard, 0, Vector3.Zero, false, false, true)
			{
				RequiredComponents = new CraftableItemComponent[2]
				{
					new CraftableItemComponent(wood, 4),
					new CraftableItemComponent(binding, 3)
				}
			};
			BuildableInventoryItem barrier = new BuildableInventoryItem(0, 10, "Barrier", "A wooden barrier that can be used to barricade gaps in your safe zone.", "prop_fncwood_16b", GTA.BlipSprite.Standard, 0, Vector3.Zero, false, false, true)
			{
				RequiredComponents = new CraftableItemComponent[2]
				{
					new CraftableItemComponent(wood, 5),
					new CraftableItemComponent(binding, 2)
				}
			};
			BuildableInventoryItem door = new BuildableInventoryItem(0, 5, "Door", "A  hinged, sliding, or revolving barrier at the entrance to a building, room, or vehicle, or in the framework of a cupboard.", "ex_p_mp_door_office_door01", GTA.BlipSprite.Standard, 0, Vector3.Zero, true, true, true)
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(wood, 3),
					new CraftableItemComponent(binding, 1),
					new CraftableItemComponent(metal, 1)
				}
			};
			CraftableInventoryItem repairKit = new CraftableInventoryItem(0, 10, "Vehicle Repair Kit", "Used to repair vehicle engines.", delegate
			{
				UI.Notify("You can only use this when repairing a vehicle.");
				return false;
			})
			{
				RequiredComponents = new CraftableItemComponent[3]
				{
					new CraftableItemComponent(vehicleParts, 5),
					new CraftableItemComponent(metal, 5),
					new CraftableItemComponent(binding, 2)
				}
			};
			Items.AddRange(new InventoryItemBase[17]
			{
				bandage,
				molotov,
				knife,
				flashlight,
				cookedMeat,
				packagedFood,
				water,
				tent,
				campFire,
				wall,
				barrier,
				door,
				supp,
				gate,
				sandbag,
				workbench,
				repairKit
			});
			Resources.AddRange(new InventoryItemBase[13]
			{
				binding,
				alcohol,
				cloth,
				bottle,
				metal,
				wood,
				battery,
				plastic,
				rawMeat,
				dirtyWater,
				matches,
				weaponParts,
				vehicleParts
			});
			Items.Sort((InventoryItemBase c1, InventoryItemBase c2) => string.Compare(c1.Id, c2.Id, StringComparison.Ordinal));
			Resources.Sort((InventoryItemBase c1, InventoryItemBase c2) => string.Compare(c1.Id, c2.Id, StringComparison.Ordinal));
			LoadMenus();
			if (!ignoreContainers)
			{
				WeaponStorageInventoryItem weaponCrate = new WeaponStorageInventoryItem(0, 1, "Weapons Crate", "A crate specifically used to store weapons.", "hei_prop_carrier_crate_01a", GTA.BlipSprite.AssaultRifle, 0, Vector3.Zero, true, false, true, new List<Weapon>())
				{
					RequiredComponents = new CraftableItemComponent[4]
					{
						new CraftableItemComponent(metal, 15),
						new CraftableItemComponent(wood, 15),
						new CraftableItemComponent(plastic, 15),
						new CraftableItemComponent(binding, 10)
					}
				};
				Items.Add(weaponCrate);
			}
		}

		public void LoadMenus()
		{
			InventoryMenu = new NativeUI.UIMenu("Inventory", "SELECT AN ITEM");
			ResourceMenu = new NativeUI.UIMenu("Resources", "SELECT AN ITEM");
			AddItemsToMenu(InventoryMenu, Items, ItemType.Item);
			AddItemsToMenu(ResourceMenu, Resources, ItemType.Resource);
			MenuConrtoller.MenuPool.Add(InventoryMenu);
			MenuConrtoller.MenuPool.Add(ResourceMenu);
			RefreshMenu();
			InventoryMenu.RefreshIndex();
			ResourceMenu.RefreshIndex();
			if (MenuType == MenuType.Player)
			{
				InventoryMenu.AddInstructionalButton(new InstructionalButton(GTA.Control.Enter, "Blueprint"));
				InventoryMenu.AddInstructionalButton(new InstructionalButton(GTA.Control.LookBehind, "Craft"));
			}
		}

		public void RefreshMenu()
		{
			UpdateMenuSpecific(InventoryMenu, Items, MenuType == MenuType.Player);
			UpdateMenuSpecific(ResourceMenu, Resources, false);
		}

		public void ProcessKeys()
		{
			if (MenuType == MenuType.Player && InventoryMenu.Visible)
			{
				Game.DisableControlThisFrame(2, GTA.Control.Enter);
				Game.DisableControlThisFrame(2, GTA.Control.VehicleExit);
				Game.DisableControlThisFrame(2, GTA.Control.LookBehind);
				if (Game.IsDisabledControlJustPressed(2, GTA.Control.Enter))
				{
					ICraftable craftable = GetSelectedInventoryItem<ICraftable>();
					if (craftable != null)
					{
						StringBuilder str = new StringBuilder("Blueprint:\n");
						if (craftable.RequiredComponents != null && (craftable.RequiredComponents.Any() || DeveloperMode))
						{
							Array.ForEach(craftable.RequiredComponents, delegate(CraftableItemComponent i)
							{
								str.Append(string.Format("{0}{1}~s~ / {2} {3}\n", (i.Resource.Amount >= i.RequiredAmount) ? "~g~" : "~r~", i.Resource.Amount, i.RequiredAmount, i.Resource.Id));
							});
							UI.Notify(str.ToString());
						}
					}
				}
				else if (Game.IsDisabledControlJustPressed(2, GTA.Control.LookBehind))
				{
					InventoryItemBase item = GetSelectedInventoryItem<InventoryItemBase>();
					ICraftable craftableItem = item as ICraftable;
					if (item == null)
					{
						throw new NullReferenceException("item");
					}
					if (craftableItem != null)
					{
						Craft(item, craftableItem);
					}
				}
			}
		}

		public bool AddItem(InventoryItemBase item, int amount, ItemType type)
		{
			if (!DeveloperMode)
			{
				if (item.Amount + amount < 0)
				{
					return false;
				}
				if (item.Amount + amount > item.MaxAmount)
				{
					UI.Notify($"There's not enough room for anymore ~g~{item.Id}s~s~.", true);
					return false;
				}
			}
			item.Amount += amount;
			switch (type)
			{
			case ItemType.Item:
				UpdateMenuSpecific(InventoryMenu, Items, MenuType == MenuType.Player);
				break;
			case ItemType.Resource:
				UpdateMenuSpecific(ResourceMenu, Resources, false);
				break;
			}
			RefreshMenu();
			this.AddedItem?.Invoke(item, item.Amount);
			return true;
		}

		private void Craft(InventoryItemBase item, ICraftable craftable)
		{
			if (MenuType == MenuType.Player && item != null && craftable != null && (DeveloperMode || (CanCraftItem(craftable) && item.Amount < item.MaxAmount)))
			{
				FoodInventoryItem food = item as FoodInventoryItem;
				if (!DeveloperMode)
				{
					NearbyResource? nearby = food?.NearbyResource;
					NearbyResource? nullable = nearby;
					NearbyResource? nullable2 = nullable;
					if (nullable2.HasValue)
					{
						NearbyResource valueOrDefault = nullable2.GetValueOrDefault();
						if (valueOrDefault == NearbyResource.CampFire)
						{
							Prop[] props = World.GetNearbyProps(PlayerPosition, 2.5f, "prop_beach_fire");
							if (!props.Any())
							{
								UI.Notify("There's no ~g~Camp Fire~s~ nearby.");
								return;
							}
						}
					}
				}
				AddItem(item, 1, ItemType.Item);
				Array.ForEach(craftable.RequiredComponents, delegate(CraftableItemComponent c)
				{
					AddItem(c.Resource, -c.RequiredAmount, ItemType.Resource);
				});
				this.TryCraft?.Invoke(item);
			}
		}

		private void AddItemsToMenu(UIMenu menu, List<InventoryItemBase> items, ItemType type)
		{
			items.ForEach(delegate(InventoryItemBase i)
			{
				i.CreateMenuItem();
				menu.AddItem(i.MenuItem);
				i.MenuItem.Activated += delegate
				{
					WeaponInventoryItem weaponInventoryItem = i as WeaponInventoryItem;
					if (weaponInventoryItem != null && weaponInventoryItem.Amount > 0 && PlayerPed.Weapons.HasWeapon(weaponInventoryItem.Hash))
					{
						UI.Notify("You already have that weapon!");
					}
					else if (i.Amount > 0)
					{
						this.ItemUsed?.Invoke(i, type);
					}
				};
			});
		}

		private void UpdateMenuSpecific(UIMenu menu, List<InventoryItemBase> collection, bool leftBadges)
		{
			menu.MenuItems.ForEach(delegate(UIMenuItem menuItem)
			{
				InventoryItemBase itemFromMenuItem = GetItemFromMenuItem<InventoryItemBase>(collection, menuItem);
				if (itemFromMenuItem != null)
				{
					if ((CanCraftItem(itemFromMenuItem as ICraftable) || DeveloperMode) & leftBadges)
					{
						menuItem.SetLeftBadge(UIMenuItem.BadgeStyle.Tick);
					}
					else if (leftBadges)
					{
						menuItem.SetLeftBadge(UIMenuItem.BadgeStyle.Lock);
					}
					menuItem.SetRightLabel($"{itemFromMenuItem.Amount}/{itemFromMenuItem.MaxAmount}");
				}
			});
		}

		private bool CanCraftItem(ICraftable craftable)
		{
			if (craftable?.RequiredComponents != null)
			{
				CraftableItemComponent[] requiredComponents = craftable.RequiredComponents;
				foreach (CraftableItemComponent component in requiredComponents)
				{
					InventoryItemBase resource = component.Resource;
					if (Resources.Contains(resource) && Resources.Find((InventoryItemBase i) => resource == i)?.Amount < component.RequiredAmount)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private T GetSelectedInventoryItem<T>() where T : class
		{
			UIMenuItem selectedItem = InventoryMenu.MenuItems[InventoryMenu.CurrentSelection];
			return GetItemFromMenuItem<T>(Items, selectedItem);
		}

		private static T GetItemFromMenuItem<T>(List<InventoryItemBase> collection, UIMenuItem menuItem) where T : class
		{
			return collection.Find((Predicate<InventoryItemBase>)((InventoryItemBase i) => i.MenuItem == menuItem)) as T;
		}
	}
}
