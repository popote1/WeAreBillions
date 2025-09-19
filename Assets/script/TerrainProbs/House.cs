using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace script
{
    [SelectionBase]
    public class House: MonoBehaviour, IDestructible
    {
        [SerializeField]private GridManager _gridManager;
        [SerializeField]private Metrics.UniteType _uniteType = Metrics.UniteType.Heavy; 
        [SerializeField]private int _hp;
        [SerializeField]private List<Vector2Int> _cellsCoordinates = new List<Vector2Int>();
        [SerializeField]private CinemachineImpulseSource _impulseSource;
        [SerializeField] private VFXBuildingDestruction _vfxBuildingDestruction;
        [Header("Spawn Parameters")]
        [SerializeField]private int _zombitToSpawn = 4;
        [SerializeField]private Vector3 _spawnOffset = new Vector3(0, 0.7f, 0);
        [SerializeField] private LayerMask _spawningExclutionLayerMask;
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private Bounds _spawningBounds;
        [Space(5)]
        [SerializeField]private float _zombieSpawnChanceStandrard =0.7f;
        [SerializeField]private float _zombieSpawnChanceBrute =0.2f;
        [SerializeField]private float _zombieSpawnChanceEngineer =0.1f;
        
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

        public event EventHandler OnDestructibleDestroy;

        public void TakeDamage(int damage, GridAgent source = null) {
            if (_hp <= 0) return;
            _hp -= damage;
            if (_hp <= 0) {
                DestroyDestructible(source);
            }
        }

        [ContextMenu("Test Building Destruction")]
        public void TestDestruiction()
        {
            DestroyDestructible();
        }
        public void DestroyDestructible(GridAgent source = null) {
            StaticData.BuildingDestroy();
            if (_impulseSource) _impulseSource.GenerateImpulse();
            foreach (var coll in  GetComponentsInChildren<Collider>())
            {
                coll.enabled = false;
            }
            ManageZombieSpawn(source);
            _vfxBuildingDestruction.StartDestruction();
            OnDestructibleDestroy?.Invoke(this, EventArgs.Empty);
            OnDestroy();
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

        public Vector3 GetPosition() =>transform.position;
        

        private void ManageZombieSpawn(GridAgent source = null) {
            ZombieAgent zSource=null;
            if (source != null && source is ZombieAgent) {
                zSource = source as ZombieAgent;
            }
            for (int i = 0; i < _zombieToSpawnStandard; i++) {
                Spawnzombie(SpawnElement(StaticData.PrefabZombieStandardAgent), zSource);
            }
            for (int i = 0; i < _zombieToSpawnBrute; i++) {
                Spawnzombie(SpawnElement(StaticData.PrefabZombieBruteAgent), zSource);
            }
            for (int i = 0; i < _zombieToSpawnEngineer; i++) {
                Spawnzombie(SpawnElement(StaticData.PrefabZombieEngineerAgent), zSource);
            }
        }

        private void Spawnzombie(ZombieAgent zombie, ZombieAgent zSource ) {
            if (zombie == null) return;
            zombie.Generate(_gridManager);
            if( zSource!=null &&zSource.IsSelected)StaticEvents.AddAgentToSelection(zombie);
            if( zSource!=null)zombie.SetNewSubGrid(zSource.Subgrid);
        }
        
        [ContextMenu("NormalizeSpawnValues")]
        private void NormalizeSpawnValues() {
            float e = _zombieSpawnChanceBrute + _zombieSpawnChanceEngineer + _zombieSpawnChanceStandrard;
            _zombieSpawnChanceStandrard = _zombieSpawnChanceStandrard * ( 1/e);
            _zombieSpawnChanceBrute = _zombieSpawnChanceBrute * (1/e);
            _zombieSpawnChanceEngineer = _zombieSpawnChanceEngineer * (1/e);
        }
        [ContextMenu("Set Zombie Count")]
        private void SetZombieCountToSpawn() {
            _zombieSpawnChanceStandrard = _zombieSpawnChanceStandrard * StaticData.ZombieSpawnChangeStandrard;
            _zombieSpawnChanceBrute = _zombieSpawnChanceBrute *  StaticData.ZombieSpawnChangeBrute;
            _zombieSpawnChanceEngineer = _zombieSpawnChanceEngineer *  StaticData.ZombieSpawnChangeEngineer;
            NormalizeSpawnValues();
            _zombieToSpawnStandard = 0;
            _zombieToSpawnBrute = 0;
            _zombieToSpawnEngineer = 0;
            for (int i = 0; i < _zombitToSpawn; i++)
            {
                float value = Random.Range(0, 1f);
                
                if (value < _zombieSpawnChanceStandrard) _zombieToSpawnStandard++;
                else if (value >= _zombieSpawnChanceStandrard &&
                    value < _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard) 
                    _zombieToSpawnBrute++;
                else if (value >= _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard &&
                    value < _zombieSpawnChanceBrute + _zombieSpawnChanceStandrard+_zombieSpawnChanceEngineer) 
                    _zombieToSpawnEngineer++;
                
            }
            StaticData.AddInHouseCivilians(_zombitToSpawn);
        }

        
        private ZombieAgent SpawnElement(ZombieAgent prfZombie)
        {
            Vector3 pos;
            RaycastHit hit;
            for (int i = 0; i < 50; i++) {
                pos = GetRandomPosInSpawnBounds();
                if(Physics.Raycast(new Ray(pos+new Vector3(0,10,0),Vector3.down), out hit, 20, _groundLayerMask)){
                    pos = hit.point +_spawnOffset;
                    if (CanSpawnTestElement(pos, prfZombie.Radius)) {
                        Debug.Log("SpawnZombie");
                        return Instantiate(prfZombie, pos, Quaternion.identity);
                    }
                }
            }

            return null;
        }

        private bool CanSpawnTestElement(Vector3 pos, float radius) {
            Collider[] cols =Physics.OverlapSphere(pos, radius, _spawningExclutionLayerMask);
            if (cols.Length > 0) {
                return false;
            }
            return true;
        }

        private Vector3 GetRandomPosInSpawnBounds() {
            Vector3 pos = new Vector3(Random.Range(_spawningBounds.min.x, _spawningBounds.max.x)
                , 0, Random.Range(_spawningBounds.min.z, _spawningBounds.max.z)) ;
            return RotateVector3OnY(pos, transform.eulerAngles.y) + transform.position;
        }
        
        public Vector3 RotateVector3OnY(Vector3 v, float delta) {
            delta = Mathf.Deg2Rad * -delta;
            return new Vector3(
                v.x * Mathf.Cos(delta) - v.z * Mathf.Sin(delta),0,
                v.x * Mathf.Sin(delta) + v.z * Mathf.Cos(delta)
            );
        }

        
    }
    
}