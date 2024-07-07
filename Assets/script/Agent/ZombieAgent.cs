﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class ZombieAgent : GridAgent
    {
        //[SerializeField] private Rigidbody Rigidbody;
        //[SerializeField] private float _maxMoveSpeed;
        //[SerializeField] private float _moveSpeed;
        //[SerializeField] private float _speedModulator;
        //[SerializeField] private GridManager GridManager;
        //[Header("Zombie Reference")]
        //public GameObject SelectionSprite;
        //public GameObject PSEmoteRedSquare;
        [Header("Zombie Parameters")] 
        //[SerializeField] private int _hpMax = 3;
        //[SerializeField] private int _hp = 3;
        [SerializeField]private GameObject _prefabDeathPS;
        //[SerializeField] private Animator _animator;
        [Header("Attack Parameters")] 
        [SerializeField] private AttackStruct _attack;
        //[SerializeField] private float _attackDelay =2;
        //[SerializeField] private int _attackDamage =1;
        [SerializeField] private GameObject _prefabsAttackEffect;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;

        [Header("TransformationParameters")] 
        [SerializeField] private float _totalTransformationTime;
        [SerializeField] private float _transformationMod=1;
        [SerializeField] private ZombieAgent _prefabZombieAgent;

        [Header("HeightOffSetting")] 
        private IDestructible _desctructibleTarget;

        private GridAgent _grabbedTarget; 
        private float _attacktimer;
        private bool _isTransformting;
        private float _transformationTimer;


        protected override void Start() {
            StaticData.ZombieCount++;
            StaticData.AddZombie(this);
            StaticData.OnZombieGain?.Invoke();
            _audioSource.clip = _spawnSound[Random.Range(0, _spawnSound.Length)];
            _audioSource.Play();
            base.Start();
        }

        public void OnDestroy() {
            StaticData.ZombieLose();
            StaticData.RemoveZombie(this);
        } 
        public float GetNormalizeHp() => (float) _hp / _maxHp;
        public float GetNormalizeTransformation() =>  _transformationTimer / _totalTransformationTime;
        public bool IsTransforming() => _isTransformting;

        public void TakeDamage(int damage)
        {
            _hp -= damage;
            if (_hp <= 0) {
                Instantiate(_prefabDeathPS, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            _animator.SetBool("TakeDamage", true);
        }
       
        protected override void ManageAttack() {
            
            if(_desctructibleTarget!=null) ManageDestructibleAttack();
            
            if ((_desctructibleTarget==null||!_desctructibleTarget.IsAlive()))
            {
                _desctructibleTarget = null;
                ChangeStat(GridActorStat.Idle);
                Rigidbody.isKinematic = false;
                Debug.Log("AttackFinish");
            }
        }

        protected override void ManageTransformation()
        {
            if (_grabbedTarget == null) {
                Rigidbody.isKinematic = false;
                ChangeStat(GridActorStat.Idle);
            }
            _grabbedTarget.transform.forward = transform.position - _grabbedTarget.transform.position;
            transform.forward =  _grabbedTarget.transform.position -transform.position ;
            _grabbedTarget.transform.position = transform.position + transform.forward * 0.4f;
            
            
            _transformationTimer -= Time.deltaTime;
            if (_transformationTimer<=0) {
                TransformGrabbedTarget();
                return;
            }
            
            _animator.SetBool("IsGrabbing", true);
        }

       

        private void TransformGrabbedTarget()
        {
            ZombieAgent z =Instantiate(_prefabZombieAgent,  _grabbedTarget.transform.position, _grabbedTarget.transform.rotation);
            z.Generate(GridManager);
            z.SetNewSubGrid(Subgrid);
            Instantiate(_prefabDeathPS, transform.position, Quaternion.identity);
            Destroy(_grabbedTarget.gameObject);
            _animator.SetBool("IsGrabbing", false);
            ChangeStat(GridActorStat.Idle);
            if (IsSelected) GameController.AddAgentToSelection.Invoke(z);
            _isTransformting = false;
            
        }

        private void ManageDestructibleAttack() {
            _attacktimer += Time.deltaTime;
            if (_attacktimer >= _attack.AttackSpeed) {
                _desctructibleTarget.TakeDamage(_attack.GetDamage(Metrics.UniteType.Heavy));
                _attacktimer = 0;
                _animator.SetTrigger("Attack");
                _audioSource.clip = _attackSound[Random.Range(0, _attackSound.Length)];
                _audioSource.Play();
                Instantiate(_prefabsAttackEffect, transform.position, quaternion.identity);
            }
        }

        protected override void Update() {
            base.Update();
            _animator.SetFloat("Velocity", Rigidbody.velocity.magnitude/_maxMoveSpeed);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Destructible")) {
                if (other.gameObject.GetComponent<IDestructible>()!=null) {
                    _desctructibleTarget = other.gameObject.GetComponent<IDestructible>();
                    ChangeStat(GridActorStat.Attack);
                    Rigidbody.velocity = Vector3.zero;
                    transform.forward = other.transform.position -transform.position;
                    Rigidbody.isKinematic = true;
                    return;
                }
            }

            if (other.gameObject.GetComponent<GridAgent>() ) {
                GridAgent target = other.gameObject.GetComponent<GridAgent>();
                if (!target.CanBetransform || target.Stat == GridActorStat.Grabed||_grabbedTarget!=null) return;
                _grabbedTarget = target;
                _grabbedTarget.SetAsGrabbed();
                _transformationTimer =_totalTransformationTime= target.TransformTime*_transformationMod;
                _isTransformting = true;
                ChangeStat(GridActorStat.Transforming);
            }
        }

        protected override void GetToMoveTarget()
        {
            Debug.Log("Arrive at target");
            base.GetToMoveTarget();
        }
        
        protected override void ChangeStat(GridActorStat stat) {
            if (stat == GridActorStat.Move && (_desctructibleTarget != null||_grabbedTarget!=null))return;
            base.ChangeStat(stat);
        }

        public override void SetNewSubGrid(Subgrid subgrid) {
            //_audioSource.clip = _spawnSound[Random.Range(0, _spawnSound.Length)];
            //_audioSource.pitch = Random.Range(0.8f, 1.2f);
            //_audioSource.Play();
            AudioManager.Instance?.PlaySFX(_spawnSound[Random.Range(0, _spawnSound.Length)],1,Random.Range(0.8f, 1.2f));
            base.SetNewSubGrid(subgrid);
        }
    }
}