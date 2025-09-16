using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class DetructibleTrigger : Destructible {
    [SerializeField] private UnityEvent OnDestruction;
    public override void DestroyDestructible(GridAgent source = null) {
        OnDestruction?.Invoke();
        base.DestroyDestructible(source);
    }
}