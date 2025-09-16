using System;
using System.Collections.Generic;
using script;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoTurrent : Destructible
{
    [FormerlySerializedAs("_isPowerOff")] [SerializeField] private bool _isPowered;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private Transform _turrentHead;
    [Header("AttackParameters")] 
    [SerializeField] protected float _attackRange;
    [SerializeField] protected ParticleSystem _pSAttack;
    [SerializeField] protected ZombieAgent _target;
    [SerializeField] protected TriggerZoneDetector _triggerZoneDetector;
    [SerializeField] protected AttackStruct _attack;

    [Header("IdleAniamtion")] 
    [SerializeField] private float _idlePos1 = -30;
    [SerializeField] private float _idlePos2 = 30;
    [SerializeField] private float _idleCicleSpeed = 3;

    [Header("PowerOfPos")] 
    [SerializeField] private Vector3 PowerOffPow;
    [SerializeField] private float _maxRadialDelta =1;
    [SerializeField] private float _maxMagnitudeDelta =1;
    
    protected float _timer;
    private bool _isIdleReverce;
    private float _idleTimer;
    [SerializeField]private Vector3 _turretHeadTarget;

    private void Start() {
        SetPowerToTheTurret(_isPowered);
    }

    private void Update()
    {
        if (_isPowered)
        {
            if (_target == null) {
                ManageLookingForTarget();
            }
            if (_target != null) ManageAttack();
            else DoIdleAnimation();
        }
        MakeTurretHeadFollow();
    }

    public void SetPowerToTheTurret(bool value) {
        _isPowered = value;
        if (!value) {
            _turretHeadTarget = _turrentHead.forward+ new Vector3(0, -1, 0);
        }
    }

    protected virtual void ManageLookingForTarget()
    {
        _triggerZoneDetector.CheckOfNull();

        if (_triggerZoneDetector.Zombis.Count <= 0) return;
        _target = GetTheClosest(_triggerZoneDetector.Zombis);
        if (_target != null)
        {
            _isAttacking = true;
        }
    }

    protected virtual ZombieAgent GetTheClosest(List<ZombieAgent> zombies) {
        ZombieAgent z = null;
        float distance = Mathf.Infinity;
        foreach (var zombie in zombies) {
            if (distance > Vector3.Distance(zombie.transform.position, transform.position)) {
                z = zombie;
                distance = Vector3.Distance(zombie.transform.position, transform.position);
            }
        }
        return z;
    }
    protected void ManageAttack() {
        if (_target == null) ManageLookingForTarget();
        if (_target == null) {
            _isAttacking = false;
            return;
        }
        ManagerFire();
    }
    protected void ManagerFire() {
        if (_attackRange < Vector3.Distance(transform.position, _target.transform.position)) {
            _target = null;
            return;
        }

        
        _turretHeadTarget = _target.transform.position - _turrentHead.position;
        _timer += Time.deltaTime;
        if (_timer >= _attack.AttackSpeed) {
            _target.TakeDamage(_attack.GetDamage(_target.UniteType));
            AlertSystemManager.Instance?.FireShot();
            _pSAttack.Play();
           //AudioSource.clip = ShotingSound[Random.Range(0, ShotingSound.Length)];
            //AudioSource.Play();
            _timer = 0;
        }
    }

    private void DoIdleAnimation()
    {
        if (_isIdleReverce) {
            _idleTimer -= Time.deltaTime;
            if (_idleTimer < 0) _isIdleReverce = false;
        }
        else {
            _idleTimer += Time.deltaTime;
            if (_idleTimer > _idleCicleSpeed) _isIdleReverce = true;
        }
        _turretHeadTarget =  _turrentHead.position+rotate(transform.forward, Mathf.Lerp(_idlePos1, _idlePos2, _idleTimer/_idleCicleSpeed)) - _turrentHead.position;
    }

    
    public static Vector3 rotate(Vector3 v, float delta)
    {
        delta = delta * Mathf.Deg2Rad;
        return new Vector3(
            v.x * Mathf.Cos(delta) - v.z * Mathf.Sin(delta),0,
            v.x * Mathf.Sin(delta) + v.z * Mathf.Cos(delta)
        );
    }

    private void MakeTurretHeadFollow() {
        _turrentHead.forward = Vector3.RotateTowards(_turrentHead.forward, _turretHeadTarget, _maxRadialDelta * Time.deltaTime,
            _maxMagnitudeDelta * Time.deltaTime);
    }


    private void OnDrawGizmos() {
        Debug.DrawLine(_turrentHead.position, _turrentHead.position+rotate(transform.forward, _idlePos1),Color.red);
        Debug.DrawLine(_turrentHead.position, _turrentHead.position+rotate(transform.forward, _idlePos2),Color.red);
    }
}
