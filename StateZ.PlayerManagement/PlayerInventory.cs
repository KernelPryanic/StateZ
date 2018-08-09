using GTA;
using GTA.Math;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using StateZ.DataClasses;
using StateZ.Extensions;
using StateZ.Static;

namespace StateZ.PlayerManagement
{
	public class PlayerInventory : Script
	{
		public delegate void OnUsedFoodEvent(FoodInventoryItem item, FoodType foodType);

		public delegate void OnUsedWeaponEvent(WeaponInventoryItem weapon);

		public delegate void OnUsedBuildableEvent(BuildableInventoryItem item, Prop newProp);

		public delegate void OnLootedEvent(Ped ped);

		public const float InteractDistance = 1.5f;

		private readonly UIMenu _mainMenu = new UIMenu(string.Empty, "INVENTORY & RESOURCES", new Point(0, -105));

		private readonly List<Ped> _lootedPeds = new List<Ped>();

		private Inventory _inventory;

		private readonly Keys _inventoryKey = Keys.I;

		public static PlayerInventory Instance
		{
			get;
			private set;
		}

		private static Ped PlayerPed => Database.PlayerPed;

		private static Vector3 PlayerPosition => Database.PlayerPosition;

		public static event OnUsedFoodEvent FoodUsed;

		public static event OnUsedWeaponEvent WeaponUsed;

		public static event OnUsedBuildableEvent BuildableUsed;

		public static event OnLootedEvent LootedPed;

		public PlayerInventory()
		{
			_inventoryKey = this.Settings.GetValue<Keys>("keys", "inventory_key", _inventoryKey);
			this.Settings.SetValue<Keys>("keys", "inventory_key", _inventoryKey);
			this.Settings.Save();
			Inventory des = Serializer.Deserialize<Inventory>("./scripts/Inventory.dat");
			if (des == null)
			{
				des = new Inventory(MenuType.Player, false);
			}
			_inventory = des;
			_inventory.LoadMenus();
			Instance = this;
			MenuConrtoller.MenuPool.Add(_mainMenu);
			UIResRectangle rectangle = new UIResRectangle();
			_mainMenu.SetBannerType(rectangle);
			UIMenuItem inventoryItem = new UIMenuItem("Inventory");
			UIMenuItem resourcesItem = new UIMenuItem("Resources");
            UIMenuCheckboxItem editModeItem = new UIMenuCheckboxItem("Edit Mode", true, "Allow yourself to pickup objects.");
            editModeItem.CheckboxEvent += (ItemCheckboxEvent)delegate (UIMenuCheckboxItem sender, bool @checked)
            {
                PlayerMap.Instance.EditMode = @checked;
            };
            UIMenuItem mainMenuItem = new UIMenuItem("Main Menu", "Navigate to the main menu. (For gamepad users)");
            mainMenuItem.Activated += (ItemActivatedEvent)delegate
            {
                MenuConrtoller.MenuPool.CloseAllMenus();
                ModController.Instance.MainMenu.Visible = true;
            };
            UIMenuCheckboxItem devModeCheckbox = new UIMenuCheckboxItem("Developer Mode", _inventory.DeveloperMode, "Enable/Disable infinite items and resources.");
            devModeCheckbox.CheckboxEvent += (ItemCheckboxEvent)delegate (UIMenuCheckboxItem item, bool @checked)
            {
                if (_inventory != null)
                {
                    if (@checked)
                    {
                        string userInput = Game.GetUserInput(GTA.WindowTitle.FMMC_KEY_TIP10, "", 12);
                        if (string.IsNullOrEmpty(userInput) || userInput.ToLower() != "michael")
                        {
                            item.Checked = false;
                            UI.Notify("Hint: Tamara Greenway's husband's first name.");
                            return;
                        }
                    }
                    _inventory.DeveloperMode = @checked;
                    if (!@checked)
                    {
                        _inventory.Items.ForEach(delegate (InventoryItemBase i)
                        {
                            i.Amount = 0;
                        });
                        _inventory.Resources.ForEach(delegate (InventoryItemBase i)
                        {
                            i.Amount = 0;
                        });
                        _inventory.RefreshMenu();
                    }
                    else
                    {
                        UI.Notify("Developer Mode: ~g~Activated~s~");
                    }
                    Serializer.Serialize("./scripts/Inventory.dat", _inventory);
                }
            };
			_mainMenu.AddItem(inventoryItem);
			_mainMenu.AddItem(resourcesItem);
			_mainMenu.BindMenuToItem(_inventory.InventoryMenu, inventoryItem);
			_mainMenu.BindMenuToItem(_inventory.ResourceMenu, resourcesItem);
			_mainMenu.AddItem(editModeItem);
			_mainMenu.AddItem(devModeCheckbox);
			_inventory.ItemUsed += InventoryOnItemUsed;
			_inventory.AddedItem += delegate
			{
				Serializer.Serialize("./scripts/Inventory.dat", _inventory);
			};
			this.Tick += (EventHandler)OnTick;
			this.KeyUp += (KeyEventHandler)OnKeyUp;
			LootedPed += OnLootedPed;
		}

