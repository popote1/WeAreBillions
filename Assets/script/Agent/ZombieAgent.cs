using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class ZombieAgent : GridAgent {
        [Header("Zombie Parameters")] 
        [SerializeField]private GameObject _prefabDeathPS;
        [SerializeField] private VFXBloodSpalterController _prefabBloodSplater; 
        [Header("Attack Parameters")] 
        [SerializeField] private AttackStruct _attack;
        [SerializeField] private GameObject _prefabsAttackEffect;
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;

        [Header("TransformationParameters")] 
        [SerializeField] private TransformStruct _transformData;

        [Header("HeightOffSetting")] 
        private IDestructible _desctructibleTarget;

        private GridAgent _agentTarget; 
        private float _attacktimer;
        private bool _isTransformting;
        private float _transformationTimer;
        private float _totalTransformationTime;

        public AttackStruct Attack => _attack;


        protected override void Start() {
            StaticData.ZombieCount++;
            StaticData.AddZombie(this);
            StaticEvents.OnZombieGain?.Invoke();
            _audioSource.clip = _spawnSound[Random.Range(0, _spawnSound.Length)];
            _audioSource.Play();
            base.Start();
        }

        protected override void OnDestroy() {
            StaticEvents.ZombieLose();
            StaticData.RemoveZombie(this);
            base.OnDestroy();
        } 
        public float GetNormalizeHp() => (float) _hp / _maxHp;
        public float GetNormalizeTransformation() =>  _transformationTimer / _totalTransformationTime;
        public bool IsTransforming() => _isTransformting;

        

        public override void KillAgent() {
            if( _agentTarget!=null)_agentTarget.SetAsGrabbed(false);
            Instantiate(_prefabDeathPS, transform.position, Quaternion.identity);
            if(_prefabBloodSplater!=null){
                VFXBloodSpalterController blood =Instantiate(_prefabBloodSplater, transform.position+new Vector3(0,0.5f,0), Quaternion.identity);
                blood.transform.forward = Vector3.down;
                blood.transform.Rotate(Vector3.forward, Random.Range(0,360));
            }
            base.KillAgent();
            
        }

        protected override void ManageAttack() {
            
            if(_desctructibleTarget!=null) ManageDestructibleAttack();
            
            if ((_desctructibleTarget==null||!_desctructibleTarget.IsAlive())) {
                _desctructibleTarget = null;
                ChangeStat(GridActorStat.Idle);
                Rigidbody.isKinematic = false;
                Debug.Log("AttackFinish");
            }
        }

        protected override void ManageTransformation() {
            if (_agentTarget == null) {
                Rigidbody.isKinematic = false;
                ChangeStat(GridActorStat.Idle);
            }
            _agentTarget.transform.forward = transform.position - _agentTarget.transform.position+new Vector3(0,0.5f,0);
            transform.forward =  (_agentTarget.transform.position-new Vector3(0,0.5f,0)) -transform.position ;
            _agentTarget.transform.position = transform.position + transform.forward * 0.4f;
            
            
            _transformationTimer -= Time.deltaTime;
            if (_transformationTimer<=0) {
                TransformGrabbedTarget();
                return;
            }
            
            _animator.SetBool("IsGrabbing", true);
        }
      
        protected virtual void TransformGrabbedTarget() {
            ZombieAgent z =Instantiate(StaticData.PrefabZombieStandardAgent,  _agentTarget.transform.position, _agentTarget.transform.rotation);
            z.Generate(GridManager);
            z.SetNewSubGrid(Subgrid);
            Instantiate(_prefabDeathPS, transform.position, Quaternion.identity);
            
            if (_agentTarget is CivillianAgent) StaticData.AddCivilianKill();
            else StaticData.AddDefenderKill();
            
            _agentTarget.KillAgent();
            
            
            _animator.SetBool("IsGrabbing", false);
            ChangeStat(GridActorStat.Idle);
            if (IsSelected) GameController.AddAgentToSelection.Invoke(z);
            _isTransformting = false;
            
        }

        private void ManageDestructibleAttack() {
            _attacktimer += Time.deltaTime;
            if (_attacktimer >= _attack.AttackSpeed) {
                _desctructibleTarget.TakeDamage(_attack.GetDamage(Metrics.UniteType.Heavy), this);
                _attacktimer = 0;
                PlayerAttackEffect();
            }
        }

        protected override void ManageMovingAttack()
        {
            if (_agentTarget == null) {
                Debug.Log("TagetAgent not found");
                ChangeStat(GridActorStat.Idle);
                _agentTarget = null;
                return;
            }
            RaycastHit[] Hits = Physics.RaycastAll(transform.position, _agentTarget.transform.position - transform.position);
            Vector3 hitPos = new Vector3(5000, 5000, 5000);
            foreach (var hit in Hits) {
                if (hit.transform.GetComponent<GridAgent>()==null) continue;
                hitPos = hit.point;
                Debug.Log("attack distance  = " + Vector3.Distance(hitPos, transform.position));
            }

            if (Vector3.Distance(hitPos, transform.position) > 20) {
                _agentTarget = null;
                ChangeStat(GridActorStat.Idle);
                
                Debug.Log("Target To Far");
            }
            else if(Vector3.Distance(hitPos, transform.position) <= 3)
            {
                Debug.Log("Target InAttackRange");
                ManageAttackTimer();
                if (_attacktimer >= _attack.AttackSpeed) {
                    _agentTarget.TakeDamage(_attack.GetDamage(_agentTarget.UniteType));
                    _attacktimer = 0;
                    Vector3 forward = hitPos - transform.position;
                    forward.y = 0;
                    transform.forward = forward;
                    PlayerAttackEffect();
                    Debug.Log("Attack !!");
                }
            }
            else {
                SetNewMoveDestination(GridManager.GetCellFromWorldPos(_agentTarget.transform.position));
                _agentTarget = null;
                ChangeStat(GridActorStat.Move);
                
                Debug.Log("Target in chasse range");
            }
            base.ManageMovingAttack();
        }

        protected override void Update() {
            base.Update();
            _animator.SetFloat("Velocity", Rigidbody.linearVelocity.magnitude/_maxMoveSpeed);
            if (Stat!=GridActorStat.Attack && Stat!= GridActorStat.MovingAttack)ManageAttackTimer();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!IsFreeToTakeTarget()) return;
            if (other.gameObject.CompareTag("Destructible")) {
                if (other.gameObject.GetComponent<IDestructible>()!=null) {
                    _desctructibleTarget = other.gameObject.GetComponent<IDestructible>();
                    ChangeStat(GridActorStat.Attack);
                    Rigidbody.linearVelocity = Vector3.zero;
                    transform.forward = other.transform.position -transform.position;
                    Rigidbody.isKinematic = true;
                    return;
                }
            }

            if (other.gameObject.GetComponent<GridAgent>() ) {
                GridAgent target = other.gameObject.GetComponent<GridAgent>();
                
                if (target.CanBeDestroy) {
                    StartMoveAttack(target);
                    return;
                }
                
                if (!target.CanBetransform || target.Stat == GridActorStat.Grabed||_agentTarget!=null) return;
                StartTransformation(target);
            }
        }

        protected virtual void StartTransformation(GridAgent target) {
            _agentTarget = target;
            _agentTarget.SetAsGrabbed();
            _transformationTimer =_totalTransformationTime= target.TransformTime*_transformData.GetTransformationMod(target.UniteType);
            _isTransformting = true;
            ChangeStat(GridActorStat.Transforming);
        }

        protected virtual void StartMoveAttack(GridAgent target) {
            _agentTarget = target;
            ChangeStat(GridActorStat.MovingAttack);
        }

        protected override void GetToMoveTarget() {
            Debug.Log("Arrive at target");
            base.GetToMoveTarget();
        }
        
        protected override void ChangeStat(GridActorStat stat) {
            if (stat == GridActorStat.Move && (_desctructibleTarget != null||_agentTarget!=null))return;
            base.ChangeStat(stat);
        }

        public override void SetNewSubGrid(Subgrid subgrid) {
            AudioManager.Instance?.PlaySFX(_spawnSound[Random.Range(0, _spawnSound.Length)],1,Random.Range(0.8f, 1.2f));
            base.SetNewSubGrid(subgrid);
        }

        private bool IsFreeToTakeTarget() {
            return Stat != GridActorStat.Transforming && Stat != GridActorStat.Attack &&
                   Stat != GridActorStat.MovingAttack;
        }

        private void ManageAttackTimer() {
            if (_attacktimer< _attack.AttackSpeed) _attacktimer += Time.deltaTime;
        }

        private void PlayerAttackEffect() {
            _animator.SetTrigger("Attack");
            _audioSource.clip = _attackSound[Random.Range(0, _attackSound.Length)];
            _audioSource.Play();
            Instantiate(_prefabsAttackEffect, transform.position, quaternion.identity);
        }
    }
}