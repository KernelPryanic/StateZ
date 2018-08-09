using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using StateZ.Extensions;
using StateZ.Static;

namespace StateZ.Scripts
{
	public class AnimalSpawner : Script, ISpawner
	{
		public const int MinAnimalsPerSpawn = 3;

		public const int MaxAnimalsPerSpawn = 10;

		public const int RespawnDistance = 200;

		private readonly PedHash[] _possibleAnimals;

		private readonly List<Blip> _spawnBlips;

		private Dictionary<Blip, List<Ped>> _spawnMap;

		public static AnimalSpawner Instance
		{
			get;
			private set;
		}

		public bool Spawn
		{
			get;
			set;
		}

		public AnimalSpawner()
		{
			PedHash[] obj = new PedHash[5];
            obj[0] = PedHash.Deer;
            obj[1] = PedHash.Rabbit;
            obj[2] = PedHash.Cormorant;
            obj[3] = PedHash.Boar;
            obj[4] = PedHash.MountainLion;
			_possibleAnimals = (PedHash[])obj;
			_spawnBlips = new List<Blip>();
			_spawnMap = new Dictionary<Blip, List<Ped>>();
			//this._002Ector();
			Instance = this;
			this.Tick += (EventHandler)OnTick;
			this.Aborted += (EventHandler)OnAborted;
		}

		private void OnAborted(object sender, EventArgs e)
		{
			Clear();
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (Spawn)
			{
				CreateBlips();
				int j = 0;
				for (int max = _spawnBlips.Count; j < max; j++)
				{
					Blip blip = _spawnBlips[j];
					if (!_spawnMap.ContainsKey(blip))
					{
						List<Ped> animals2 = CreateAnimals(blip);
						_spawnMap.Add(blip, animals2);
					}
					else
					{
						List<Ped> animals = _spawnMap[blip];
						for (int k = animals.Count - 1; k >= 0; k--)
						{
							Ped animal = animals[k];
							if (animal == null)
							{
								animals.Remove(null);
							}
							else if (animal.IsDead || !animal.Exists())
							{
								animal.MarkAsNoLongerNeeded();
								animals.Remove(animal);
							}
						}
					}
				}
				_spawnMap = _spawnMap.Where(delegate(KeyValuePair<Blip, List<Ped>> i)
				{
					if (i.Value.Count == 0)
					{
						float num = i.Key.Position.VDist(Database.PlayerPosition);
						return !(num > 200f);
					}
					return true;
				}).ToDictionary((KeyValuePair<Blip, List<Ped>> i) => i.Key, (KeyValuePair<Blip, List<Ped>> i) => i.Value);
			}
			else
			{
				Clear();
			}
		}

		private List<Ped> CreateAnimals(Blip blip)
		{
			List<Ped> animals = new List<Ped>();
			int num = Database.Random.Next(3, 10);
			PedHash hash = _possibleAnimals[Database.Random.Next(_possibleAnimals.Length)];
			for (int i = 0; i < num; i++)
			{
				Model val = hash;
				Vector3 position = blip.Position;
				Ped animal = World.CreatePed(val, position.Around(5f));
				if (!(animal == null))
				{
					animals.Add(animal);
					animal.Task.WanderAround();
					animal.IsPersistent = true;
					Relationships.SetRelationshipBothWays(GTA.Relationship.Hate, Relationships.PlayerRelationship, animal.RelationshipGroup);
				}
			}
			return animals;
		}

		private void CreateBlips()
		{
			if (_spawnBlips.Count < Database.AnimalSpawns.Length)
			{
				int i = 0;
				for (int max = Database.AnimalSpawns.Length; i < max; i++)
				{
					Vector3 position = Database.AnimalSpawns[i];
					Blip b = World.CreateBlip(position);
					b.Sprite = GTA.BlipSprite.Hunting;
					b.Name = "Animals";
					_spawnBlips.Add(b);
				}
			}
		}

		private void Clear()
		{
			if (_spawnBlips.Count > 0)
			{
				foreach (Blip spawnBlip in _spawnBlips)
				{
					if (_spawnMap.ContainsKey(spawnBlip))
					{
						List<Ped> animals = _spawnMap[spawnBlip];
						foreach (Ped item in animals)
						{
							item.Delete();
						}
					}
					spawnBlip.Remove();
				}
			}
			_spawnMap.Clear();
		}
	}
}