		private void OnLootedPed(Ped ped)
		{
			if (ped.IsHuman)
			{
				PickupLoot(ped, ItemType.Resource, 1, 3, 0.2f);
			}
			else
			{
				AnimalLoot(ped);
			}
		}

		private void AnimalLoot(Ped ped)
		{
			if (!PlayerPed.Weapons.HasWeapon(GTA.Native.WeaponHash.Knife))
			{
				UI.Notify("You need a knife!");
			}
			else if (_inventory.AddItem(ItemFromName("Raw Meat"), 2, ItemType.Resource))
			{
				PlayerPed.Weapons.Select(GTA.Native.WeaponHash.Knife, true);
				UI.Notify("You gutted the animal for ~g~raw meat~s~.");
				PlayerPed.Task.PlayAnimation("amb@world_human_gardener_plant@male@base", "base", 8f, 3000, 0);
				_lootedPeds.Add(ped);
			}
		}

		public void PickupLoot(Ped ped, ItemType type = ItemType.Resource, int amountPerItemMin = 1, int amountPerItemMax = 3, float successChance = 0.2f)
		{
			List<InventoryItemBase> items = (type == ItemType.Resource) ? _inventory.Resources : _inventory.Items;
			if (items.All((InventoryItemBase r) => r.Amount == r.MaxAmount))
			{
				UI.Notify($"Your {type}s are full!");
			}
			else
			{
				int amount = 0;
				items.ForEach(delegate(InventoryItemBase i)
				{
					if (!(i.Id == "Cooked Meat"))
					{
						Random random = Database.Random;
						if (!(random.NextDouble() > (double)successChance))
						{
							int num = Database.Random.Next(amountPerItemMin, amountPerItemMax);
							if (i.Amount + num > i.MaxAmount)
							{
								num = i.MaxAmount - i.Amount;
							}
							_inventory.AddItem(i, num, type);
							amount += num;
						}
					}
				});
				UI.Notify(string.Format("{0}", (amount > 0) ? $"{type}s: +~g~{amount}" : "Nothing found."), true);
				PlayerPed.Task.PlayAnimation("pickup_object", "pickup_low");
				if (!(ped == null))
				{
					_lootedPeds.Add(ped);
				}
			}
		}

		private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
			if (!MenuConrtoller.MenuPool.IsAnyMenuOpen() && keyEventArgs.KeyCode == _inventoryKey)
			{
				_mainMenu.Visible = !_mainMenu.Visible;
			}
		}

		private void InventoryOnItemUsed(InventoryItemBase item, ItemType type)
		{
			if (item != null && type != 0)
			{
				if (item.GetType() == typeof(FoodInventoryItem))
				{
					FoodInventoryItem foodItem = (FoodInventoryItem)item;
					PlayerPed.Task.PlayAnimation(foodItem.AnimationDict, foodItem.AnimationName, 8f, foodItem.AnimationDuration, foodItem.AnimationFlags);
					PlayerInventory.FoodUsed?.Invoke(foodItem, foodItem.FoodType);
				}
				else if (item.GetType() == typeof(WeaponInventoryItem))
				{
					WeaponInventoryItem weaponItem = (WeaponInventoryItem)item;
					PlayerPed.Weapons.Give(weaponItem.Hash, weaponItem.Ammo, true, true);
					PlayerInventory.WeaponUsed?.Invoke(weaponItem);
				}
				else if (item.GetType() == typeof(BuildableInventoryItem) || item.GetType() == typeof(WeaponStorageInventoryItem))
				{
					if (PlayerPed.IsInVehicle())
					{
						UI.Notify("You can't build while in a vehicle!");
						return;
					}
					BuildableInventoryItem buildableItem = (BuildableInventoryItem)item;
					ItemPreview preview = new ItemPreview();
					preview.StartPreview(buildableItem.PropName, buildableItem.GroundOffset, buildableItem.IsDoor);
					while (!preview.PreviewComplete)
					{
						Script.Yield();
					}
					Prop result = preview.GetResult();
					if (result == null)
					{
						return;
					}
					AddBlipToProp(buildableItem, buildableItem.Id, result);
					PlayerInventory.BuildableUsed?.Invoke(buildableItem, result);
				}
				else if (item.GetType() == typeof(UsableInventoryItem))
				{
					UsableInventoryItem usableItem = (UsableInventoryItem)item;
					UsableItemEvent[] events = usableItem.ItemEvents;
					UsableItemEvent[] array = events;
					foreach (UsableItemEvent @event in array)
					{
						switch (@event.Event)
						{
						case ItemEvent.GiveArmor:
						{
							int arg = (@event.EventArgument as int?) ?? 0;
							Ped playerPed2 = new Ped(PlayerPed.Handle);
							playerPed2.Health = playerPed2.Health + arg;
							break;
						}
						case ItemEvent.GiveHealth:
						{
							int arg2 = (@event.EventArgument as int?) ?? 0;
							Ped playerPed = new Ped(PlayerPed.Handle);
							playerPed.Armor = playerPed.Armor + arg2;
							break;
						}
						}
					}
				}
				else if (item.GetType() == typeof(CraftableInventoryItem))
				{
					CraftableInventoryItem craftableItem = (CraftableInventoryItem)item;
					if (!craftableItem.Validation())
					{
						return;
					}
				}
				_inventory.AddItem(item, -1, type);
			}
		}

