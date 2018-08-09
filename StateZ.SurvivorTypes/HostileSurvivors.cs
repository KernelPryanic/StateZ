using GTA;
using GTA.Math;
using System.Collections.Generic;
using StateZ.DataClasses;
using StateZ.Extensions;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.SurvivorTypes
{
	public class HostileSurvivors : Survivors
	{
		private readonly PedGroup _group = new PedGroup();

		private readonly List<Ped> _peds = new List<Ped>();

		private Vehicle _vehicle;

		public override void Update()
		{
			if (_peds.Count <= 0)
			{
				Complete();
			}
		}

		public override void SpawnEntities()
		{
			Vector3 spawn = GetSpawnPoint();
			if (IsValidSpawn(spawn))
			{
				Vehicle vehicle = World.CreateVehicle(Database.GetRandomVehicleModel(), spawn, (float)Database.Random.Next(1, 360));
				if (vehicle == null)
				{
					Complete();
				}
				else
				{
					_vehicle = vehicle;
					Blip vehicleBlip = _vehicle.AddBlip();
					vehicleBlip.Name = "Enemy Vehicle";
					vehicleBlip.Sprite = GTA.BlipSprite.PersonalVehicleCar;
					vehicleBlip.Color = GTA.BlipColor.Red;
					EntityEventWrapper vehicleWrapper = new EntityEventWrapper(_vehicle);
					vehicleWrapper.Died += VehicleWrapperOnDied;
					vehicleWrapper.Updated += VehicleWrapperOnUpdated;
					for (int i = 0; i < vehicle.PassengerSeats + 1; i++)
					{
						if (_group.MemberCount < 6 && vehicle.IsSeatFree(GTA.VehicleSeat.Any))
						{
							Ped ped = vehicle.CreateRandomPedOnSeat((i == 0) ? GTA.VehicleSeat.Driver : GTA.VehicleSeat.Any);
							if (!(ped == null))
							{
								ped.Weapons.Give(Database.WeaponHashes[Database.Random.Next(Database.WeaponHashes.Length)], 25, true, true);
								ped.SetCombatAttributes(CombatAttributes.AlwaysFight, true);
								ped.SetAlertness(Alertness.FullyAlert);
								ped.RelationshipGroup = Relationships.HostileRelationship;
								_group.Add(ped, i == 0);
								Blip blip = ped.AddBlip();
								blip.Name = "Enemy";
								_peds.Add(ped);
								EntityEventWrapper pedWrapper = new EntityEventWrapper(ped);
								pedWrapper.Died += PedWrapperOnDied;
								pedWrapper.Updated += PedWrapperOnUpdated;
								pedWrapper.Disposed += PedWrapperOnDisposed;
							}
						}
					}
					UI.Notify("~r~Hostiles~s~ nearby!");
				}
			}
		}

		private void PedWrapperOnDisposed(EntityEventWrapper sender, Entity entity)
		{
			if (_peds.Contains(entity as Ped))
			{
				_peds.Remove(entity as Ped);
			}
		}

		private void VehicleWrapperOnUpdated(EntityEventWrapper sender, Entity entity)
		{
			if (!(entity == null))
			{
				entity.CurrentBlip.Alpha = _vehicle.Driver.Exists() ? 255 : 0;
			}
		}

		private void VehicleWrapperOnDied(EntityEventWrapper sender, Entity entity)
		{
			Blip currentBlip = entity.CurrentBlip;
			if (currentBlip.Handle != 0)
			{
				currentBlip.Remove();
			}
			sender.Dispose();
			_vehicle.MarkAsNoLongerNeeded();
			_vehicle = null;
		}

		private void PedWrapperOnUpdated(EntityEventWrapper sender, Entity entity)
		{
			Ped ped = entity as Ped;
			if (!(ped == null))
			{
				Vehicle currentVehicle = ped.CurrentVehicle;
				if (((currentVehicle.Handle != 0) ? currentVehicle.Driver : null) == ped && !ped.IsInCombat)
				{
					ped.Task.DriveTo(ped.CurrentVehicle, base.PlayerPosition, 25f, 75f);
				}
				if (ped.Position.VDist(base.PlayerPosition) > Survivors.DeleteRange)
				{
					ped.Delete();
				}
				if (ped.CurrentBlip.Exists())
				{
					ped.CurrentBlip.Alpha = (!ped.IsInVehicle()) ? 255 : 0;
				}
			}
		}

		private void PedWrapperOnDied(EntityEventWrapper sender, Entity entity)
		{
			Blip currentBlip = entity.CurrentBlip;
			if (currentBlip.Handle != 0)
			{
				currentBlip.Remove();
			}
			_peds.Remove(entity as Ped);
		}

		public override void CleanUp()
		{
			Vehicle vehicle = _vehicle;
			if (vehicle != null)
			{
				Blip currentBlip = vehicle.CurrentBlip;
				if (currentBlip.Handle != 0)
				{
					currentBlip.Remove();
				}
			}
			EntityEventWrapper.Dispose(_vehicle);
		}

		public override void Abort()
		{
			Vehicle vehicle = _vehicle;
			if (vehicle != null)
			{
				vehicle.Delete();
			}
			while (_peds.Count > 0)
			{
				Ped ped = _peds[0];
				if (ped != null)
				{
					ped.Delete();
				}
				_peds.RemoveAt(0);
			}
		}
	}
}
