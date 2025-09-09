using Unity.Cinemachine;
using UnityEngine;

public class InsignifiantPhysics : InsignifiantProbs
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider[] _colliders;
    [SerializeField] private float _physicMaxTime = 20f;
    [SerializeField] private float _physicMinimalSpeed = 0.1f;

    private bool _isPhysic;
    private float _timer;

    private void Update()
    {
        if (_isPhysic) {
            if (_rigidbody.linearVelocity.magnitude < _physicMinimalSpeed) {
                _timer += Time.deltaTime;
                if (_timer >= _physicMaxTime) {
                    StopPhysic();
                }
            }
            else {
                _timer = 0;
            }
        }
    }

    protected override void Destroy() {
        if (_isPhysic)return;
        base.Destroy();
        _isPhysic =true;
        _rigidbody.isKinematic = false;
    }

    private void StopPhysic() {
        _isPhysic = false;
        DisableColliders(_colliders);
        Destroy(_rigidbody);
        Destroy(this);
    }

    [ContextMenu("SetupRefsTool")]
    private void SetupRefsTool() {
        _colliders = GetComponents<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
        _impulse = GetComponent<CinemachineImpulseSource>();
    }
}