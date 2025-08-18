using System;
using UnityEngine;

public class FullPhysicProbs : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _physicMaxTime = 20f;
    [SerializeField] private float _physicMinimalSpeed = 0.1f;

    private enum FullPhysicStat {
        Physic,Delay, TurnOff
    }

    private FullPhysicStat _stat;
    private float _delay;
    private float _timer;

    public void AddImpulseForce(Vector3 force) => _rigidbody.AddForce(force, ForceMode.Impulse);
    public void AddImpulseTorque(Vector3 torque) => _rigidbody.AddTorque(torque, ForceMode.Impulse);
    
    public void SetStartupDelay(float delay) {
        _delay = delay;
        _stat = FullPhysicStat.Delay;
        _timer = 0;
        _collider.enabled = false;
    }
    private void Update() {

        switch (_stat) {
            case FullPhysicStat.Physic:
                if (_rigidbody.linearVelocity.magnitude < _physicMinimalSpeed) {
                    _timer += Time.deltaTime;
                    if (_timer >= _physicMaxTime) {
                        StopPhysic();
                    }
                }
                else {
                    _timer = 0;
                }
                break;
            case FullPhysicStat.Delay:
                _timer += Time.deltaTime;
                if (_timer >= _delay) {
                    StopDelay();
                }
                break;
            case FullPhysicStat.TurnOff:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    

    private void StopDelay() {
        _stat = FullPhysicStat.Physic;
        _collider.enabled =true;
        _timer = 0;
    }
    
    private void StopPhysic() {
        _stat = FullPhysicStat.TurnOff;
        _collider.enabled = false;
        Destroy(_rigidbody);
        Destroy(this);
    }
    [ContextMenu("SetupRefsTool")]
    private void SetupRefsTool() {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }
}