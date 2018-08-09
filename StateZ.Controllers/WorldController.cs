using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Linq;
using StateZ.Extensions;
using StateZ.Static;

namespace StateZ.Controllers
{
	public class WorldController : Script
	{
		private bool _reset;

		public static bool Configure
		{
			get;
			set;
		}

		public static bool StopPedsFromSpawning
		{
			get;
			set;
		}

		public Vector3 PlayerPosition => Database.PlayerPosition;

		public WorldController()
		{
            this.Tick += (EventHandler)OnTick;
			this.Aborted += (EventHandler)OnAborted;
		}

		private static void OnAborted(object sender, EventArgs e)
		{
			Reset();
		}

		private static void Reset()
		{
			Function.Call(GTA.Native.Hash.CAN_CREATE_RANDOM_COPS, (InputArgument[])new InputArgument[1]
			{
				true
			});
			Function.Call(GTA.Native.Hash.SET_RANDOM_BOATS, (InputArgument[])new InputArgument[1]
			{
				true
			});
			Function.Call(GTA.Native.Hash.SET_RANDOM_TRAINS, (InputArgument[])new InputArgument[1]
			{
				true
			});
			Function.Call(GTA.Native.Hash.SET_GARBAGE_TRUCKS, (InputArgument[])new InputArgument[1]
			{
				true
			});
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (Configure)
			{
				WorldExtended.ClearCops(10000f);
				WorldExtended.SetScenarioPedDensityThisMultiplierFrame(0f);
				WorldExtended.SetVehicleDensityMultiplierThisFrame(0f);
				WorldExtended.SetRandomVehicleDensityMultiplierThisFrame(0f);
				WorldExtended.SetParkedVehicleDensityMultiplierThisFrame(0f);
				WorldExtended.SetPedDensityThisMultiplierFrame(0f);
				WorldExtended.SetScenarioPedDensityThisMultiplierFrame(0f);
				Game.MaxWantedLevel = 0;
				Vehicle[] allVehicles = (from v in World.GetAllVehicles()
				where !v.IsPersistent
				select v).ToArray();
				Vehicle[] planes = Array.FindAll(allVehicles, (Vehicle v) => (int)v.ClassType == 16);
				Vehicle[] trains = Array.FindAll(allVehicles, (Vehicle v) => (int)v.ClassType == 21);
				Vehicle[] drivers = Array.FindAll(allVehicles, (Vehicle v) => v.Driver.Exists() && !v.Driver.IsPlayer);
				Array.ForEach(drivers, delegate(Vehicle vehicle)
				{
					vehicle.Delete();
				});
				Array.ForEach(planes, delegate(Vehicle plane)
				{
					if (plane.Driver.Exists() && !plane.Driver.IsPlayer && !plane.Driver.IsDead)
					{
						plane.Driver.Kill();
					}
				});
				Array.ForEach(trains, delegate(Vehicle t)
				{
					Function.Call(GTA.Native.Hash.SET_TRAIN_SPEED, (InputArgument[])new InputArgument[2]
					{
						t.Handle,
						0f
					});
				});
				ScriptExtended.TerminateScriptByName("re_prison");
				ScriptExtended.TerminateScriptByName("am_prison");
				ScriptExtended.TerminateScriptByName("gb_biker_free_prisoner");
				ScriptExtended.TerminateScriptByName("re_prisonvanbreak");
				ScriptExtended.TerminateScriptByName("am_vehicle_spawn");
				ScriptExtended.TerminateScriptByName("am_taxi");
				ScriptExtended.TerminateScriptByName("audiotest");
				ScriptExtended.TerminateScriptByName("freemode");
				ScriptExtended.TerminateScriptByName("re_prisonerlift");
				ScriptExtended.TerminateScriptByName("am_prison");
				ScriptExtended.TerminateScriptByName("re_lossantosintl");
				ScriptExtended.TerminateScriptByName("re_armybase");
				ScriptExtended.TerminateScriptByName("restrictedareas");
				ScriptExtended.TerminateScriptByName("stripclub");
				ScriptExtended.TerminateScriptByName("re_gangfight");
				ScriptExtended.TerminateScriptByName("re_gang_intimidation");
				ScriptExtended.TerminateScriptByName("spawn_activities");
				ScriptExtended.TerminateScriptByName("am_vehiclespawn");
				ScriptExtended.TerminateScriptByName("traffick_air");
				ScriptExtended.TerminateScriptByName("traffick_ground");
				ScriptExtended.TerminateScriptByName("emergencycall");
				ScriptExtended.TerminateScriptByName("emergencycalllauncher");
				ScriptExtended.TerminateScriptByName("clothes_shop_sp");
				ScriptExtended.TerminateScriptByName("gb_rob_shop");
				ScriptExtended.TerminateScriptByName("gunclub_shop");
				ScriptExtended.TerminateScriptByName("hairdo_shop_sp");
				ScriptExtended.TerminateScriptByName("re_shoprobbery");
				ScriptExtended.TerminateScriptByName("shop_controller");
				ScriptExtended.TerminateScriptByName("re_crashrescue");
				ScriptExtended.TerminateScriptByName("re_rescuehostage");
				ScriptExtended.TerminateScriptByName("fm_mission_controller");
				ScriptExtended.TerminateScriptByName("player_scene_m_shopping");
				ScriptExtended.TerminateScriptByName("shoprobberies");
				ScriptExtended.TerminateScriptByName("re_atmrobbery");
				ScriptExtended.TerminateScriptByName("ob_vend1");
				ScriptExtended.TerminateScriptByName("ob_vend2");
				Function.Call(GTA.Native.Hash.STOP_ALARM, (InputArgument[])new InputArgument[2]
				{
					"PRISON_ALARMS",
					0
				});
				Function.Call(GTA.Native.Hash.CLEAR_AMBIENT_ZONE_STATE, (InputArgument[])new InputArgument[3]
				{
					"AZ_COUNTRYSIDE_PRISON_01_ANNOUNCER_GENERAL",
					0,
					0
				});
				Function.Call(GTA.Native.Hash.CLEAR_AMBIENT_ZONE_STATE, (InputArgument[])new InputArgument[3]
				{
					"AZ_COUNTRYSIDE_PRISON_01_ANNOUNCER_WARNING",
					0,
					0
				});
				int t3 = Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, (InputArgument[])new InputArgument[1]
				{
					"prop_gate_prison_01"
				});
				Function.Call(GTA.Native.Hash.SET_STATE_OF_CLOSEST_DOOR_OF_TYPE, (InputArgument[])new InputArgument[7]
				{
					t3,
					1845f,
					2605f,
					45f,
					false,
					0,
					0
				});
				int t2 = Function.Call<int>(GTA.Native.Hash.GET_HASH_KEY, (InputArgument[])new InputArgument[1]
				{
					"prop_gate_prison_01"
				});
				Function.Call(GTA.Native.Hash._DOOR_CONTROL, (InputArgument[])new InputArgument[7]
				{
					t2,
					1819.27f,
					2608.53f,
					44.61f,
					false,
					0,
					0
				});
				if (_reset)
				{
					Function.Call(GTA.Native.Hash.CAN_CREATE_RANDOM_COPS, (InputArgument[])new InputArgument[1]
					{
						false
					});
					Function.Call(GTA.Native.Hash.SET_RANDOM_BOATS, (InputArgument[])new InputArgument[1]
					{
						false
					});
					Function.Call(GTA.Native.Hash.SET_RANDOM_TRAINS, (InputArgument[])new InputArgument[1]
					{
						false
					});
					Function.Call(GTA.Native.Hash.SET_GARBAGE_TRUCKS, (InputArgument[])new InputArgument[1]
					{
						false
					});
					Function.Call(GTA.Native.Hash._0xF796359A959DF65D, (InputArgument[])new InputArgument[1]
					{
						false
					});
					_reset = false;
				}
			}
			else if (!_reset)
			{
				Reset();
				_reset = true;
			}
		}
	}
}
