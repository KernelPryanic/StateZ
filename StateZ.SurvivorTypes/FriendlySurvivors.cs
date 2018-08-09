using GTA;
using GTA.Math;
using System.Collections.Generic;
using StateZ.DataClasses;
using StateZ.Extensions;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.SurvivorTypes
{
	public class FriendlySurvivors : Survivors
	{
		private readonly List<Ped> _peds = new List<Ped>();

		private readonly PedGroup _pedGroup = new PedGroup();

		public static FriendlySurvivors Instance
		{
			get;
			private set;
		}

		public FriendlySurvivors()
		{
			Instance = this;
		}

		public void RemovePed(Ped item)
		{
			if (_peds.Contains(item))
			{
				_peds.Remove(item);
				item.LeaveGroup();
                Blip currentBlip = item.CurrentBlip;
				if (currentBlip.Handle != 0)
				{
					currentBlip.Remove();
				}
				EntityEventWrapper.Dispose(item);
			}
		}

		public override void Update()
		{
			if (_peds.Count <= 0)
			{
				Complete();
			}
		}

		public override void SpawnEntities()
		{
			int rand = Database.Random.Next(3, 6);
			Vector3 spawn = GetSpawnPoint();
			if (IsValidSpawn(spawn))
			{
				for (int i = 0; i <= rand; i++)
				{
					Ped ped = World.CreateRandomPed(spawn.Around(5f));
					if (!(ped == null))
					{
						Blip blip = ped.AddBlip();
						blip.Color = GTA.BlipColor.Blue;
						blip.Name = "Friendly";
						ped.RelationshipGroup = Relationships.FriendlyRelationship;
						ped.Task.FightAgainstHatedTargets(9000f);
						ped.SetAlertness(Alertness.FullyAlert);
						ped.SetCombatAttributes(CombatAttributes.AlwaysFight, true);
						ped.Weapons.Give(Database.WeaponHashes[Database.Random.Next(Database.WeaponHashes.Length)], 25, true, true);
						_pedGroup.Add(ped, i == 0);
						_pedGroup.FormationType = 0;
						_peds.Add(ped);
						EntityEventWrapper eventWrapper = new EntityEventWrapper(ped);
						eventWrapper.Died += EventWrapperOnDied;
						eventWrapper.Disposed += EventWrapperOnDisposed;
					}
				}
				UI.Notify("~b~Friendly~s~ survivors nearby.", true);
			}
		}

		private void EventWrapperOnDisposed(EntityEventWrapper sender, Entity entity)
		{
			if (_peds.Contains(entity as Ped))
			{
				_peds.Remove(entity as Ped);
			}
		}

		private void EventWrapperOnDied(EntityEventWrapper sender, Entity entity)
		{
			_peds.Remove(entity as Ped);
			Blip currentBlip = entity.CurrentBlip;
			if (currentBlip.Handle != 0)
			{
				currentBlip.Remove();
			}
			entity.MarkAsNoLongerNeeded();
			sender.Dispose();
		}

		public override void CleanUp()
		{
			_peds.ForEach(delegate(Ped ped)
			{
				Blip currentBlip = ped.CurrentBlip;
				if (currentBlip.Handle != 0)
				{
					currentBlip.Remove();
				}
				ped.MarkAsNoLongerNeeded();
				EntityEventWrapper.Dispose(ped);
			});
		}

		public override void Abort()
		{
			while (_peds.Count > 0)
			{
				Ped ped = _peds[0];
				ped.Delete();
				_peds.RemoveAt(0);
			}
		}
	}
}
