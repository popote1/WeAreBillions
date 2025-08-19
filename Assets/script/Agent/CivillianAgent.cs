using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class CivillianAgent : GridAgent
    {
        
        [Header("Agent Parameters")] 
        [SerializeField]private GameObject _prefabDeathPS;

        [Space(5)] 
        
        [SerializeField] private GameObject _prfEmoteExost;
        [SerializeField] private GameObject _prfEmoteSuprice;
        [Space(5)] 
        [SerializeField] public float DetectionDistance =10;
        [SerializeField] private TriggerZoneDetector TriggerZoneDetector;
        [SerializeField] private int RunAwayDistance = 10;
        [Space(5)] 
        [SerializeField] private float MaxStamina = 10;
        [SerializeField] private float RunAwayMoveSpeed =30;
        [SerializeField] private float StaminaRegenRate = 0.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _attackSound;
        [SerializeField] private AudioClip[] _spawnSound;
        [SerializeField] private AudioClip[] _dieSound;
        
        private float _currentMoveSpeed;
        private float _currentStamina;
        
        public bool IsRunAway;
        protected override void Start() {
            _currentMoveSpeed = _moveSpeed;
            TriggerZoneDetector.MaxDistance = DetectionDistance;
            _currentStamina = MaxStamina;
            base.Start();
        }

        public float GetCurrentStaminaStat() => _currentStamina / MaxStamina;
        public float GetDebugMoveSpeed() => GetMoveSpeed();
        
        protected override void Update() {
            ManageRunAway();
            ManageStamina();
            base.Update();
        }
      

        protected override void GetToMoveTarget()
        {
            if (Stat == GridActorStat.Grabed) return;
            IsRunAway = false;
            ManageRunAway();
            if(!IsRunAway){
                base.GetToMoveTarget();
            }
        }

        private void ManageRunAway()
        {
            if (!IsRunAway) {
                TriggerZoneDetector.CheckOfNull();
                if (TriggerZoneDetector.Zombis.Count > 0) {
                    ZombieAgent zombi = TriggerZoneDetector.GetTheClosest();
                    Cell[] cells = GridManager.GetBreathFirstCells(GridManager.GetCellFromWorldPos(transform.position),
                        RunAwayDistance);
                    _currentMoveSpeed = RunAwayMoveSpeed;
                    SpawnEmote(_prfEmoteSuprice);
                    IsRunAway = true;
                    SetNewMoveDestination(GetFarCellFrom(cells, zombi.transform));
                }
            }
        }

        private Cell GetFarCellFrom(Cell[] cells, Transform target){
            Vector2Int targetPos = GridManager.GetCellFromWorldPos(target.position).Pos; 
            float bestDist = float.MinValue;
            Cell bestcell = null;
            foreach (var cell in cells) {
                if (Vector2.Distance(targetPos, cell.Pos) > bestDist) {
                    bestcell = cell;
                    bestDist = Vector2.Distance(targetPos, cell.Pos);
                }
            }

            return bestcell;
        }
        
        private void ManageStamina() {
            if (IsRunAway && _currentStamina>0) {
                _currentStamina = Mathf.Clamp(_currentStamina -Time.deltaTime,0,MaxStamina);
                if (_currentStamina == 0) {
                   SpawnEmote(_prfEmoteExost);
                }
            }
            if (!IsRunAway && _currentStamina < MaxStamina) {
                _currentStamina = Mathf.Clamp(_currentStamina + Time.deltaTime * StaminaRegenRate,0,MaxStamina);
            }
        }

        protected override float GetMoveSpeed() {
            if (IsRunAway && _currentStamina > 0) return RunAwayMoveSpeed;
            return _moveSpeed;
        }

        private void OnDrawGizmos()
        {
            if (EditorControlStatics.DisplayDetectionZone) {
                Gizmos.color = new Color(0, 0.5f, 1, 0.25f);
                Gizmos.DrawSphere(transform.position , DetectionDistance);
            }
        }

        private void SpawnEmote(GameObject emote)
        {
            if (emote == null) return;
            GameObject go =Instantiate(emote, _transformEmote);
            go.transform.localPosition = Vector3.zero;
        }
    }
}
