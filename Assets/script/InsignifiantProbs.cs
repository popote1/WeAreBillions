using System.Collections;
using System.Collections.Generic;
using script;
using Unity.Cinemachine;
using UnityEngine;

public class InsignifiantProbs : MonoBehaviour
{
    
    [SerializeField]private GameObject prefabsDebrie;
    [SerializeField]private GameObject prefabsFX;
    [SerializeField]private CinemachineImpulseSource _impulse;
   
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Zombi")) {
            Destroy();
        }
        if (other.gameObject.CompareTag("Defender")) {
            Destroy();
        }
    }
    
    public void Destroy() {
        if (prefabsDebrie) Instantiate(prefabsDebrie, transform.position, transform.rotation);
        if (prefabsFX) Instantiate(prefabsFX, transform.position, transform.rotation);
        if (_impulse) _impulse.GenerateImpulse();
        StaticData.DestroyElement();
        GetComponentInChildren<Renderer>().enabled=false;
        GetComponentInChildren<Collider>().enabled=false;
        Destroy(gameObject, 1);
    }
}
