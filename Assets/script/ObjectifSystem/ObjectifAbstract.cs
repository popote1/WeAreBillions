using System;
using script;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ObjectifAbstract
{
    
    public enum ObjectifType {
        ZoneToReach, HordeSize
    }

    [SerializeField] private ObjectifType _objectifType;
    [SerializeField, TextArea] private string _description;
    [SerializeField] private int _objectifId;
    
    [SerializeField]private TriggerZone _triggerZone;
    [SerializeField] private int _hordeSize;
    
    [SerializeField] private bool _doWin;
    private bool _editorShowMore;
    
    public string Description { get => _description; }

    public ObjectifType Type
    {
        get => _objectifType;
    }
    public  bool DoWin { get => _doWin; }

    public int ObjectifId { get => _objectifId; }
    private bool _objectifIsComplete;
    public virtual void OnStartObjectif() {
        if( _objectifType==ObjectifType.ZoneToReach)_triggerZone.OnZoneTrigger+= TriggerZoneOnOnZoneTrigger;
        StaticEvents.DoObjectifStart(_objectifId);
    }

    public virtual void OnEndObjectif(){}

    public virtual bool CheckIfObjectifIsComplet()
    {
        switch (_objectifType)
        {
            case ObjectifType.ZoneToReach: return _objectifIsComplete;
            case ObjectifType.HordeSize:return StaticData.zombieCount >= _hordeSize;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return false;
    }
    private void TriggerZoneOnOnZoneTrigger(object sender, EventArgs e) {
        _objectifIsComplete = true;
    }
    
}