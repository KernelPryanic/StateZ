using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using StateZ.DataClasses;
using StateZ.Extensions;
using StateZ.PlayerManagement;
using StateZ.Static;
using StateZ.Wrappers;

namespace StateZ.SurvivorTypes
{
	public class MerryweatherSurvivors : Survivors
	{
		private enum DropType
		{
			Weapons,
			Loot
		}

		public const float InteractDistance = 2.3f;

		private const int BlipRadius = 145;

		private readonly int _timeOut;

		private ParticleEffect _particle;

		private readonly PedGroup _pedGroup = new PedGroup();

		private readonly List<Ped> _peds = new List<Ped>();

		private Blip _blip;

		private Prop _prop;

		private DropType _dropType;

		private bool _notify;

		private Vector3 _dropZone;

		private readonly BarTimerBar _timerBar;

		private float _currentTime;

		private readonly PedHash[] _pedHashes;

		private readonly WeaponHash[] _weapons;

		public MerryweatherSurvivors(int timeout)
		{
			PedHash[] obj = new PedHash[3];
            obj[0] = PedHash.Armoured01SMM;
            obj[1] = PedHash.Armoured02SMM;
            obj[2] = PedHash.Armymech01SMY;
			_pedHashes = (PedHash[])obj;
			WeaponHash[] obj2 = new WeaponHash[5];
            obj2[0] = WeaponHash.AssaultRifle;
            obj2[1] = WeaponHash.AssaultShotgun;
            obj2[2] = WeaponHash.AssaultSMG;
            obj2[3] = WeaponHash.MarksmanRifle;
            obj2[4] = WeaponHash.SMG;
			_weapons = (WeaponHash[])obj2;
			//base._002Ector();
			_timerBar = new BarTimerBar("TIME LEFT");
			_timeOut = timeout;
			_currentTime = (float)_timeOut;
		}

		public override void Update()
		{
			if (!(_prop == null))
			{
				TryInteract(_prop);
				UpdateTimer();
				if (!CantSeeCrate())
				{
					Blip blip = _prop.AddBlip();
					if (!(blip == null))
					{
						blip.Sprite = GTA.BlipSprite.CrateDrop;
						blip.Color = GTA.BlipColor.Yellow;
						blip.Name = "Crate Drop";
						_blip.Remove();
						_peds.ForEach(delegate(Ped ped)
						{
							Blip val = ped.AddBlip();
							val.Color = GTA.BlipColor.Yellow;
							val.Name = "Merryweather Security";
						});
					}
				}
			}
		}

		private bool CantSeeCrate()
		{
			return !_prop.IsOnScreen || _prop.IsOccluded || _prop.CurrentBlip.Exists() || _prop.Position.VDist(base.PlayerPosition) > 50f;
		}

		private void UpdateTimer()
		{
			if (base.PlayerPosition.VDist(_dropZone) < 145f)
			{
				if (!_notify)
				{
					BigMessageThread.MessageInstance.ShowMissionPassedMessage("~r~Entering Hostile Zone", 5000);
					_notify = true;
				}
				if (MenuConrtoller.BarPool.ToList().Contains(_timerBar))
				{
					MenuConrtoller.BarPool.Remove(_timerBar);
				}
			}
			else
			{
				if (!MenuConrtoller.BarPool.ToList().Contains(_timerBar))
				{
					MenuConrtoller.BarPool.Add(_timerBar);
				}
				_timerBar.Percentage = _currentTime / (float)_timeOut;
				_currentTime -= Game.LastFrameTime;
				if (!(_currentTime > 0f))
				{
					Complete();
					UI.Notify("~r~Failed~s~ to retrieve crate.");
				}
			}
		}

