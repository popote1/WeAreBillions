using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace script
{
    [SelectionBase]
    public class ObjectifBuilding :MonoBehaviour , IDestructible
    {
        [SerializeField]private bool WinOnDestruction;
        [SerializeField]private int HP;
        public UnityEvent OnDestruction;
        [Space(10)] [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private VFXBuildingDestruction _vfxBuildingDestruction; 
        public void TakeDamage(int damage, GridAgent source = null)
        {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0) {
                DestroyDestructible();
            }
        }

        public void DestroyDestructible(GridAgent source = null) {
            foreach (var coll in  GetComponentsInChildren<Collider>()) {
                coll.enabled = false;
            }
            _vfxBuildingDestruction.StartDestruction();
            if (_impulseSource) _impulseSource.GenerateImpulse();
            OnDestruction?.Invoke();
            if (WinOnDestruction) StaticEvents.OnGameWin?.Invoke();
        }

        public bool IsAlive() {
            return (HP > 0);
        }
    }
}