using UnityEngine;

namespace script
{
    public interface IDestructible
    {
        public abstract void TakeDamage(int damage, GridAgent source = null);
        public abstract void DestroyDestructible(GridAgent source = null);
        public abstract bool IsAlive();
    }
}