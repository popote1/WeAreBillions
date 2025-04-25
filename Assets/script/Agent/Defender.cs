using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Defender: GridAgent
{
    [Header("AlertCall")]
    [SerializeField, Range(0, 100)] protected float _chanceOfCall = 10;
    [SerializeField] protected float _durationOfCall = 4;
    [SerializeField] protected float _alertIncrease = 30;
    [SerializeField] protected float _alertCheckRadius = 3;
    
    [SerializeField] protected AttackStruct _attack;
    

    public bool IsCallingAlert => Stat == GridActorStat.CallingAlert;
    public float AlertCallProgress => _alertCallingTimer / _durationOfCall;
    public float AlertCheckRadius => _alertCheckRadius;

    protected bool _hadAlertCalling;
    protected float _alertCallingTimer;

    public event EventHandler<bool> OnChangeAlertCallingStat;

    protected bool CheckForAlertCalling() {
        if (_hadAlertCalling) return false;
        if (Random.Range(0, 100) > _chanceOfCall) return false;
        Collider[] cols =Physics.OverlapSphere(transform.position, _alertCheckRadius);
        foreach (var col in cols) {
            if (col.GetComponent<Defender>() && col.GetComponent<Defender>().IsCallingAlert) return false;
        }
        return true;
    }

    protected void StartAlertCalling() {
        _alertCallingTimer = 0;
        ChangeStat(GridActorStat.CallingAlert);
        OnChangeAlertCallingStat.Invoke(this, true);
    }
    protected override void ManageAlertCalling() {
        _alertCallingTimer += Time.deltaTime;
        if (_alertCallingTimer >= _durationOfCall) {
            AlertSystemManager.Instance?.IncreaseAlertScore(_alertIncrease);
            EndAlertCalling();
            ChangeStat(GridActorStat.Idle);
            _hadAlertCalling = true;
        }
    }

    protected void EndAlertCalling() {
        OnChangeAlertCallingStat.Invoke(this, false);
        _alertCallingTimer = 0;
    } 

    public override void SetAsGrabbed(bool grabed = true) {
        EndAlertCalling();
        base.SetAsGrabbed(grabed);
    }
}