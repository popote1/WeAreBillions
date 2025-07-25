
using script;
using UnityEngine;


    public class Destructible : MonoBehaviour, IDestructible
    {
        public GameObject prefabsDebrie;
        public GameObject prefabsFX;
        public int HP;

        public void TakeDamage(int damage, GridAgent source = null) {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0)
            {
                DestroyDestructible();
            }
        }

        public void DestroyDestructible(GridAgent source = null)
        {
            if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
            if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
            Destroy(gameObject);
        }

        public bool IsAlive() {
            return (HP > 0);
        }

        public Vector3 GetWorldPos() {
            return transform.position;
        }
    }
