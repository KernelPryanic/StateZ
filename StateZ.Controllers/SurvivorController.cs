using GTA;
using System;
using System.Collections.Generic;
using StateZ.DataClasses;
using StateZ.Static;
using StateZ.SurvivorTypes;

namespace StateZ.Controllers
{
	public class SurvivorController : Script, ISpawner
	{
		public delegate void OnCreatedSurvivorsEvent();

		private readonly List<Survivors> _survivors = new List<Survivors>();

		private readonly int _survivorInterval = 30;

		private readonly float _survivorSpawnChance = 0.7f;

		private readonly int _merryweatherTimeout = 120;

		private DateTime _currentDelayTime;

		public bool Spawn
		{
			get;
			set;
		}

		public static SurvivorController Instance
		{
			get;
			private set;
		}

		public event OnCreatedSurvivorsEvent CreatedSurvivors;

		public SurvivorController()
		{
			Instance = this;
			_survivorInterval = this.Settings.GetValue<int>("survivors", "survivor_interval", _survivorInterval);
			_survivorSpawnChance = this.Settings.GetValue<float>("survivors", "survivor_spawn_chance", _survivorSpawnChance);
			_merryweatherTimeout = this.Settings.GetValue<int>("survivors", "merryweather_timeout", _merryweatherTimeout);
			this.Settings.SetValue<int>("survivors", "survivor_interval", _survivorInterval);
			this.Settings.SetValue<float>("survivors", "survivor_spawn_chance", _survivorSpawnChance);
			this.Settings.SetValue<int>("survivors", "merryweather_timeout", _merryweatherTimeout);
			this.Settings.Save();
			this.Tick += (EventHandler)OnTick;
			this.Aborted += delegate
			{
				_survivors.ForEach(delegate(Survivors s)
				{
					s.Abort();
				});
			};
			CreatedSurvivors += OnCreatedSurvivors;
		}

		private void OnCreatedSurvivors()
		{
			bool rand = Database.Random.NextDouble() <= (double)_survivorSpawnChance;
			EventTypes[] values = (EventTypes[])Enum.GetValues(typeof(EventTypes));
			EventTypes type = values[Database.Random.Next(values.Length)];
			Survivors currentEvent = null;
			switch (type)
			{
			case EventTypes.Friendly:
			{
				FriendlySurvivors friendly = new FriendlySurvivors();
				currentEvent = TryCreateEvent(friendly);
				break;
			}
			case EventTypes.Hostile:
				if (Database.Random.NextDouble() <= 0.20000000298023224)
				{
					HostileSurvivors hostile = new HostileSurvivors();
					currentEvent = TryCreateEvent(hostile);
				}
				break;
			case EventTypes.Merryweather:
			{
				MerryweatherSurvivors merry = new MerryweatherSurvivors(_merryweatherTimeout);
				currentEvent = TryCreateEvent(merry);
				break;
			}
			}
			if (currentEvent != null)
			{
				_survivors.Add(currentEvent);
				currentEvent.SpawnEntities();
				currentEvent.Completed += delegate(Survivors survivors)
				{
					SetDelayTime();
					survivors.CleanUp();
					_survivors.Remove(survivors);
				};
			}
		}

		private Survivors TryCreateEvent(Survivors survivors)
		{
			Survivors currentEvent = null;
			if (_survivors.FindIndex((Survivors s) => s.GetType() == survivors.GetType()) <= -1)
			{
				currentEvent = survivors;
			}
			return currentEvent;
		}

		private void OnTick(object sender, EventArgs eventArgs)
		{
			Create();
			Destroy();
			_survivors.ForEach(delegate(Survivors s)
			{
				s.Update();
			});
		}

		private void Destroy()
		{
			if (!Spawn)
			{
				_survivors.ForEach(delegate(Survivors s)
				{
					_survivors.Remove(s);
					s.Abort();
				});
				_currentDelayTime = DateTime.UtcNow;
			}
		}

		private void Create()
		{
			if (Spawn)
			{
				DateTime currentTime = DateTime.UtcNow;
				if (!(currentTime <= _currentDelayTime))
				{
					this.CreatedSurvivors?.Invoke();
					SetDelayTime();
				}
			}
		}

		private void SetDelayTime()
		{
			_currentDelayTime = DateTime.UtcNow + new TimeSpan(0, 0, _survivorInterval);
		}
	}
}
