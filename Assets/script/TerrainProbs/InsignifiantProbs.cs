using System;
using script;
using Unity.Cinemachine;
using UnityEngine;

public class InsignifiantProbs : MonoBehaviour
{
    [SerializeField]protected GameObject prefabsFX;
    [SerializeField]protected CinemachineImpulseSource _impulse;
    protected virtual void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Zombi")) {
            Destroy();
        }
        if (other.gameObject.CompareTag("Defender")) {
            Destroy();
        }
    }
    
    [ContextMenu("DestroyTest")]
    protected virtual void Destroy() {
        if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
        if (_impulse) _impulse.GenerateImpulse();
        StaticData.DestroyElement();
    }
    protected void DisableColliders(Collider[]cols) {
        foreach (var col in cols) {
            if (col == null) continue;
            col.enabled = false;
        }
    } 
}