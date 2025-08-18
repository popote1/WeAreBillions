using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

[SelectionBase]
public class InsignifiantProbPhysicBurst : InsignifiantProbs {
    [SerializeField] private Collider _collider;
    [Space(10),Header("SpawningParameters")]
    [SerializeField] private float _prefabToSpawnCountMin = 2;
    [SerializeField] private float _prefabToSpawnCountMax = 4;
    [SerializeField] private FullPhysicProbs[] _prfSpawnElements;
    [SerializeField] private Vector3 _offsetSpawnPoint;
    [SerializeField] private float _spawnOrientationRadius =0.5f;
    [SerializeField] private float _spawnPower =1;
    [SerializeField] private float _spawnTorqueForce=1;
    [SerializeField] private float _fullPhysicDelay = 1;
    

    protected override void Destroy() {
        _collider.enabled = false;
        SpawnFullPhysicsProbs();
        SetAssetToGround();
        base.Destroy();
    }

    private Vector3 GetSpawnPoint() => transform.position + _offsetSpawnPoint;
    

    [ContextMenu("TestSpawn")]
    protected void SpawnFullPhysicsProbs() {
        for (int i = 0; i < Random.Range(_prefabToSpawnCountMin, _prefabToSpawnCountMax); i++) {
            Vector2 pos = Random.insideUnitSphere;
            Vector3 orientation =  new Vector3(pos.x, 0, pos.y)*_spawnOrientationRadius- new Vector3(0, -_spawnPower, 0);
            FullPhysicProbs probs = Instantiate(_prfSpawnElements[Random.Range(0,_prfSpawnElements.Length)], GetSpawnPoint(), Random.rotation);
            probs.SetStartupDelay(_fullPhysicDelay);
            probs.AddImpulseForce(orientation*_spawnPower);
            probs.AddImpulseTorque(Random.insideUnitCircle*_spawnTorqueForce);
        }
    }

    private void SetAssetToGround() {
        transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 70);
    }

    

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetSpawnPoint()+new Vector3(0, _spawnPower, 0), _spawnOrientationRadius);
        Gizmos.DrawLine(GetSpawnPoint(), GetSpawnPoint()+new Vector3(_spawnOrientationRadius,_spawnPower,0 ));
        Gizmos.DrawLine(GetSpawnPoint(), GetSpawnPoint()+new Vector3(-_spawnOrientationRadius,_spawnPower,0 ));
        Gizmos.DrawLine(GetSpawnPoint(), GetSpawnPoint()+new Vector3(0,_spawnPower,_spawnOrientationRadius ));
        Gizmos.DrawLine(GetSpawnPoint(), GetSpawnPoint()+new Vector3(0,_spawnPower,-_spawnOrientationRadius ));
    }
    [ContextMenu("SetupRefsTool")]
    private void SetupRefsTool() {
        _collider = GetComponent<Collider>();
        _impulse = GetComponent<CinemachineImpulseSource>();
    }
}