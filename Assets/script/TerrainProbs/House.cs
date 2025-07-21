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
        [SerializeField]private GameObject _prefabsDebrie;
        [SerializeField]private GridManager _gridManager;
        [SerializeField]private Metrics.UniteType _uniteType = Metrics.UniteType.Heavy; 
        [SerializeField]private int _hp;
        [SerializeField]private GameObject _prefabsDestruciotnParticules;
        [SerializeField]private List<Vector2Int> _cellsCoordinates = new List<Vector2Int>();
        [SerializeField]private CinemachineImpulseSource _impulseSource;
        [Header("Spawn Parameters")]
        [SerializeField]private int _zombitToSpawn = 4;
        [SerializeField]private Vector3 _spawnOffset = new Vector3(0, 0.5f, 0);
        [SerializeField]private float _randomRange = 1;
        [SerializeField] private Bounds _spawningBounds;
        [Space(5)]
        [SerializeField]private float _zombieSpawnChanceStandrard =0.7f;
        [SerializeField]private float _zombieSpawnChanceBrute =0.2f;
        [SerializeField]private float _zombieSpawnChanceEngineer =0.1f;
        [Header(("Sounds"))] public AudioSource AudioSource;
        [SerializeField]private AudioClip[] _hitSounds;
        
        private int _zombieToSpawnStandard = 0;
        private int _zombieToSpawnBrute = 0;
        private int _zombieToSpawnEngineer = 0;

        public Bounds SpawningBound {
            get => _spawningBounds;
            set { _spawningBounds = value; }
        }

        [ExecuteInEditMode]
        public void Awake() {
            _gridManager = GridManager.Instance;
            GridManager.OnClearPathFindingData += ClearCellCoordinateData;
            if(!_gridManager) Debug.LogWarning(" GridManager non Assigner sur Maison "+name);
            StaticData.AddBuilding(this);
        }

        private void Start() {
            SetZombieCountToSpawn();
        }


        public void ClearCellCoordinateData() => _cellsCoordinates.Clear();
        
        public void TakeDamage(int damage, GridAgent source = null) {
            if (_hp <= 0) return;
            _hp -= damage;
            if (_hp <= 0) {
                DestroyDestructible(source);
            }
        }

        public void DestroyDestructible(GridAgent source = null)
        {
            if (_prefabsDebrie) Instantiate(_prefabsDebrie, transform.position, transform.rotation);
            if( _prefabsDestruciotnParticules)Instantiate(_prefabsDestruciotnParticules, transform.position, transform.rotation);
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
            return (_hp > 0);
        }

        private void ManageZombieSpawn(GridAgent source = null) {
            ZombieAgent zSource=null;
            if (source != null && source is ZombieAgent) {
                zSource = source as ZombieAgent;
                
            }
            
            for (int i = 0; i < _zombieToSpawnStandard; i++) {
                Vector3 pos = transform.position + _spawnOffset + new Vector3(Random.Range(-_randomRange, _randomRange), 0,
                    Random.Range(-_randomRange, _randomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieStandardAgent, pos, Quaternion.identity);
                zombie.Generate(_gridManager);
                if( zSource!=null &&zSource.IsSelected)GameController.AddAgentToSelection.Invoke(zombie);
            }
            for (int i = 0; i < _zombieToSpawnBrute; i++) {
                Vector3 pos = transform.position + _spawnOffset + new Vector3(Random.Range(-_randomRange, _randomRange), 0,
                    Random.Range(-_randomRange, _randomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieBruteAgent, pos, Quaternion.identity);
                zombie.Generate(_gridManager);
                if( zSource!=null &&zSource.IsSelected)GameController.AddAgentToSelection.Invoke(zombie);
            }
            for (int i = 0; i < _zombieToSpawnEngineer; i++) {
                Vector3 pos = transform.position + _spawnOffset + new Vector3(Random.Range(-_randomRange, _randomRange), 0,
                    Random.Range(-_randomRange, _randomRange));
                ZombieAgent  zombie =Instantiate(StaticData.PrefabZombieEngineerAgent, pos, Quaternion.identity);
                zombie.Generate(_gridManager);
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
            for (int i = 0; i < _zombitToSpawn; i++)
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
            StaticData.AddInHouseCivilians(_zombitToSpawn);
            
            
        }

        
    }
    
}