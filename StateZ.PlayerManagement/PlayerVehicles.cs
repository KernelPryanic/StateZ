using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.PlayerManagement
{
	public class PlayerVehicles : Script
	{
		private VehicleCollection _vehicleCollection;

		private readonly List<Vehicle> _vehicles = new List<Vehicle>();

		public static PlayerVehicles Instance
		{
			get;
			private set;
		}

		public PlayerVehicles()
		{
			Instance = this;
			this.Aborted += (EventHandler)OnAborted;
		}

		private void OnAborted(object sender, EventArgs eventArgs)
		{
			_vehicleCollection.ToList().ForEach(delegate(VehicleData vehicle)
			{
				vehicle.Delete();
			});
		}

		public void Deserialize()
		{
			if (_vehicleCollection == null)
			{
				VehicleCollection des = Serializer.Deserialize<VehicleCollection>("./scripts/Vehicles.dat");
				if (des == null)
				{
					des = new VehicleCollection();
				}
				_vehicleCollection = des;
				_vehicleCollection.ListChanged += delegate
				{
					Serialize(false);
				};
				foreach (VehicleData item in _vehicleCollection)
				{
					Vehicle vehicle = World.CreateVehicle(item.Hash, item.Position);
					if (vehicle == null)
					{
						UI.Notify("Failed to load vehicle.");
						break;
					}
					vehicle.PrimaryColor = item.PrimaryColor;
					vehicle.SecondaryColor = item.SecondaryColor;
					vehicle.Health = item.Health;
					vehicle.EngineHealth = item.EngineHealth;
					vehicle.Rotation = item.Rotation;
					item.Handle = vehicle.Handle;
					AddKit(vehicle, item);
					AddBlipToVehicle(vehicle);
					_vehicles.Add(vehicle);
					vehicle.IsPersistent = true;
					EntityEventWrapper wrapper = new EntityEventWrapper(vehicle);
					wrapper.Died += WrapperOnDied;
				}
			}
		}

		private static void AddKit(Vehicle vehicle, VehicleData data)
		{
			if (data != null && !(vehicle == null))
			{
				vehicle.InstallModKit();
				data.NeonLights?.ToList().ForEach(delegate(VehicleNeonLight h)
				{
					vehicle.SetNeonLightsOn(h, true);
				});
				data.Mods?.ForEach(delegate(Tuple<VehicleMod, int> m)
				{
					vehicle.SetMod(m.Item1, m.Item2, true);
				});
				data.ToggleMods?.ToList().ForEach(delegate(VehicleToggleMod h)
				{
					vehicle.ToggleMod(h, true);
				});
				vehicle.WindowTint = data.WindowTint;
				vehicle.WheelType = data.WheelType;
				vehicle.NeonLightsColor = data.NeonColor;
				Function.Call(GTA.Native.Hash.SET_VEHICLE_LIVERY, (InputArgument[])new InputArgument[2]
				{
					vehicle.Handle,
					data.Livery
				});
			}
		}

		public void Serialize(bool notify = false)
		{
			if (_vehicleCollection != null)
			{
				UpdateVehicleData();
				Serializer.Serialize("./scripts/Vehicles.dat", _vehicleCollection);
				if (notify)
				{
					UI.Notify((_vehicleCollection.Count <= 0) ? "No vehicles." : "~p~Vehicles~s~ saved!");
				}
			}
		}

		private void UpdateVehicleData()
		{
			if (_vehicleCollection.Count > 0)
			{
				_vehicleCollection.ToList().ForEach(delegate(VehicleData v)
				{
					Vehicle val = _vehicles.Find((Vehicle i) => i.Handle == v.Handle);
					if (!(val == null))
					{
						UpdateDataSpecific(v, val);
					}
				});
			}
		}

		private static void UpdateDataSpecific(VehicleData vehicleData, Vehicle vehicle)
		{
			vehicleData.Position = vehicle.Position;
			vehicleData.Rotation = vehicle.Rotation;
			vehicleData.Health = vehicle.Health;
			vehicleData.EngineHealth = vehicle.EngineHealth;
			vehicleData.PrimaryColor = vehicle.PrimaryColor;
			vehicleData.SecondaryColor = vehicle.SecondaryColor;
		}

		public void SaveVehicle(Vehicle vehicle)
		{
			if (_vehicleCollection == null)
			{
				Deserialize();
			}
			VehicleData saved = _vehicleCollection.ToList().Find((VehicleData v) => v.Handle == vehicle.Handle);
			if (saved != null)
			{
				UpdateDataSpecific(saved, vehicle);
				Serialize(true);
			}
			else
			{
				VehicleNeonLight[] neonHashes2 = (VehicleNeonLight[])Enum.GetValues(typeof(VehicleNeonLight));
				neonHashes2 = ((IEnumerable<VehicleNeonLight>)neonHashes2).Where((Func<VehicleNeonLight, bool>)vehicle.IsNeonLightsOn).ToArray();
				VehicleMod[] modHashes = (VehicleMod[])Enum.GetValues(typeof(VehicleMod));
				List<Tuple<VehicleMod, int>> mods = new List<Tuple<VehicleMod, int>>();
				modHashes.ToList().ForEach(delegate(VehicleMod h)
				{
					int mod = vehicle.GetMod(h);
					if (mod != -1)
					{
						mods.Add(new Tuple<VehicleMod, int>(h, mod));
					}
				});
				VehicleToggleMod[] toggleModHashes2 = (VehicleToggleMod[])Enum.GetValues(typeof(VehicleToggleMod));
				toggleModHashes2 = ((IEnumerable<VehicleToggleMod>)toggleModHashes2).Where((Func<VehicleToggleMod, bool>)vehicle.IsToggleModOn).ToArray();
				int handle = vehicle.Handle;
				Model model = vehicle.Model;
				VehicleData item = new VehicleData(handle, model.Hash, vehicle.Rotation, vehicle.Position, vehicle.PrimaryColor, vehicle.SecondaryColor, vehicle.Health, vehicle.EngineHealth, vehicle.Heading, neonHashes2, mods, toggleModHashes2, vehicle.WindowTint, vehicle.WheelType, vehicle.NeonLightsColor, Function.Call<int>(GTA.Native.Hash.GET_VEHICLE_LIVERY, (InputArgument[])new InputArgument[1]
				{
					vehicle.Handle
				}), Function.Call<bool>(GTA.Native.Hash.GET_VEHICLE_MOD_VARIATION, (InputArgument[])new InputArgument[2]
				{
					vehicle.Handle,
					23
				}), Function.Call<bool>(GTA.Native.Hash.GET_VEHICLE_MOD_VARIATION, (InputArgument[])new InputArgument[2]
				{
					vehicle.Handle,
				    24
				}));
				_vehicleCollection.Add(item);
				_vehicles.Add(vehicle);
				vehicle.IsPersistent = true;
				EntityEventWrapper wrapper = new EntityEventWrapper(vehicle);
				wrapper.Died += WrapperOnDied;
				AddBlipToVehicle(vehicle);
			}
		}

		private static void AddBlipToVehicle(Vehicle vehicle)
		{
			Blip blip = vehicle.AddBlip();
			blip.Sprite = GetSprite(vehicle);
			blip.Color = GTA.BlipColor.Yellow;
			blip.Name = vehicle.FriendlyName;
			blip.Scale = 0.85f;
		}

		private static BlipSprite GetSprite(Vehicle vehicle)
		{
			return ((int)vehicle.ClassType == 8) ? GTA.BlipSprite.PersonalVehicleBike : (((int)vehicle.ClassType == 14) ? GTA.BlipSprite.Boat : (((int)vehicle.ClassType == 15) ? GTA.BlipSprite.Helicopter : (((int)vehicle.ClassType == 16) ? GTA.BlipSprite.Plane : GTA.BlipSprite.PersonalVehicleCar)));
		}

		private void WrapperOnDied(EntityEventWrapper sender, Entity entity)
		{
			UI.Notify("Your vehicle was ~r~destroyed~s~!");
			_vehicleCollection.Remove(_vehicleCollection.ToList().Find((VehicleData v) => v.Handle == entity.Handle));
			Blip currentBlip = entity.CurrentBlip;
			if (currentBlip.Handle != 0)
			{
				currentBlip.Remove();
			}
			sender.Dispose();
		}
	}
}
