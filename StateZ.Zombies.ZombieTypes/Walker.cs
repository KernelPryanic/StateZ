using GTA;
using StateZ.Extensions;

namespace StateZ.Zombies.ZombieTypes
{
    public class Walker : ZombiePed
    {
        private readonly Ped _ped;

        public override string MovementStyle
        {
            get;
            set;
        } = "move_m@drunk@verydrunk";


        public Walker(int handle)
            : base(handle)
        {
            _ped = this;
        }

        public override void OnAttackTarget(Ped target)
        {
            if (target.IsDead)
            {
                if (!_ped.IsPlayingAnim("amb@world_human_bum_wash@male@high@idle_a", "idle_b"))
                {
                    _ped.Task.PlayAnimation("amb@world_human_bum_wash@male@high@idle_a", "idle_b", 8f, -1, GTA.AnimationFlags.Loop);
                }
            }
            else if (!_ped.IsPlayingAnim("rcmbarry", "bar_1_teleport_aln"))
            {
                _ped.Task.PlayAnimation("rcmbarry", "bar_1_teleport_aln", 8f, 1000, GTA.AnimationFlags.UpperBodyOnly);
                if (!target.IsInvincible)
                {
                    target.ApplyDamage(ZombiePed.ZombieDamage);
                }
                InfectTarget(target);
            }
        }

        public override void OnGoToTarget(Ped target)
        {
            _ped.Task.GoTo(target);
        }
    }
}
