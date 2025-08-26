using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObjectifReachZone: ObjectifAbstract {
    [SerializeField]private TriggerZone _triggerZone;
    [SerializeField]private UnityEvent OnObjectifStart;
    [SerializeField]private UnityEvent OnObjectifComplet;
     
     
     
    private bool _zoneReach;
     
    public override void OnStartObjectif() {
        _triggerZone.OnZoneTrigger+= TriggerZoneOnOnZoneTrigger;
        OnObjectifStart?.Invoke();
    }
    private void TriggerZoneOnOnZoneTrigger(object sender, EventArgs e) {
        _zoneReach = true;
    }

    public override void OnEndObjectif() {
        OnObjectifComplet?.Invoke();
    }

    public override bool CheckIfObjectifIsComplet()
    {
        return _zoneReach;
    }
}