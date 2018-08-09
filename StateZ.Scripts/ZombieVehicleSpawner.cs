using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StateZ.DataClasses;
using StateZ.Extensions;
using StateZ.Static;

namespace StateZ.Scripts
{
	public class ZombieVehicleSpawner : Script, ISpawner
	{
		public const int SpawnBlockingDistance = 150;

		private readonly int _maxVehicles = 10;

		private readonly int _maxZombies = 30;

		private readonly int _minVehicles = 1;

		private readonly int _minZombies = 7;

		private readonly int _spawnDistance = 75;

		private readonly int _minSpawnDistance = 50;

		private readonly int _zombieHealth = 750;

		private bool _nightFall;

		private List<Ped> _peds = new List<Ped>();

		private List<Vehicle> _vehicles = new List<Vehicle>();

		private readonly VehicleClass[] _classes;

		public string[] InvalidZoneNames;

		public bool Spawn
		{
			get;
			set;
		}

		public SpawnBlocker SpawnBlocker
		{
			get;
		}

		private static Ped PlayerPed => Database.PlayerPed;

		private static Vector3 PlayerPosition => Database.PlayerPosition;

		public static ZombieVehicleSpawner Instance
		{
			get;
			private set;
		}

		public ZombieVehicleSpawner()
		{
			VehicleClass[] obj = new VehicleClass[3];
            obj[0] = GTA.VehicleClass.Commercial;
            obj[1] = GTA.VehicleClass.Muscle;
            obj[2] = GTA.VehicleClass.Compacts;
			_classes = (VehicleClass[])obj;
			InvalidZoneNames = new string[8]
			{
				"Los Santos International Airport",
				"Fort Zancudo",
				"Bolingbroke Penitentiary",
				"Davis Quartz",
				"Palmer-Taylor Power Station",
				"RON Alternates Wind Farm",
				"Terminal",
				"Humane Labs and Research"
			};
			SpawnBlocker = new SpawnBlocker();
			//this._002Ector();
			Instance = this;
			_minZombies = this.Settings.GetValue<int>("spawning", "min_spawned_zombies", _minZombies);
			_maxZombies = this.Settings.GetValue<int>("spawning", "max_spawned_zombies", _maxZombies);
			_minVehicles = this.Settings.GetValue<int>("spawning", "min_spawned_vehicles", _minVehicles);
			_maxVehicles = this.Settings.GetValue<int>("spawning", "max_spawned_vehicles", _maxVehicles);
			_spawnDistance = this.Settings.GetValue<int>("spawning", "spawn_distance", _spawnDistance);
			_minSpawnDistance = this.Settings.GetValue<int>("spawning", "min_spawn_distance", _minSpawnDistance);
			_zombieHealth = this.Settings.GetValue<int>("zombies", "zombie_health", _zombieHealth);
			this.Settings.SetValue<int>("spawning", "min_spawned_zombies", _minZombies);
			this.Settings.SetValue<int>("spawning", "max_spawned_zombies", _maxZombies);
			this.Settings.SetValue<int>("spawning", "min_spawned_vehicles", _minVehicles);
			this.Settings.SetValue<int>("spawning", "max_spawned_vehicles", _maxVehicles);
			this.Settings.SetValue<int>("spawning", "spawn_distance", _spawnDistance);
			this.Settings.SetValue<int>("spawning", "min_spawn_distance", _minSpawnDistance);
			this.Settings.SetValue<int>("zombies", "zombie_health", _zombieHealth);
			this.Settings.Save();
			this.Tick += (EventHandler)OnTick;
			this.Aborted += (EventHandler)delegate
			{
				ClearAll();
			};
			this.Interval = 100;
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (Spawn)
			{
				if (!MenuConrtoller.MenuPool.IsAnyMenuOpen())
				{
					if (ZombieCreator.IsNightFall() && !_nightFall)
					{
						UiExtended.DisplayHelpTextThisFrame("Nightfall approaches. Zombies are far more ~r~aggressive~s~ at night.", false);
						_nightFall = true;
					}
					else if (!ZombieCreator.IsNightFall())
					{
						_nightFall = false;
					}
				}
				SpawnVehicles();
				SpawnPeds();
			}
			else
			{
				ClearAll();
			}
		}

