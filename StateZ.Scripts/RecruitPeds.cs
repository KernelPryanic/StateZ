using GTA;
using GTA.Math;
using System;
using StateZ.Extensions;
using StateZ.Static;
using StateZ.SurvivorTypes;

namespace StateZ.Scripts
{
	public class RecruitPeds : Script
	{
		public const float InteractDistance = 1.5f;

		private static Ped PlayerPed => Database.PlayerPed;

		private static Vector3 PlayerPosition => Database.PlayerPosition;

		public RecruitPeds()
		{
			this.Tick += (EventHandler)OnTick;
		}

		private static void OnTick(object sender, EventArgs eventArgs)
		{
			if (!MenuConrtoller.MenuPool.IsAnyMenuOpen() && PlayerPed.CurrentPedGroup.MemberCount < 6)
			{
				Ped[] peds = World.GetNearbyPeds(PlayerPed, 1.5f);
				Ped ped = World.GetClosest<Ped>(PlayerPosition, peds);
				if (!(ped == null) && !ped.IsDead && !ped.IsInCombatAgainst(PlayerPed) && (int)ped.GetRelationshipWithPed(PlayerPed) != 5 && ped.RelationshipGroup == Relationships.FriendlyRelationship && !(ped.CurrentPedGroup == PlayerPed.CurrentPedGroup))
				{
					Game.DisableControlThisFrame(2, GTA.Control.Enter);
					UiExtended.DisplayHelpTextThisFrame("Press ~INPUT_ENTER~ to recruit this ped.", false);
					if (Game.IsDisabledControlJustPressed(2, GTA.Control.Enter))
					{
						if (FriendlySurvivors.Instance != null)
						{
							FriendlySurvivors.Instance.RemovePed(ped);
						}
						ped.Recruit(PlayerPed);
						if (PlayerPed.CurrentPedGroup.MemberCount >= 6)
						{
							UI.Notify("You've reached the max amount of ~b~guards~s~.");
						}
					}
				}
			}
		}
	}
}
