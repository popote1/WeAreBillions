using UnityEngine;

namespace script
{
    public class CityHall :MonoBehaviour , IDestructible
    {
        public int HP;
        public void TakeDamage(int damage)
        {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0) {
                Destroy();
            }
        }

        public void Destroy() {
            StaticEvents.OnGameWin?.Invoke();
        }

        public bool IsAlive() {
            return (HP > 0);
        }
    }
}