using System;
using UnityEngine;

[RequireComponent(typeof(TriggerZone))]
public class CameraTargetZoneChanger : MonoBehaviour
{
    [SerializeField] private CameratargetController _cameraTargetController;
    
    public Vector2 MaxPosition = new Vector2(10,10);
    public Vector2 MinPosition = new Vector2(0,0);

    private void Awake()
    {
        TriggerZone trigger = GetComponent<TriggerZone>();
        trigger.OnZoneTrigger+= TriggerOnOnZoneTrigger;
        if (_cameraTargetController==null)Debug.LogWarning(" Camara Target Controller is missing in a Zone Changer", this);
    }

    private void TriggerOnOnZoneTrigger(object sender, EventArgs e) {
        SetNewZone();
    }

    public void SetNewZone() {
        if (_cameraTargetController == null) return;
        _cameraTargetController.MaxPosition = MaxPosition;
        _cameraTargetController.MinPosition = MinPosition;
    }
}
