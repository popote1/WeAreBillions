using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    public class PoliceMan:Defender
    {
        
        
        [Header("Sound")]
        public AudioSource AudioSource;
        public AudioClip[] ShotingSound;
        public AudioClip[] GettingTarget;
        public AudioClip[] GetKill;
        
        
        private float _timer;

        protected override void Start()
        {
            _triggerZoneDetector.MaxDistance = _attackRange;
            _triggerZoneDetector.transform.GetComponent<SphereCollider>().radius = _attackRange;
            base.Start();
        }

        protected override void Update() {
            
            _animator.SetBool("HaveTarget", _target!= null);
            base.Update();
        }

        protected override void ManageStat() {
            if (Stat == GridActorStat.CallingAlert) ManageAlertCalling();
            base.ManageStat();
            if (Stat == GridActorStat.Idle || Stat == GridActorStat.Move) { 
                ManageLookingForTarget();
            }
        }

        private void ManageLookingForTarget() {
            _triggerZoneDetector.CheckOfNull();
            
            if (_triggerZoneDetector.Zombis.Count <= 0) return;
            if (CheckForAlertCalling()) {
                StartAlertCalling();
                AudioSource.clip = GettingTarget[Random.Range(0, GettingTarget.Length)];
                return;
            }
            
            _target =GetTheClosest(_triggerZoneDetector.Zombis);
            AudioSource.clip = GettingTarget[Random.Range(0, GettingTarget.Length)];
            AudioSource.Play();
            ChangeStat(GridActorStat.Attack);

        }

        protected override void ManageAttack()
        {
            if (_target == null) ManageLookingForTarget();
            if (_target == null) {
                ChangeStat(GridActorStat.Idle);
                return;
            }
            ManagerFire();
        }

        public void ManagerFire() {
            
            if (_attackRange < Vector3.Distance(transform.position, _target.transform.position)) {
                _target = null;
                return;
            }

            transform.forward = _target.transform.position - transform.position;
            _timer += Time.deltaTime;
            if (_timer >= _attack.AttackSpeed) {
                _target.TakeDamage(_attack.GetDamage(_target.UniteType));
                AlertSystemManager.Instance?.FireShot();
                _pSAttack.Play();
                AudioSource.clip = ShotingSound[Random.Range(0, ShotingSound.Length)];
                AudioSource.Play();
                _timer = 0;
            }
        }

        public ZombieAgent GetTheClosest(List<ZombieAgent> zombies) {
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

        //private void OnCollisionEnter(Collision other) {
        //    if (other.gameObject.CompareTag("Zombi")) {
        //        ZombieAgent z =Instantiate(PrefabsZombieAgent, transform.position, quaternion.identity);
        //        z.Generate(GridManager);
        //        Instantiate(PrefabsDeathPS, transform.position, Quaternion.identity);
        //        AudioSource.PlayClipAtPoint(GetKill[Random.Range(0,GetKill.Length)], transform.position);
        //        DestroyBuilding(gameObject);
        //    }
        //}

        private void OnDrawGizmos() {
            if (EditorControlStatics.DisplayEnnemisDangerZone) {
                Gizmos.color = Color.red * new Color(1, 1, 1, 0.4f);
                Gizmos.DrawSphere(transform.position,  _attackRange);
                
                Gizmos.color = Color.green * new Color(1, 1, 1, 0.4f);
                Gizmos.DrawWireSphere(transform.position,  _alertCheckRadius);
            }
        }
    }
}