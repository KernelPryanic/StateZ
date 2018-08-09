using GTA;
using GTA.Native;
using System;
using StateZ.Wrappers;

namespace StateZ.Extensions
{
    public static class PedExtended
    {
        internal static readonly string[] SpeechModifierNames = new string[37]
        {
            "SPEECH_PARAMS_STANDARD",
            "SPEECH_PARAMS_ALLOW_REPEAT",
            "SPEECH_PARAMS_BEAT",
            "SPEECH_PARAMS_FORCE",
            "SPEECH_PARAMS_FORCE_FRONTEND",
            "SPEECH_PARAMS_FORCE_NO_REPEAT_FRONTEND",
            "SPEECH_PARAMS_FORCE_NORMAL",
            "SPEECH_PARAMS_FORCE_NORMAL_CLEAR",
            "SPEECH_PARAMS_FORCE_NORMAL_CRITICAL",
            "SPEECH_PARAMS_FORCE_SHOUTED",
            "SPEECH_PARAMS_FORCE_SHOUTED_CLEAR",
            "SPEECH_PARAMS_FORCE_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY",
            "SPEECH_PARAMS_MEGAPHONE",
            "SPEECH_PARAMS_HELI",
            "SPEECH_PARAMS_FORCE_MEGAPHONE",
            "SPEECH_PARAMS_FORCE_HELI",
            "SPEECH_PARAMS_INTERRUPT",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED_CLEAR",
            "SPEECH_PARAMS_INTERRUPT_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_INTERRUPT_NO_FORCE",
            "SPEECH_PARAMS_INTERRUPT_FRONTEND",
            "SPEECH_PARAMS_INTERRUPT_NO_FORCE_FRONTEND",
            "SPEECH_PARAMS_ADD_BLIP",
            "SPEECH_PARAMS_ADD_BLIP_ALLOW_REPEAT",
            "SPEECH_PARAMS_ADD_BLIP_FORCE",
            "SPEECH_PARAMS_ADD_BLIP_SHOUTED",
            "SPEECH_PARAMS_ADD_BLIP_SHOUTED_FORCE",
            "SPEECH_PARAMS_ADD_BLIP_INTERRUPT",
            "SPEECH_PARAMS_ADD_BLIP_INTERRUPT_FORCE",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CLEAR",
            "SPEECH_PARAMS_FORCE_PRELOAD_ONLY_SHOUTED_CRITICAL",
            "SPEECH_PARAMS_SHOUTED",
            "SPEECH_PARAMS_SHOUTED_CLEAR",
            "SPEECH_PARAMS_SHOUTED_CRITICAL"
        };

        public static void PlayPain(this Ped ped, int type)
        {
            Function.Call(GTA.Native.Hash.PLAY_PAIN, (InputArgument[])new InputArgument[4]
            {
                ped.Handle,
                type,
                0,
                0
            });
        }

        public static void PlayFacialAnim(this Ped ped, string animSet, string animName)
        {
            Function.Call(GTA.Native.Hash.PLAY_FACIAL_ANIM, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                animName,
                animSet
            });
        }

