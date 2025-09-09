using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    public class PoliceMan:Defender {
        [Header("Sound")]
        public AudioSource AudioSource;
        public AudioClip[] ShotingSound;
        private float _timer;
        protected override void Start() {
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
        
        protected override void ManageAttack() {
            if (_target == null) ManageLookingForTarget();
            if (_target == null) {
                ChangeStat(GridActorStat.Idle);
                return;
            }
            ManagerFire();
        }

        protected void ManagerFire() {
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