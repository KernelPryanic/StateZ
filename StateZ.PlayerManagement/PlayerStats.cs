using GTA;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using StateZ.Static;

namespace StateZ.PlayerManagement
{
	public class PlayerStats : Script
	{
		public static bool UseStats = true;

		private readonly float _statDamageInterval = 5f;

		private readonly float _hungerReductionMultiplier = 0.00045f;

		private readonly float _thirstReductionMultiplier = 0.0007f;

		private readonly float _sprintReductionMultiplier = 0.05f;

		private readonly float _statSustainLength = 120f;

		private readonly List<StatDisplayItem> _statDisplay;

		private float _hungerDamageTimer;

		private float _hungerSustainTimer;

		private float _thirstDamageTimer;

		private float _thirstSustainTimer;

		private bool _removedDisplay;

		private static Ped PlayerPed => Database.PlayerPed;

		public PlayerStats()
		{
			PlayerInventory.FoodUsed += PlayerInventoryOnFoodUsed;
			_sprintReductionMultiplier = this.Settings.GetValue<float>("stats", "sprint_reduction_multiplier", _sprintReductionMultiplier);
			_hungerReductionMultiplier = this.Settings.GetValue<float>("stats", "hunger_reduction_multiplier", _hungerReductionMultiplier);
			_thirstReductionMultiplier = this.Settings.GetValue<float>("stats", "thirst_reduction_multiplier", _thirstReductionMultiplier);
			_statDamageInterval = this.Settings.GetValue<float>("stats", "stat_damage_interaval", _statDamageInterval);
			_statSustainLength = this.Settings.GetValue<float>("stats", "stat_sustain_length", _statSustainLength);
			this.Settings.SetValue<bool>("stats", "use_stats", UseStats);
			this.Settings.SetValue<float>("stats", "sprint_reduction_multiplier", _sprintReductionMultiplier);
			this.Settings.SetValue<float>("stats", "hunger_reduction_multiplier", _hungerReductionMultiplier);
			this.Settings.SetValue<float>("stats", "thirst_reduction_multiplier", _thirstReductionMultiplier);
			this.Settings.SetValue<float>("stats", "stat_damage_interaval", _statDamageInterval);
			this.Settings.SetValue<float>("stats", "stat_sustain_length", _statSustainLength);
			this.Settings.Save();
			_statDisplay = new List<StatDisplayItem>();
			Stats stats = new Stats();
			foreach (Stat stat in stats.StatList)
			{
				StatDisplayItem statDisplayItem = new StatDisplayItem
				{
					Stat = stat,
					Bar = new BarTimerBar(stat.Name.ToUpper())
					{
						ForegroundColor = Color.White,
						BackgroundColor = Color.Gray
					}
				};
				_statDisplay.Add(statDisplayItem);
				MenuConrtoller.BarPool.Add(statDisplayItem.Bar);
			}
			this.Tick += (EventHandler)OnTick;
			this.Interval = 10;
		}

		private void PlayerInventoryOnFoodUsed(FoodInventoryItem item, FoodType foodType)
		{
			switch (foodType)
			{
			case FoodType.Food:
				UpdateStat(item, "Hunger", "Hunger ~g~sustained~s~.", 0f);
				break;
			case FoodType.Water:
				UpdateStat(item, "Thirst", "Thirst ~g~sustained~s~.", 0f);
				break;
			case FoodType.SpecialFood:
				UpdateStat(item, "Hunger", "Hunger ~g~sustained~s~.", 0f);
				UpdateStat(item, "Thirst", "Thirst ~g~sustained~s~.", 0.15f);
				break;
			}
		}

		private void UpdateStat(IFood item, string name, string notify, float valueOverride = 0f)
		{
			StatDisplayItem data = _statDisplay.Find((StatDisplayItem displayItem) => displayItem.Stat.Name == name);
			data.Stat.Value += ((valueOverride <= 0f) ? item.RestorationAmount : valueOverride);
			data.Stat.Sustained = true;
			UI.Notify(notify, true);
			if (data.Stat.Value > data.Stat.MaxVal)
			{
				data.Stat.Value = data.Stat.MaxVal;
			}
		}

		private void OnTick(object sender, EventArgs e)
		{
			if (Database.PlayerIsDead)
			{
				foreach (StatDisplayItem item in _statDisplay)
				{
					item.Stat.Value = item.Stat.MaxVal;
				}
			}
			else if (!UseStats)
			{
				if (!_removedDisplay)
				{
					foreach (StatDisplayItem item2 in _statDisplay)
					{
						MenuConrtoller.BarPool.Remove(item2.Bar);
					}
					_removedDisplay = true;
				}
			}
			else
			{
				if (_removedDisplay)
				{
					foreach (StatDisplayItem item3 in _statDisplay)
					{
						MenuConrtoller.BarPool.Add(item3.Bar);
					}
					_removedDisplay = false;
				}
				int i = 0;
				for (int max = _statDisplay.Count; i < max; i++)
				{
					StatDisplayItem s = _statDisplay[i];
					Stat stat = s.Stat;
					s.Bar.Percentage = stat.Value;
					HandleReductionStat(stat, "Hunger", "You're ~r~starving~s~!", _hungerReductionMultiplier, ref _hungerDamageTimer, ref _hungerSustainTimer);
					HandleReductionStat(stat, "Thirst", "You're ~r~dehydrated~s~!", _thirstReductionMultiplier, ref _thirstDamageTimer, ref _thirstSustainTimer);
					HandleStamina(stat);
				}
			}
		}

		private void HandleStamina(Stat stat)
		{
			if (!(stat.Name != "Stamina"))
			{
				if (stat.Sustained)
				{
					if (Database.PlayerIsSprinting)
					{
						if (stat.Value > 0f)
						{
							stat.Value -= Game.LastFrameTime * _sprintReductionMultiplier;
						}
						else
						{
							stat.Sustained = false;
							stat.Value = 0f;
						}
					}
					else if (stat.Value < stat.MaxVal)
					{
						stat.Value += Game.LastFrameTime * (_sprintReductionMultiplier * 10f);
					}
					else
					{
						stat.Value = stat.MaxVal;
					}
				}
				else
				{
					Game.DisableControlThisFrame(2, GTA.Control.Sprint);
					stat.Value += Game.LastFrameTime * _sprintReductionMultiplier;
					if (stat.Value >= stat.MaxVal * 0.3f)
					{
						stat.Sustained = true;
					}
				}
			}
		}

		private void HandleReductionStat(Stat stat, string targetName, string notification, float reductionMultiplier, ref float damageTimer, ref float sustainTimer)
		{
			if (!(stat.Name != targetName))
			{
				if (!stat.Sustained)
				{
					if (stat.Value > 0f)
					{
						stat.Value -= Game.LastFrameTime * reductionMultiplier;
						damageTimer = _statDamageInterval;
					}
					else
					{
						UI.Notify(notification);
						damageTimer += Game.LastFrameTime;
						if (damageTimer >= _statDamageInterval)
						{
							PlayerPed.ApplyDamage(Database.Random.Next(3, 15));
							damageTimer = 0f;
						}
						stat.Value = 0f;
					}
				}
				else
				{
					damageTimer = _statDamageInterval;
					sustainTimer += Game.LastFrameTime;
					if (!(sustainTimer < _statSustainLength))
					{
						sustainTimer = 0f;
						stat.Sustained = false;
					}
				}
			}
		}
	}
}
