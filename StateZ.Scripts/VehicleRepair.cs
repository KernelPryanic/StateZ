using GTA;
using GTA.Math;
using System;
using StateZ.Extensions;
using StateZ.PlayerManagement;
using StateZ.Static;

namespace StateZ.Scripts
{
	public class VehicleRepair : Script
	{
		private Vehicle _selectedVehicle;

		private InventoryItemBase _item;

		private readonly int _repairTimeMs = 7500;

		private static Ped PlayerPed => Database.PlayerPed;

		public VehicleRepair()
		{
			_repairTimeMs = this.Settings.GetValue<int>("interaction", "vehicle_repair_time_ms", _repairTimeMs);
			this.Settings.SetValue<int>("interaction", "vehicle_repair_time_ms", _repairTimeMs);
			this.Settings.Save();
			this.Tick += (EventHandler)OnTick;
			this.Aborted += (EventHandler)OnAborted;
		}

		private static void OnAborted(object sender, EventArgs e)
		{
			PlayerPed.Task.ClearAll();
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (!Database.PlayerInVehicle)
			{
				Vehicle vehicle = World.GetClosestVehicle(Database.PlayerPosition, 20f);
				if (_item == null)
				{
					_item = PlayerInventory.Instance.ItemFromName("Vehicle Repair Kit");
				}
				if (_selectedVehicle != null)
				{
					Game.DisableControlThisFrame(2, GTA.Control.Attack);
					UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_ATTACK~ to cancel.", false);
					if (Game.IsDisabledControlJustPressed(2, GTA.Control.Attack))
					{
						PlayerPed.Task.ClearAllImmediately();
						_selectedVehicle.CloseDoor(GTA.VehicleDoor.Hood, false);
						_selectedVehicle = null;
					}
					else if (PlayerPed.TaskSequenceProgress == -1)
					{
						_selectedVehicle.EngineHealth = 1000f;
						_selectedVehicle.CloseDoor(GTA.VehicleDoor.Hood, false);
						_selectedVehicle = null;
						PlayerInventory.Instance.AddItem(_item, -1, ItemType.Item);
						UI.Notify("Items: -~r~1");
					}
				}
				else if (vehicle != null)
				{
					Model model = vehicle.Model;
					if (model.IsCar && vehicle.EngineHealth < 1000f && !MenuConrtoller.MenuPool.IsAnyMenuOpen() && !vehicle.IsUpsideDown && vehicle.HasBone("engine"))
					{
						Vector3 pos = vehicle.GetBoneCoord(vehicle.GetBoneIndex("engine"));
						if (!(pos == Vector3.Zero) && PlayerPed.IsInRangeOf(pos, 1.5f))
						{
							if (!PlayerInventory.Instance.HasItem(_item, ItemType.Item))
							{
								UiExtended.DisplayHelpTextThisFrame("You need a vehicle repair kit to fix this engine.", false);
							}
							else
							{
								Game.DisableControlThisFrame(2, GTA.Control.Context);
								UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to repair engine.", false);
								if (Game.IsDisabledControlJustPressed(2, GTA.Control.Context))
								{
									vehicle.OpenDoor(GTA.VehicleDoor.Hood, false, false);
									PlayerPed.Weapons.Select(GTA.Native.WeaponHash.Wrench, true);
									Vector3 position = pos + vehicle.ForwardVector;
									Vector3 val = vehicle.Position - Database.PlayerPosition;
									float heading = val.ToHeading();
									TaskSequence sequence = new TaskSequence();
									sequence.AddTask.ClearAllImmediately();
									sequence.AddTask.GoTo(position, false, 1500);
									sequence.AddTask.AchieveHeading(heading, 2000);
									sequence.AddTask.PlayAnimation("mp_intro_seq@", "mp_mech_fix", 8f, -8f, _repairTimeMs, GTA.AnimationFlags.Loop, 1f);
									sequence.AddTask.ClearAll();
									sequence.Close();
									PlayerPed.Task.PerformSequence(sequence);
									sequence.Dispose();
									_selectedVehicle = vehicle;
								}
							}
						}
					}
				}
			}
		}
	}
}
