using System;
using UnityEngine;

public class VFXBirdTakeoff : MonoBehaviour
{
    [SerializeField] private Vector3 _dir;
    [SerializeField] private float _rotation = 10;
    [SerializeField] private float _upMotion = 2;
    [SerializeField] private float _forwardSpeed =2;
    

    [SerializeField] private bool _flyAway;
    private Vector3 _startPos;
    private Quaternion _startRot;
    private Animator _animator;
    void Start() {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _animator = GetComponent<Animator>();
    }


    void Update() {
        
        _animator.SetBool("IsFlying", _flyAway);
        if (_flyAway) {
            ManageFlyAway();
        }
    }

    public void SetReadyToFlyAway(Vector3 dir, float delay) {
        _dir = dir;
        Invoke("DelayFlyAway", delay);
    }

    private void DelayFlyAway() {
        _flyAway = true;
    }
    private void ManageFlyAway() {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(_dir),_rotation*Time.deltaTime);
        transform.position = (transform.position + transform.forward * _forwardSpeed * Time.deltaTime) +new Vector3(0,_upMotion*Time.deltaTime,0);
    }
    
    [ContextMenu("Reset To Default Value;")]
    public void ResetDefaultPos() {
        transform.position = _startPos;
        transform.rotation = _startRot;
        _flyAway = false;
        _animator.SetBool("IsFlying", _flyAway);
    }

    
}