		private void OnTick(object sender, EventArgs eventArgs)
		{
			_inventory.ProcessKeys();
			GetWater();
			LootDeadPeds();
		}

		private void GetWater()
		{
			if (!PlayerPed.IsInVehicle() && !PlayerPed.IsSwimming && PlayerPed.IsInWater && !PlayerPed.IsPlayingAnim("pickup_object", "pickup_low"))
			{
				InventoryItemBase find = _inventory.Resources.Find((InventoryItemBase i) => i.Id == "Bottle");
				if (find != null && find.Amount > 0)
				{
					Game.DisableControlThisFrame(2, GTA.Control.Enter);
					if (Game.IsDisabledControlJustPressed(2, GTA.Control.Enter))
					{
						PlayerPed.Task.PlayAnimation("pickup_object", "pickup_low");
						InventoryItemBase item = ItemFromName("Dirty Water");
						AddItem(item, 1, ItemType.Resource);
						AddItem(find, -1, ItemType.Resource);
						UI.Notify("Resources: -~r~1", true);
						UI.Notify("Resources: +~g~1", true);
					}
				}
			}
		}

		private void LootDeadPeds()
		{
			if (!PlayerPed.IsInVehicle())
			{
				Ped ped = World.GetClosest<Ped>(PlayerPosition, World.GetNearbyPeds(PlayerPed, 1.5f));
				if (!(ped == null) && ped.IsDead && !_lootedPeds.Contains(ped))
				{
					UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to loot.", false);
					Game.DisableControlThisFrame(2, GTA.Control.Context);
					if (Game.IsDisabledControlJustPressed(2, GTA.Control.Context))
					{
						PlayerInventory.LootedPed?.Invoke(ped);
					}
				}
			}
		}

		private void Controller()
		{
		}

		public bool AddItem(InventoryItemBase item, int amount, ItemType type)
		{
			return item != null && _inventory.AddItem(item, amount, type);
		}

		public bool PickupItem(InventoryItemBase item, ItemType type)
		{
			return item != null && _inventory.AddItem(item, 1, type);
		}

		public InventoryItemBase ItemFromName(string id)
		{
			if (_inventory?.Items != null)
			{
				if (_inventory?.Resources != null)
				{
					InventoryItemBase[] concat = _inventory.Items.Concat(_inventory.Resources).ToArray();
					return Array.Find(concat, (InventoryItemBase i) => i.Id == id);
				}
				return null;
			}
			return null;
		}

		private static void AddBlipToProp(IProp item, string name, Entity entity)
		{
			if ((int)item.BlipSprite != 1)
			{
				Blip blip = entity.AddBlip();
				blip.Sprite = item.BlipSprite;
				blip.Color = item.BlipColor;
				blip.Name = name;
			}
		}

		public bool HasItem(InventoryItemBase item, ItemType itemType)
		{
			if (item != null)
			{
				switch (itemType)
				{
				case ItemType.Item:
					return _inventory.Items.Contains(item) && item.Amount > 0;
				case ItemType.Resource:
					return _inventory.Resources.Contains(item) && item.Amount > 0;
				default:
					return false;
				}
			}
			return false;
		}
	}
}
