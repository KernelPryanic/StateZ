using GTA;
using System;
using System.Collections.Generic;
using StateZ.Scripts;

namespace StateZ.Wrappers
{
    public class EntityEventWrapper
    {
        public delegate void OnDeathEvent(EntityEventWrapper sender, Entity entity);

        public delegate void OnWrapperAbortedEvent(EntityEventWrapper sender, Entity entity);

        public delegate void OnWrapperUpdateEvent(EntityEventWrapper sender, Entity entity);

        public delegate void OnWrapperDisposedEvent(EntityEventWrapper sender, Entity entity);

        private static readonly List<EntityEventWrapper> Wrappers = new List<EntityEventWrapper>();

        private bool _isDead;

        public Entity Entity
        {
            get;
            set;
        }

        public bool IsDead
        {
            get
            {
                return Entity.IsDead;
            }
            private set
            {
                if (value && !_isDead)
                {
                    this.Died?.Invoke(this, Entity);
                }
                _isDead = value;
            }
        }

        public event OnDeathEvent Died;

        public event OnWrapperAbortedEvent Aborted;

        public event OnWrapperUpdateEvent Updated;

        public event OnWrapperDisposedEvent Disposed;

        public EntityEventWrapper(Entity ent)
        {
            Entity = ent;
            ScriptEventHandler.Instance.RegisterWrapper(OnTick);
            ScriptEventHandler.Instance.Aborted += delegate
            {
                Abort();
            };
            Wrappers.Add(this);
        }

        public void OnTick(object sender, EventArgs eventArgs)
        {
            if (Entity == null || !Entity.Exists())
            {
                Dispose();
            }
            else
            {
                IsDead = Entity.IsDead;
                this.Updated?.Invoke(this, Entity);
            }
        }

        public void Abort()
        {
            this.Aborted?.Invoke(this, Entity);
        }

        public void Dispose()
        {
            ScriptEventHandler.Instance.UnregisterWrapper(OnTick);
            Wrappers.Remove(this);
            this.Disposed?.Invoke(this, Entity);
        }

        public static void Dispose(Entity entity)
        {
            EntityEventWrapper wrapper = Wrappers.Find((EntityEventWrapper w) => w.Entity == entity);
            wrapper?.Dispose();
            Wrappers.Remove(wrapper);
        }
    }
}
