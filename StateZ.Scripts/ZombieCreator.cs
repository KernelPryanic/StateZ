using GTA;
using GTA.Native;
using System;
using StateZ.Extensions;
using StateZ.Static;
using StateZ.Zombies;
using StateZ.Zombies.ZombieTypes;

namespace StateZ.Scripts
{
    public static class ZombieCreator
    {
        public static bool Runners
        {
            get;
            set;
        }

        public static ZombiePed InfectPed(Ped ped, int health, bool overrideAsFastZombie = false)
        {
            ped.Task.ClearAllImmediately();
            ped.CanPlayGestures = false;
            ped.SetCanPlayAmbientAnims(false);
            ped.SetCanEvasiveDive(false);
            ped.SetPathCanUseLadders(false);
            ped.SetPathCanClimb(false);
            ped.DisablePainAudio(true);
            ped.ApplyDamagePack(0f, 1f, DamagePack.BigHitByVehicle);
            ped.ApplyDamagePack(0f, 1f, DamagePack.ExplosionMed);
            ped.AlwaysDiesOnLowHealth = false;
            ped.SetAlertness(Alertness.Nuetral);
            ped.SetCombatAttributes(CombatAttributes.AlwaysFight, true);
            Function.Call(GTA.Native.Hash.SET_PED_FLEE_ATTRIBUTES, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                0,
                0
            });
            ped.SetConfigFlag(281, true);
            ped.Task.WanderAround(ped.Position, ZombiePed.WanderRadius);
            ped.AlwaysKeepTask = true;
            ped.BlockPermanentEvents = true;
            ped.IsPersistent = false;
            Blip currentBlip = ped.CurrentBlip;
            if (currentBlip.Handle != 0)
            {
                currentBlip.Remove();
            }
            ped.RelationshipGroup = Relationships.InfectedRelationship;
            float chance = 0.055f;
            if (IsNightFall())
            {
                chance = 0.5f;
            }
            TimeSpan time = World.CurrentDayTime;
            if (time.Hours >= 20 || time.Hours <= 3)
            {
                chance = 0.4f;
            }
            if (!((Database.Random.NextDouble() < (double)chance) | overrideAsFastZombie) || !Runners)
            {
                int health2;
                ped.MaxHealth = health2 = health;
                ped.Health = health2;
                return new Walker(ped.Handle);
            }
            return new Runner(ped.Handle);
        }

        public static bool IsNightFall()
        {
            if (Runners)
            {
                TimeSpan time = World.CurrentDayTime;
                return time.Hours >= 20 || time.Hours <= 3;
            }
            return false;
        }
    }
}
