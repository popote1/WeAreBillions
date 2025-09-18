using System;
using UnityEngine;

namespace script
{
    
    public interface IDestructible
    {
        public event EventHandler OnDestructibleDestroy;
        public abstract void TakeDamage(int damage, GridAgent source = null);
        public abstract void DestroyDestructible(GridAgent source = null);
        public abstract bool IsAlive();
        public abstract Vector3 GetPosition();
    }
}