using System;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{

    public event EventHandler OnZoneTrigger;
    
    [SerializeField] private bool _disableOnTrigger =true;
    [SerializeField] private bool _wasPlay;
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Zombi")) {
            _wasPlay = true;
            if (_disableOnTrigger) gameObject.SetActive(false);
            OnZoneTrigger?.Invoke(this, EventArgs.Empty);
        }
    }
}