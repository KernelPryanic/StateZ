using GTA;
using System;
using System.Linq;
using StateZ.Wrappers;
using StateZ.Static;
using StateZ.Extensions;
using StateZ.Scripts;

namespace StateZ.Zombies
{
    public abstract class ZombiePed : IEquatable<Ped>
    {
        public static float SensingRange = 120f;
        public static float VisionDistance = 35f;
        public static float SilencerEffectiveRange = 15f;
        public static float BehindZombieNoticeDistance = 1f;
        public static float RunningNoticeDistance = 25f;
        public static float WanderRadius = 100f;
        public static float AttackRange = 1.2f;
        public static int ZombieDamage = 15;
        public const int MovementUpdateInterval = 5;

        public delegate void OnGoingToTargetEvent(Ped target);
        public delegate void OnAttackingTargetEvent(Ped target);

        private readonly Ped _ped;
        private Ped _target;
        private bool _goingToTarget;
        private bool _attackingTarget;
        private EntityEventWrapper _eventWrapper;
        private DateTime _currentMovementUpdateTime;
        public event OnGoingToTargetEvent GoToTarget;
        public event OnAttackingTargetEvent AttackTarget;

        public virtual string MovementStyle
        {
            get;
            set;
        }

        public virtual bool PlayAudio
        {
            get;
            set;
        }

        public bool GoingToTarget
        {
            get
            {
                return _goingToTarget;
            }
            set
            {
                if (value && !_goingToTarget)
                {
                    this.GoToTarget?.Invoke(Target);
                }
                _goingToTarget = value;
            }
        }

        public bool AttackingTarget
        {
            get
            {
                return _attackingTarget;
            }
            set
            {
                if (value && !_ped.IsRagdoll && !_ped.IsDead && !_ped.IsClimbing && !_ped.IsFalling && !_ped.IsBeingStunned && !_ped.IsGettingUp)
                {
                    this.AttackTarget?.Invoke(Target);
                }
                _attackingTarget = value;
            }
        }

        public Ped Target
        {
            get
            {
                return _target;
            }
            private set
            {
                if (value == null && _target != null)
                {
                    _ped.Task.WanderAround(_ped.Position, WanderRadius);
                    bool goingToTarget = AttackingTarget = false;
                    GoingToTarget = goingToTarget;
                }
                _target = value;
            }
        }

        protected ZombiePed(int handle)
        {
            _ped = new Ped(handle);
            _eventWrapper = new EntityEventWrapper(_ped);
            _eventWrapper.Died += OnDied;
            _eventWrapper.Updated += Update;
            _eventWrapper.Aborted += Abort;
            _currentMovementUpdateTime = DateTime.UtcNow;
            GoToTarget += OnGoToTarget;
            AttackTarget += OnAttackTarget;
        }

        protected bool Equals(ZombiePed other)
        {
            return this.Equals(other) && object.Equals(_ped, other._ped);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (this != obj)
                {
                    return obj.GetType() == GetType() && Equals((ZombiePed)obj);
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (this.GetHashCode() * 397) ^ ((_ped != null) ? ((object)_ped).GetHashCode() : 0);
        }

        public bool Equals(Ped other)
        {
            return object.Equals(_ped, other);
        }

        public static implicit operator Ped(ZombiePed v)
        {
            return v._ped;
        }

        public void Abort(EntityEventWrapper sender, Entity entity)
        {
            _ped.Delete();
        }

        private static bool IsBehindZombie(float distance)
        {
            return distance < BehindZombieNoticeDistance;
        }

        private static bool IsRunningNoticed(Ped ped, float distance)
        {
            return ped.IsSprinting && distance < RunningNoticeDistance;
        }

        private bool IsGoodTarget(Ped ped)
        {
            return (int)ped.GetRelationshipWithPed(_ped) == 5;
        }

        private static bool IsWeaponWellSilenced(Ped ped, float distance)
        {
            if (ped.IsShooting)
            {
                return ped.IsCurrentWeaponSileced() && distance > SilencerEffectiveRange;
            }
            return true;
        }

        private bool CanHearPed(Ped ped)
        {
            float distance = ped.Position.DistanceTo(_ped.Position);
            return !IsWeaponWellSilenced(ped, distance) || IsBehindZombie(distance) || IsRunningNoticed(ped, distance);
        }

        private void GetTarget()
        {
            Ped[] peds = World.GetNearbyPeds(_ped, SensingRange).Where(IsGoodTarget).ToArray();
            Ped closest = World.GetClosest<Ped>(_ped.Position, peds);
            if (closest != null && (_ped.HasClearLineOfSight(closest, VisionDistance) || CanHearPed(closest)))
            {
                Target = closest;
            }
            else if ((Target != null && !IsGoodTarget(Target)) || closest != Target)
            {
                Target = null;
            }
        }

        public void InfectTarget(Ped target)
        {
            if (!target.IsPlayer && target.Health <= target.MaxHealth / 4)
            {
                target.SetToRagdoll(3000);
                ZombieCreator.InfectPed(target, _ped.MaxHealth, true);
                ForgetTarget();
                target.LeaveGroup();
                target.Weapons.Drop();
                EntityEventWrapper.Dispose(target);
            }
        }

        public void ForgetTarget()
        {
            _target = null;
        }

        private void SetWalkStyle()
        {
            if (!(DateTime.UtcNow <= _currentMovementUpdateTime))
            {
                _ped.SetMovementAnimSet(MovementStyle);
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            _currentMovementUpdateTime = DateTime.UtcNow + new TimeSpan(0, 0, 0, 5);
        }

        public abstract void OnAttackTarget(Ped target);

        public abstract void OnGoToTarget(Ped target);

        private void OnDied(EntityEventWrapper sender, Entity entity)
        {
            Blip currentBlip = _ped.CurrentBlip;
            if (currentBlip.Type != 0)
            {
                currentBlip.Remove();
            }
        }

        public void Update(EntityEventWrapper entityEventWrapper, Entity entity)
        {
            if (_ped.Position.DistanceTo(Database.PlayerPosition) > 120f && (!_ped.IsOnScreen || _ped.IsDead))
            {
                _ped.Delete();
            }
            if (_ped.IsRunning)
            {
                _ped.DisablePainAudio(false);
                _ped.PlayPain(8);
                _ped.PlayFacialAnim("facials@gen_male@base", "burning_1");
            }
            GetTarget();
            SetWalkStyle();
            if (_ped.IsOnFire && !_ped.IsDead)
            {
                _ped.Kill();
            }
            _ped.StopAmbientSpeechThisFrame();
            if (!PlayAudio)
            {
                _ped.StopSpeaking(true);
            }
            if (!(Target == null))
            {
                float distance = _ped.Position.DistanceTo(Target.Position);
                if (distance > AttackRange)
                {
                    AttackingTarget = false;
                    GoingToTarget = true;
                }
                else
                {
                    AttackingTarget = true;
                    GoingToTarget = false;
                }
            }
        }
    }
}