        public static bool HasBeenDamagedByMelee(this Ped ped)
        {
            return Function.Call<bool>(GTA.Native.Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                0,
                1
            });
        }

        public static bool HasBeenDamagedBy(this Ped ped, WeaponHash weapon)
        {
            return Function.Call<bool>(GTA.Native.Hash.HAS_ENTITY_BEEN_DAMAGED_BY_WEAPON, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                (int)weapon,
                0
            });
        }

        public unsafe static Bone LastDamagedBone(this Ped ped)
        {
            int outBone = default(int);
            if (!Function.Call<bool>(GTA.Native.Hash.GET_PED_LAST_DAMAGE_BONE, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                &outBone
            }))
            {
                return 0;
            }
            return (Bone)outBone;
        }

        public static void SetPathAvoidWater(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_PATH_PREFER_TO_AVOID_WATER, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void SetStealthMovement(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_STEALTH_MOVEMENT, (InputArgument[])new InputArgument[2]
            {
                toggle ? 1 : 0,
                "DEFAULT_ACTION"
            });
        }

        public static bool GetStealthMovement(this Ped ped)
        {
            return Function.Call<bool>(GTA.Native.Hash.GET_PED_STEALTH_MOVEMENT, (InputArgument[])new InputArgument[1]
            {
                ped.Handle
            });
        }

        public static void SetComponentVariation(this Ped ped, ComponentId id, int drawableId, int textureId, int paletteId)
        {
            Function.Call(GTA.Native.Hash.SET_PED_COMPONENT_VARIATION, (InputArgument[])new InputArgument[5]
            {
                ped.Handle,
                (int)id,
                drawableId,
                textureId,
                paletteId
            });
        }

        public static int GetDrawableVariation(this Ped ped, ComponentId id)
        {
            return Function.Call<int>(GTA.Native.Hash.GET_PED_DRAWABLE_VARIATION, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)id
            });
        }

        public static int GetNumberOfDrawableVariations(this Ped ped, ComponentId id)
        {
            return Function.Call<int>(GTA.Native.Hash.GET_NUMBER_OF_PED_DRAWABLE_VARIATIONS, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)id
            });
        }

        public static bool IsTaskActive(this Ped ped, Subtask task)
        {
            return Function.Call<bool>(GTA.Native.Hash.GET_IS_TASK_ACTIVE, (InputArgument[])new InputArgument[2]
            {
                ped,
                (int)task
            });
        }

        public static bool IsDriving(this Ped ped)
        {
            return ped.IsTaskActive(Subtask.DrivingWandering) || ped.IsTaskActive(Subtask.DrivingGoingToDestinationOrEscorting);
        }

        public static void SetPathCanUseLadders(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_PATH_CAN_USE_LADDERS, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void SetPathCanClimb(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_PATH_CAN_USE_CLIMBOVERS, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void SetMovementAnimSet(this Ped ped, string animation)
        {
            if (!(ped == null))
            {
                while (!Function.Call<bool>(GTA.Native.Hash.HAS_ANIM_SET_LOADED, (InputArgument[])new InputArgument[1]
                {
                    animation
                }))
                {
                    Function.Call(GTA.Native.Hash.REQUEST_ANIM_SET, (InputArgument[])new InputArgument[1]
                    {
                        animation
                    });
                    Script.Yield();
                }
                Function.Call(GTA.Native.Hash.SET_PED_MOVEMENT_CLIPSET, (InputArgument[])new InputArgument[3]
                {
                    ped.Handle,
                    animation,
                    1048576000
                });
            }
        }

        public static void RemoveElegantly(this Ped ped)
        {
            Function.Call(GTA.Native.Hash.REMOVE_PED_ELEGANTLY, (InputArgument[])new InputArgument[1]
            {
                ped.Handle
            });
        }

        public static void SetRagdollOnCollision(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_RAGDOLL_ON_COLLISION, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle
            });
        }

        public static void SetAlertness(this Ped ped, Alertness alertness)
        {
            Function.Call(GTA.Native.Hash.SET_PED_ALERTNESS, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)alertness
            });
        }

        public static void SetCombatAblility(this Ped ped, CombatAbility ability)
        {
            Function.Call(GTA.Native.Hash.SET_PED_COMBAT_ABILITY, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)ability
            });
        }

        public static void SetCanEvasiveDive(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_CAN_EVASIVE_DIVE, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void StopAmbientSpeechThisFrame(this Ped ped)
        {
            if (ped.IsAmbientSpeechPlaying())
            {
                Function.Call(GTA.Native.Hash.STOP_CURRENT_PLAYING_AMBIENT_SPEECH, (InputArgument[])new InputArgument[1]
                {
                    ped.Handle
                });
            }
        }

        public static bool IsAmbientSpeechPlaying(this Ped ped)
        {
            return Function.Call<bool>(GTA.Native.Hash.IS_AMBIENT_SPEECH_PLAYING, (InputArgument[])new InputArgument[1]
            {
                ped.Handle
            });
        }

        public static void DisablePainAudio(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.DISABLE_PED_PAIN_AUDIO, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void StopSpeaking(this Ped ped, bool shaking)
        {
            Function.Call(GTA.Native.Hash.STOP_PED_SPEAKING, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                shaking ? 1 : 0
            });
        }

        public static void SetCanPlayAmbientAnims(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_CAN_PLAY_AMBIENT_ANIMS, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void SetCombatAttributes(this Ped ped, CombatAttributes attribute, bool enabled)
        {
            Function.Call(GTA.Native.Hash.SET_PED_COMBAT_ATTRIBUTES, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                (int)attribute,
                enabled
            });
        }

        public static void SetPathAvoidFires(this Ped ped, bool toggle)
        {
            Function.Call(GTA.Native.Hash.SET_PED_PATH_AVOID_FIRE, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                toggle ? 1 : 0
            });
        }

        public static void ApplyDamagePack(this Ped ped, float damage, float multiplier, DamagePack damagePack)
        {
            Function.Call(GTA.Native.Hash.APPLY_PED_DAMAGE_PACK, (InputArgument[])new InputArgument[4]
            {
                ped.Handle,
                damagePack.ToString(),
                damage,
                multiplier
            });
        }

        public static void SetCanAttackFriendlies(this Ped ped, FirendlyFireType type)
        {
            switch (type)
            {
                case FirendlyFireType.CanAttack:
                    Function.Call(GTA.Native.Hash.SET_CAN_ATTACK_FRIENDLY, (InputArgument[])new InputArgument[3]
                    {
                        ped.Handle,
                        true,
                        false
                    });
                    break;
                case FirendlyFireType.CantAttack:
                    Function.Call(GTA.Native.Hash.SET_CAN_ATTACK_FRIENDLY, (InputArgument[])new InputArgument[3]
                    {
                        ped.Handle,
                        false,
                        false
                    });
                    break;
            }
        }

        public static void PlayAmbientSpeech(this Ped ped, string speechName, SpeechModifier modifier = SpeechModifier.Standard)
        {
            if (modifier < SpeechModifier.Standard || (int)modifier >= SpeechModifierNames.Length)
            {
                throw new ArgumentOutOfRangeException("modifier");
            }
            Function.Call(GTA.Native.Hash._PLAY_AMBIENT_SPEECH1, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                speechName,
                SpeechModifierNames[(int)modifier]
            });
        }

        public static void Recruit(this Ped ped, Ped leader, bool canBeTargeted, bool invincible, int accuracy)
        {
            if (!(leader == null))
            {
                ped.LeaveGroup();
                ped.SetRagdollOnCollision(false);
                ped.Task.ClearAll();
                PedGroup group = leader.CurrentPedGroup;
                group.SeparationRange = 2.14748365E+09f;
                if (!group.Contains(leader))
                {
                    group.Add(leader, true);
                }
                if (!group.Contains(ped))
                {
                    group.Add(ped, false);
                }
                ped.CanBeTargetted = canBeTargeted;
                ped.Accuracy = accuracy;
                ped.IsInvincible = invincible;
                ped.IsPersistent = true;
                ped.RelationshipGroup = leader.RelationshipGroup;
                ped.NeverLeavesGroup = true;
                Blip currentBlip = ped.CurrentBlip;
                if (currentBlip.Type != 0)
                {
                    currentBlip.Remove();
                }
                Blip blip = ped.AddBlip();
                blip.Color = GTA.BlipColor.Blue;
                blip.Scale = 0.7f;
                blip.Name = "Friend";
                EntityEventWrapper wrapper = new EntityEventWrapper(ped);
                wrapper.Died += delegate (EntityEventWrapper sender, Entity entity)
                {
                    Blip currentBlip2 = entity.CurrentBlip;
                    if (currentBlip2.Type != 0)
                    {
                        currentBlip2.Remove();
                    }
                    wrapper.Dispose();
                };
                ped.PlayAmbientSpeech("GENERIC_HI", SpeechModifier.Standard);
            }
        }

        public static void Recruit(this Ped ped, Ped leader, bool canBeTargetted)
        {
            ped.Recruit(leader, canBeTargetted, false, 100);
        }

        public static void Recruit(this Ped ped, Ped leader)
        {
            ped.Recruit(leader, true);
        }

        public static void SetCombatRange(this Ped ped, CombatRange range)
        {
            Function.Call(GTA.Native.Hash.SET_PED_COMBAT_RANGE, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)range
            });
        }

        public static void SetCombatMovement(this Ped ped, CombatMovement movement)
        {
            Function.Call(GTA.Native.Hash.SET_PED_COMBAT_MOVEMENT, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                (int)movement
            });
        }

        public static void ClearFleeAttributes(this Ped ped)
        {
            Function.Call(GTA.Native.Hash.SET_PED_FLEE_ATTRIBUTES, (InputArgument[])new InputArgument[3]
            {
                ped.Handle,
                0,
                0
            });
        }

        public static bool IsUsingAnyScenario(this Ped ped)
        {
            return Function.Call<bool>(GTA.Native.Hash.IS_PED_USING_ANY_SCENARIO, (InputArgument[])new InputArgument[1]
            {
                ped.Handle
            });
        }

        public static bool CanHearPlayer(this Ped ped, Player player)
        {
            return Function.Call<bool>(GTA.Native.Hash.CAN_PED_HEAR_PLAYER, (InputArgument[])new InputArgument[2]
            {
                player.Handle,
                ped.Handle
            });
        }

        public static void SetHearingRange(this Ped ped, float hearingRange)
        {
            Function.Call(GTA.Native.Hash.SET_PED_HEARING_RANGE, (InputArgument[])new InputArgument[2]
            {
                ped.Handle,
                hearingRange
            });
        }

        public static bool IsCurrentWeaponSileced(this Ped ped)
        {
            return Function.Call<bool>(GTA.Native.Hash.IS_PED_CURRENT_WEAPON_SILENCED, (InputArgument[])new InputArgument[1]
            {
                ped.Handle
            });
        }

        public static void Jump(this Ped ped)
        {
            Function.Call(GTA.Native.Hash.TASK_JUMP, (InputArgument[])new InputArgument[4]
            {
                ped.Handle,
                true,
                0,
                0
            });
        }

        public static void SetToRagdoll(this Ped ped, int time)
        {
            Function.Call(GTA.Native.Hash.SET_PED_TO_RAGDOLL, (InputArgument[])new InputArgument[7]
            {
                ped.Handle,
                time,
                0,
                0,
                0,
                0,
                0
            });
        }
    }
}
