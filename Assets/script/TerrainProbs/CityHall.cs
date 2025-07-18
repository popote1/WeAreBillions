using UnityEngine;

namespace script
{
    public class CityHall :MonoBehaviour , IDestructible
    {
        public int HP;
        public void TakeDamage(int damage, GridAgent source = null)
        {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0) {
                DestroyDestructible();
            }
        }

        public void DestroyDestructible(GridAgent source = null) {
            StaticEvents.OnGameWin?.Invoke();
        }

        public bool IsAlive() {
            return (HP > 0);
        }
    }
}