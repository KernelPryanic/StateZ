using GTA;
using GTA.Native;
using NativeUI;
using System.Windows.Forms;
using StateZ.Controllers;
using StateZ.Extensions;
using StateZ.PlayerManagement;
using StateZ.Scripts;
using StateZ.Static;
using StateZ.Zombies;

namespace StateZ
{
	public class ModController : Script
	{
		private Keys _menuKey = Keys.F10;

		public UIMenu MainMenu
		{
			get;
			private set;
		}

		public static ModController Instance
		{
			get;
			private set;
		}

		public ModController()
		{
			Instance = this;
			Config.Check();
			Relationships.SetRelationships();
			LoadSave();
			ConfigureMenu();
			this.KeyUp += (KeyEventHandler)OnKeyUp;
		}

		private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
		{
			if (!MenuConrtoller.MenuPool.IsAnyMenuOpen() && keyEventArgs.KeyCode == _menuKey)
			{
				MainMenu.Visible = !MainMenu.Visible;
			}
		}

		private void LoadSave()
		{
			_menuKey = this.Settings.GetValue<Keys>("keys", "zombies_menu_key", _menuKey);
			ZombiePed.ZombieDamage = this.Settings.GetValue<int>("zombies", "zombie_damage", ZombiePed.ZombieDamage);
			this.Settings.SetValue<Keys>("keys", "zombies_menu_key", _menuKey);
			this.Settings.SetValue<int>("zombies", "zombie_damage", ZombiePed.ZombieDamage);
			this.Settings.Save();
		}

		private void ConfigureMenu()
		{
			MainMenu = new UIMenu("Simple Zombies", "SELECT AN OPTION");
			MenuConrtoller.MenuPool.Add(MainMenu);
			UIMenuCheckboxItem infectionCheckbox = new UIMenuCheckboxItem("Infection Mode", false, "Enable/Disable zombies.");
			infectionCheckbox.CheckboxEvent += delegate(UIMenuCheckboxItem sender, bool @checked)
			{
				ZombieVehicleSpawner.Instance.Spawn = @checked;
				Loot247.Instance.Spawn = @checked;
				WorldController.Configure = @checked;
				AnimalSpawner.Instance.Spawn = @checked;
				if (@checked)
				{
					WorldExtended.ClearAreaOfEverything(Database.PlayerPosition, 10000f);
					Function.Call(GTA.Native.Hash.REQUEST_IPL, (InputArgument[])new InputArgument[1]
					{
						"cs3_07_mpgates"
					});
				}
			};
			UIMenuCheckboxItem fastZombiesCheckbox = new UIMenuCheckboxItem("Fast Zombies", false, "Enable/Disable running zombies.");
			fastZombiesCheckbox.CheckboxEvent += delegate(UIMenuCheckboxItem sender, bool @checked)
			{
				ZombieCreator.Runners = @checked;
			};
			UIMenuCheckboxItem electricityCheckBox = new UIMenuCheckboxItem("Electricity", true, "Enables/Disable blackout mode.");
			electricityCheckBox.CheckboxEvent += delegate(UIMenuCheckboxItem sender, bool @checked)
			{
				World.SetBlackout(!@checked);
			};
			UIMenuCheckboxItem survivorsCheckBox = new UIMenuCheckboxItem("Survivors", false, "Enable/Disable survivors.");
			survivorsCheckBox.CheckboxEvent += delegate(UIMenuCheckboxItem sender, bool @checked)
			{
				SurvivorController.Instance.Spawn = @checked;
			};
			UIMenuCheckboxItem statsCheckbox = new UIMenuCheckboxItem("Stats", true, "Enable/Disable stats.");
			statsCheckbox.CheckboxEvent += delegate(UIMenuCheckboxItem sender, bool @checked)
			{
				PlayerStats.UseStats = @checked;
			};
			UIMenuItem loadItem = new UIMenuItem("Load", "Load the map, your vehicles and your bodyguards.");
			loadItem.SetLeftBadge(UIMenuItem.BadgeStyle.Heart);
			loadItem.Activated += delegate
			{
				PlayerMap.Instance.Deserialize();
				PlayerVehicles.Instance.Deserialize();
				PlayerGroupManager.Instance.Deserialize();
			};
			UIMenuItem saveCurrentVehicleItem = new UIMenuItem("Save", "Saves the vehicle you are currently in.");
			saveCurrentVehicleItem.SetLeftBadge(UIMenuItem.BadgeStyle.Car);
			saveCurrentVehicleItem.Activated += delegate
			{
				if (Database.PlayerCurrentVehicle == null || (Database.PlayerCurrentVehicle != null && !Database.PlayerCurrentVehicle.Exists()))
				{
					UI.Notify("You're not in a vehicle.");
				}
				else
				{
					PlayerVehicles.Instance.SaveVehicle(Database.PlayerCurrentVehicle);
				}
			};
			UIMenuItem saveAllVehiclesItem = new UIMenuItem("Save All", "Saves all marked vehicles, and their positions.");
			saveAllVehiclesItem.SetLeftBadge(UIMenuItem.BadgeStyle.Car);
			saveAllVehiclesItem.Activated += delegate
			{
				PlayerVehicles.Instance.Serialize(true);
			};
			UIMenuItem saveAllGuardsItem = new UIMenuItem("Save All", "Saves the player ped group (guards).");
			saveAllGuardsItem.SetLeftBadge(UIMenuItem.BadgeStyle.Mask);
			saveAllGuardsItem.Activated += delegate
			{
				PlayerGroupManager.Instance.SavePeds();
			};
			MainMenu.AddItem(infectionCheckbox);
			MainMenu.AddItem(fastZombiesCheckbox);
			MainMenu.AddItem(electricityCheckBox);
			MainMenu.AddItem(survivorsCheckBox);
			MainMenu.AddItem(statsCheckbox);
			MainMenu.AddItem(loadItem);
			MainMenu.AddItem(saveCurrentVehicleItem);
			MainMenu.AddItem(saveAllVehiclesItem);
			MainMenu.AddItem(saveAllGuardsItem);
			MainMenu.RefreshIndex();
		}
	}
}
