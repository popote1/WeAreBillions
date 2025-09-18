using System;
using script;
using UnityEngine;

public class Destructible : MonoBehaviour, IDestructible {
        public GameObject prefabsDebrie;
        public GameObject prefabsFX;
        public int HP;

        public event EventHandler OnDestructibleDestroy;

        public virtual void TakeDamage(int damage, GridAgent source = null) {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0)
            {
                DestroyDestructible();
            }
        }

        public virtual void DestroyDestructible(GridAgent source = null) {
            ManageSpawnVFX();
            if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
            OnDestructibleDestroy?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }

        private void ManageSpawnVFX() {
            if (VFXPoolManager.Instance) {
                VfxPoolableMono vfx =VFXPoolManager.Instance.GetPooledVFXOfType(VFXPoolManager.VFXPooledType.SmallExplosion);
                vfx.transform.position = transform.position;
            }
            else if (prefabsFX) {
                Instantiate(prefabsFX, transform.position, transform.rotation);
            }
        }

        public virtual bool IsAlive() {
            return (HP > 0);
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Vector3 GetWorldPos() {
            return transform.position;
        }
}