		public override void SpawnEntities()
		{
			Vector3 spawn = GetSpawnPoint();
			if (IsValidSpawn(spawn))
			{
				DropType[] dropTypes = (DropType[])Enum.GetValues(typeof(DropType));
				DropType randDrop = _dropType = dropTypes[Database.Random.Next(dropTypes.Length)];
				string model = (_dropType == DropType.Weapons) ? "prop_mil_crate_01" : "ex_prop_crate_closed_bc";
				_prop = World.CreateProp(model, spawn, Vector3.Zero, false, false);
				if (_prop == null)
				{
					Complete();
				}
				else
				{
					Vector3 position = _prop.Position;
					_blip = World.CreateBlip(position.Around(45f), 145f);
					_blip.Color = GTA.BlipColor.Yellow;
					_blip.Alpha = 150;
					_dropZone = _blip.Position;
					position = _prop.Position;
					Vector3 particlePos = position.Around(5f);
					_particle = WorldExtended.CreateParticleEffectAtCoord(particlePos, "exp_grd_flare");
					_particle.Color = Color.LightGoldenrodYellow;
					int rand = Database.Random.Next(3, 6);
					for (int i = 0; i <= rand; i++)
					{
						Vector3 pedSpawn = spawn.Around(10f);
						PedHash pedHash = _pedHashes[Database.Random.Next(_pedHashes.Length)];
						Ped ped = World.CreatePed(pedHash, pedSpawn);
						if (!(ped == null))
						{
							if (i > 0)
							{
								ped.Weapons.Give(_weapons[Database.Random.Next(_weapons.Length)], 45, true, true);
							}
							else
							{
								ped.Weapons.Give(GTA.Native.WeaponHash.SniperRifle, 15, true, true);
                            }
							ped.Accuracy = 100;
							ped.Task.GuardCurrentPosition();
							ped.RelationshipGroup = Relationships.MilitiaRelationship;
							_pedGroup.Add(ped, i == 0);
							_peds.Add(ped);
							EntityEventWrapper pedWrapper = new EntityEventWrapper(ped);
							pedWrapper.Died += PedWrapperOnDied;
						}
					}
					Model val = "mesa3";
					position = _prop.Position;
					World.CreateVehicle(val, World.GetNextPositionOnStreet(position.Around(25f)));
					UI.Notify(string.Format("~y~Merryweather~s~ {0} drop nearby.", (_dropType == DropType.Loot) ? "loot" : "weapons"));
				}
			}
		}

		private void PedWrapperOnDied(EntityEventWrapper sender, Entity entity)
		{
			_peds.Remove(entity as Ped);
			entity.MarkAsNoLongerNeeded();
			Blip currentBlip = entity.CurrentBlip;
			if (currentBlip.Handle != 0)
			{
				currentBlip.Remove();
			}
			sender.Dispose();
		}

		private void TryInteract(Entity prop)
		{
			if (!(prop == null) && !(prop.Position.VDist(base.PlayerPosition) >= 2.3f))
			{
				UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to loot.", false);
				Game.DisableControlThisFrame(2, GTA.Control.Context);
				if (Game.IsDisabledControlJustPressed(2, GTA.Control.Context))
				{
					prop.Delete();
					switch (_dropType)
					{
					case DropType.Loot:
					{
						int rand = Database.Random.Next(1, 3);
						PlayerInventory.Instance.PickupLoot(null, ItemType.Item, rand, rand, 0.4f);
						break;
					}
					case DropType.Weapons:
					{
						int rand2 = Database.Random.Next(3, 5);
						int amount2 = 0;
						for (int i = 0; i <= rand2; i++)
						{
							WeaponHash[] weaponHashes = ((WeaponHash[])Enum.GetValues(typeof(WeaponHash))).Where(IsGoodHash).ToArray();
							if (weaponHashes.Length != 0)
							{
								WeaponHash hash = weaponHashes[Database.Random.Next(weaponHashes.Length)];
								base.PlayerPed.Weapons.Give(hash, Database.Random.Next(20, 45), false, true);
								amount2++;
							}
						}
						UI.Notify($"Found ~g~{amount2}~s~ weapons.");
						break;
					}
					}
					Complete();
				}
			}
		}

		public override void CleanUp()
		{
			_particle.Delete();
			_peds?.ForEach(delegate(Ped ped)
			{
				Blip currentBlip = ped.CurrentBlip;
				if (currentBlip.Handle != 0)
				{
					currentBlip.Remove();
				}
				ped.AlwaysKeepTask = true;
				ped.IsPersistent = false;
			});
			Blip blip = _blip;
			if (blip != null)
			{
				blip.Remove();
			}
			if (MenuConrtoller.BarPool.ToList().Contains(_timerBar))
			{
				MenuConrtoller.BarPool.Remove(_timerBar);
			}
		}

		public override void Abort()
		{
			_particle?.Delete();
			Prop prop = _prop;
			if (prop != null)
			{
				prop.Delete();
			}
			_peds?.ForEach(delegate(Ped ped)
			{
				Blip currentBlip = ped.CurrentBlip;
				if (currentBlip.Handle != 0)
				{
					currentBlip.Remove();
				}
				ped.Delete();
			});
			Blip blip = _blip;
			if (blip != null)
			{
				blip.Remove();
			}
			if (MenuConrtoller.BarPool.ToList().Contains(_timerBar))
			{
				MenuConrtoller.BarPool.Remove(_timerBar);
			}
		}

		private bool IsGoodHash(WeaponHash hash)
		{
			return (int)hash != -1569615261 && (int)hash != -1600701090 && (int)hash != 600439132 && (int)hash != 126349499 && !base.PlayerPed.Weapons.HasWeapon(hash);
		}
	}
}