		private void SpawnPeds()
		{
			_peds = _peds.Where(Exists).ToList();
			if (_peds.Count < _maxZombies)
			{
				for (int i = 0; i < _maxZombies - _peds.Count; i++)
				{
					Vector3 position = PlayerPed.Position;
					Vector3 spawn = position.Around((float)_spawnDistance);
					spawn = World.GetNextPositionOnStreet(spawn);
					if (!IsValidSpawn(spawn))
					{
						break;
					}
					Vector3 around = spawn.Around(5f);
					if (around.IsOnScreen() || around.VDist(PlayerPosition) < (float)_minSpawnDistance)
					{
						break;
					}
					Ped ped = World.CreateRandomPed(around);
					if (!(ped == null))
					{
						_peds.Add(ZombieCreator.InfectPed(ped, _zombieHealth, false));
					}
				}
			}
		}

		private void SpawnVehicles()
		{
			_vehicles = _vehicles.Where(Exists).ToList();
			if (_vehicles.Count < _maxVehicles)
			{
				for (int i = 0; i < _maxVehicles - _vehicles.Count; i++)
				{
					Vector3 position = PlayerPed.Position;
					Vector3 spawn = position.Around((float)_spawnDistance);
					spawn = World.GetNextPositionOnStreet(spawn);
					if (IsInvalidZone(spawn) || !IsValidSpawn(spawn))
					{
						break;
					}
					Vector3 around = spawn.Around(2.5f);
					if (around.IsOnScreen() || around.VDist(PlayerPosition) < (float)_minSpawnDistance)
					{
						break;
					}
					Model model = Database.GetRandomVehicleModel();
					Vehicle veh = World.CreateVehicle(model, around);
					if (!(veh == null))
					{
						veh.EngineHealth = 0f;
						veh.MarkAsNoLongerNeeded();
						veh.DirtLevel = 14f;
						SmashRandomWindow(veh);
						if (Database.Random.NextDouble() < 0.5)
						{
							SmashRandomWindow(veh);
						}
						if (Database.Random.NextDouble() < 0.20000000298023224)
						{
							OpenRandomDoor(veh);
						}
						veh.Heading += (float)Database.Random.Next(1, 360);
						_vehicles.Add(veh);
					}
				}
			}
		}

		private static void OpenRandomDoor(Vehicle veh)
		{
			VehicleDoor[] values = veh.GetDoors();
			VehicleDoor rand = values[Database.Random.Next(values.Length)];
			veh.OpenDoor(rand, false, true);
		}

		private static void SmashRandomWindow(Vehicle veh)
		{
			VehicleWindow[] values2 = (VehicleWindow[])Enum.GetValues(typeof(VehicleWindow));
			values2 = (from v in values2
			where Function.Call<bool>(GTA.Native.Hash.IS_VEHICLE_WINDOW_INTACT, (InputArgument[])new InputArgument[2]
			{
				veh.Handle,
				(int)v
			})
			select v).ToArray();
			VehicleWindow rand = values2[Database.Random.Next(values2.Length)];
			veh.SmashWindow(rand);
		}

		public bool IsInvalidZone(Vector3 spawn)
		{
			return Array.Find(InvalidZoneNames, (string z) => z == World.GetZoneName(spawn)) != null;
		}

		private static bool Exists(Entity arg)
		{
			return arg != null && arg.Exists();
		}

		private void ClearAll()
		{
			while (_peds.Count > 0)
			{
				Ped ped = _peds[0];
				ped.Delete();
				_peds.RemoveAt(0);
			}
			while (_vehicles.Count > 0)
			{
				Vehicle veh = _vehicles[0];
				veh.Delete();
				_vehicles.RemoveAt(0);
			}
		}

		public bool IsValidSpawn(Vector3 spawnPoint)
		{
			int index = SpawnBlocker.FindIndex((Vector3 spawn) => spawn.VDist(spawnPoint) < 150f);
			return index <= -1;
		}
	}
}
