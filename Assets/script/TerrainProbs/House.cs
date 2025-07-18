using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class House: MonoBehaviour, IDestructible
    {
        [SerializeField]private GameObject prefabsDebrie;
        [SerializeField]private GridManager GridManager;
        [SerializeField]private Metrics.UniteType _uniteType = Metrics.UniteType.Heavy; 
        [SerializeField]private int HP;
        [SerializeField]private GameObject PrefabsDestruciotnParticules;
        [SerializeField]private List<Vector2Int> CellsCoordinates = new List<Vector2Int>();
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [Header("Spawn Parameters")]
        [SerializeField]private int zombitToSpawn = 4;
        [SerializeField]private Vector3 SpawnOffset = new Vector3(0, 0.5f, 0);
        [SerializeField]private float RandomRange = 1;
        [Space(5)]
        [SerializeField]private float _zombieSpawnChanceStandrard =0.7f;
        [SerializeField]private float _zombieSpawnChanceBrute =0.2f;
        [SerializeField]private float _zombieSpawnChanceEngineer =0.1f;
        [Header(("Sounds"))] public AudioSource AudioSource;
        [SerializeField]private AudioClip[] HitSounds;

        private int _zombieToSpawnStandard = 0;
        private int _zombieToSpawnBrute = 0;
        private int _zombieToSpawnEngineer = 0;
        
        [ExecuteInEditMode]
        public void Awake() {
            GridManager = GridManager.Instance;
            GridManager.OnClearPathFindingData += ClearCellCoordinateData;
            if(!GridManager) Debug.LogWarning(" GridManager non Assigner sur Maison "+name);
            StaticData.AddBuilding(this);
        }

        private void Start() {
            SetZombieCountToSpawn();
        }


        public void ClearCellCoordinateData() => CellsCoordinates.Clear();
        
        public void TakeDamage(int damage, GridAgent source = null) {
            if (HP <= 0) return;
            HP -= damage;
            if (HP <= 0) {
                DestroyDestructible(source);
            }
        }

        public void DestroyDestructible(GridAgent source = null)
        {
            if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
            if( PrefabsDestruciotnParticules)Instantiate(PrefabsDestruciotnParticules, transform.position, transform.rotation);
            ManageZombieSpawn(source);
            StaticData.BuildingDestroy();
            if (_impulseSource) _impulseSource.GenerateImpulse();
            GetComponentInChildren<Renderer>().enabled=false;
            GetComponentInChildren<Collider>().enabled=false;
            Destroy(gameObject,0.5f);
        }
        [ExecuteInEditMode]
        private void OnDestroy()
        {
            GridManager.OnClearPathFindingData -= ClearCellCoordinateData;
        }

        public bool IsAlive()
        {
            return (HP > 0);
        }

        private void ManageZombieSpawn(GridAgent source = null) {
            ZombieAgent zSource=null;
            if (source != null && source is ZombieAgent) {
                zSource = source as ZombieAgent;
                
            }
            
            for (int i = 0; i < _zombieToSpawnStandard; i++) {
                Vector3 pos = transform.position + SpawnOffset + new Vector3(Random.Range(-RandomRange, RandomRange), 0,
                    Random.Range(-RandomRange, RandomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieStandardAgent, pos, Quaternion.identity);
                zombie.Generate(GridManager);
                if( zSource!=null &&zSource.IsSelected)GameController.AddAgentToSelection.Invoke(zombie);
            }
            for (int i = 0; i < _zombieToSpawnBrute; i++) {
                Vector3 pos = transform.position + SpawnOffset + new Vector3(Random.Range(-RandomRange, RandomRange), 0,
                    Random.Range(-RandomRange, RandomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieBruteAgent, pos, Quaternion.identity);
                zombie.Generate(GridManager);
                if( zSource!=null &&zSource.IsSelected)GameController.AddAgentToSelection.Invoke(zombie);
            }
            for (int i = 0; i < _zombieToSpawnEngineer; i++) {
                Vector3 pos = transform.position + SpawnOffset + new Vector3(Random.Range(-RandomRange, RandomRange), 0,
                    Random.Range(-RandomRange, RandomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieEngineerAgent, pos, Quaternion.identity);
                zombie.Generate(GridManager);
                if( zSource!=null &&zSource.IsSelected)GameController.AddAgentToSelection.Invoke(zombie);
            }
        }
        [ContextMenu("NormalizeSpawnValues")]
        private void NormalizeSpawnValues() {
            float e = _zombieSpawnChanceBrute + _zombieSpawnChanceEngineer + _zombieSpawnChanceStandrard;
            _zombieSpawnChanceStandrard = _zombieSpawnChanceStandrard * ( 1/e);
            _zombieSpawnChanceBrute = _zombieSpawnChanceBrute * (1/e);
            _zombieSpawnChanceEngineer = _zombieSpawnChanceEngineer * (1/e);
        }

        private void SetZombieCountToSpawn() {
            _zombieSpawnChanceStandrard = _zombieSpawnChanceStandrard * StaticData.ZombieSpawnChangeStandrard;
            _zombieSpawnChanceBrute = _zombieSpawnChanceBrute *  StaticData.ZombieSpawnChangeBrute;
            _zombieSpawnChanceEngineer = _zombieSpawnChanceEngineer *  StaticData.ZombieSpawnChangeEngineer;
            NormalizeSpawnValues();
            for (int i = 0; i < zombitToSpawn; i++)
            {
                float value = Random.Range(0, 1f);
                
                if (value < _zombieSpawnChanceBrute) _zombieToSpawnStandard++;
                
                if (value >= _zombieSpawnChanceStandrard &&
                    value < _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard) 
                    _zombieToSpawnBrute++;
                
                if (value >= _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard &&
                    value < _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard+_zombieSpawnChanceEngineer) 
                    _zombieToSpawnEngineer++;
                
            }
            StaticData.AddInHouseCivilians(zombitToSpawn);
            
            
        }
    }
}