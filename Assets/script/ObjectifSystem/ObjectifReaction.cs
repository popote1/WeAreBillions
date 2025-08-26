using System;
using script;
using UnityEngine;

public class ObjectifReaction : MonoBehaviour
{
    [SerializeField] protected ReactionStat _reactionStat;
    [SerializeField] protected int _objectifId;

    protected enum ReactionStat {
        OnStart, OnChange, OnEnd       
    }
    protected virtual void Awake() {
        StaticEvents.OnObjectifStart += StaticEventsOnOnObjectifStart;
        StaticEvents.OnCurrentObjectifChange += StaticEventsOnOnCurrentObjectifChange;
        StaticEvents.OnObjectifEnded += StaticEventsOnOnObjectifEnded;
    }
    

    protected void OnDestroy() {
        StaticEvents.OnObjectifStart -= StaticEventsOnOnObjectifStart;
        StaticEvents.OnCurrentObjectifChange -= StaticEventsOnOnCurrentObjectifChange;
        StaticEvents.OnObjectifEnded -= StaticEventsOnOnObjectifEnded;
    }

    private void StaticEventsOnOnObjectifEnded(object sender, int e) {
        if(_reactionStat == ReactionStat.OnEnd && e == _objectifId) DoReaction(e);
    }
    private void StaticEventsOnOnCurrentObjectifChange(object sender, ObjectifAbstract e) {
        if(_reactionStat == ReactionStat.OnChange && e.ObjectifId == _objectifId) DoReaction(e.ObjectifId);
    }

    private void StaticEventsOnOnObjectifStart(object sender, int e) {
        if(_reactionStat == ReactionStat.OnStart && e == _objectifId) DoReaction(e);
    }

    protected virtual void DoReaction(int id)
    {
        
    }
    
}