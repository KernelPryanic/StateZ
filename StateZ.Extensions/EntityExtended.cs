using GTA;
using GTA.Native;

namespace StateZ.Extensions
{
    public static class EntityExtended
    {
        public static bool IsPlayingAnim(this Entity entity, string animSet, string animName)
        {
            return Function.Call<bool>(GTA.Native.Hash.IS_ENTITY_PLAYING_ANIM, (InputArgument[])new InputArgument[4]
            {
                entity.Handle,
                animSet,
                animName,
                3
            });
        }

        public static void Fade(this Entity entity, bool state)
        {
            Function.Call(GTA.Native.Hash.FADE_OUT_LOCAL_PLAYER, (InputArgument[])new InputArgument[2]
            {
                entity.Handle,
                state ? 1 : 0
            });
        }

        public static bool HasClearLineOfSight(this Entity entity, Entity target, float visionDistance)
        {
            return Function.Call<bool>(GTA.Native.Hash.HAS_ENTITY_CLEAR_LOS_TO_ENTITY, (InputArgument[])new InputArgument[2]
            {
                entity.Handle,
                target.Handle
            }) && entity.Position.DistanceTo(target.Position) < visionDistance;
        }
    }
}
