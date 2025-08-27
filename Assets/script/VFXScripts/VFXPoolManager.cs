using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VFXPoolManager : MonoBehaviour
{
    
    public static VFXPoolManager Instance;
    
    public enum VFXPooledType {
        SmallExplosion , BigExplosion, MoveOrder, bloodSplater, buildingColaps, SmokeStripe, Death, zombieAttack, EmoteSurprise, EmoteAlert, EmoteExost
    }
    
    [SerializeField] private VFXPoolInitializeData[] _vfxPoolInitializeData;
    private Dictionary<VFXPooledType, VFXPoolHolder> _vfxPools;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            Debug.Log("VFXPoolManager Set is"+ Instance);
        }
        else {
            Debug.LogWarning("VFXPoolManager Already in the scene");
        }
        SetUpPools();
    }

    private void SetUpPools() {
        _vfxPools = new Dictionary<VFXPooledType, VFXPoolHolder>();

        foreach (var data in _vfxPoolInitializeData) {
            if (data.PrfVfx== null) continue;
            _vfxPools.Add(data.Type, new VFXPoolHolder(data));
        }
        Debug.Log(_vfxPools.Count + " VFX Pool are created");
    }

    public VfxPoolableMono GetPooledVFXOfType(VFXPooledType type) {
        if (!_vfxPools.ContainsKey(type)) return null;
        return _vfxPools[type].VFXPool.Get();
    }

   
    [Serializable]
    public struct VFXPoolInitializeData {
        public VFXPooledType Type;
        public VfxPoolableMono PrfVfx;
        public bool CollectionCheck ;
        public int DefaultCapacity ;
        public int MaxCapacity;
        

        public VFXPoolInitializeData(VFXPooledType type, VfxPoolableMono prf) {
            Type = type;
            PrfVfx = prf;
            CollectionCheck = true;
            DefaultCapacity = 20;
            MaxCapacity=100;
        }
    }

    private class VFXPoolHolder {
        
        [SerializeField] private VfxPoolableMono _prfPooledVFX;
        private IObjectPool<VfxPoolableMono> _vfxPool;
        public IObjectPool<VfxPoolableMono> VFXPool { get => _vfxPool; }

        public VFXPoolHolder(VFXPoolInitializeData data) {
            _prfPooledVFX = data.PrfVfx;
            _vfxPool = new ObjectPool<VfxPoolableMono>(CreateVFX, OnGetFromPool, OnReleaseToPool, OnDestroyPoolObject,
                data.CollectionCheck, data.DefaultCapacity, data.MaxCapacity);
        }
        private VfxPoolableMono CreateVFX() {
            VfxPoolableMono vfx = Instantiate(_prfPooledVFX);
            vfx.PooledObject = _vfxPool;
            return vfx;
        }

        private void OnGetFromPool(VfxPoolableMono vfxTest) => vfxTest.gameObject.SetActive(true);
        private void OnReleaseToPool(VfxPoolableMono vfxPool) => vfxPool.gameObject.SetActive(false);
        private void OnDestroyPoolObject(VfxPoolableMono vfxTestPool)=> Destroy(vfxTestPool.gameObject);
        
    }
    
    //-----------------------------------Test Part---------------------------------------------//

   /* [SerializeField]private Camera _camera;

    private void Start() {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out hit)) {
                VfxPoolableMono vfx = GetPooledVFXOfType(VFXPooledType.SmallExplosion);

                if (vfx == null) return;

                vfx.transform.position = hit.point;
            }
        }
        if (Input.GetButtonDown("Fire2")) {
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition),out hit)) {
                VfxPoolableMono vfx = GetPooledVFXOfType(VFXPooledType.BigExplosion);

                if (vfx == null) return;

                vfx.transform.position = hit.point;
            }
        }
    }*/
}