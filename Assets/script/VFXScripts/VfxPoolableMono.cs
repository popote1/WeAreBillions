using UnityEngine;
using UnityEngine.Pool;

public class VfxPoolableMono : MonoBehaviour {
    
    
    protected IObjectPool<VfxPoolableMono> _pooledObject;
    
    public IObjectPool<VfxPoolableMono> PooledObject { set => _pooledObject = value; }

    protected void OnDisable() {
        if (_pooledObject == null) Destroy(gameObject);
        else
        {
            _pooledObject.Release(this);
            Debug.Log("Object Release", this);
        }
    }
}