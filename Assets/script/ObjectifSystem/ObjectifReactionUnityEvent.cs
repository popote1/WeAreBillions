using UnityEngine;
using UnityEngine.Events;

public class ObjectifReactionUnityEvent : ObjectifReaction
{
    [SerializeField] private UnityEvent _onReaction;
    protected override void DoReaction(int id) {
        _onReaction.Invoke();
        base.DoReaction(id);
    }
}