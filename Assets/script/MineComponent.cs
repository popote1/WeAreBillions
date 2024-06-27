using script;
using Unity.Mathematics;
using UnityEngine;

public class MineComponent : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private TriggerZoneDetector _triggerZoneDetector;
    [SerializeField] private float _delayExplosion =1;
    [SerializeField] private GameObject _psExplosion;
    private bool _istrigger;


    private float _delay;
    
    // Update is called once per frame
    void Update()
    {
        ManagerTrigger();
        ManagerDelay();
        
    }

    private void ManagerTrigger() {
        _triggerZoneDetector.CheckOfNull();
        _istrigger =_triggerZoneDetector.Zombis.Count>0;
        _animator.SetBool("IsTrigger", _istrigger);
    }

    private void ManagerDelay() {
        if (_istrigger) {
            _delay += Time.deltaTime;
            if (_delay >= _delayExplosion) {
                _triggerZoneDetector.CheckOfNull();
                foreach (var zombi in _triggerZoneDetector.Zombis) {
                    zombi.TakeDamage(5);
                }

                Instantiate(_psExplosion, transform.position, quaternion.identity);
                Destroy(gameObject);
            }
        }
        else {
            _delay = 0;
        }
    }
}
