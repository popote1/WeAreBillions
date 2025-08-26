using UnityEngine;

[RequireComponent(typeof(CameratargetController))]
public class ObjectifReactionCameraZoneChanger : ObjectifReaction {
    [SerializeField] private Vector2 _maxPos;
    [SerializeField] private Vector2 _minPos;
    
    private CameratargetController _cameratargetController;

    protected override void Awake() {
        _cameratargetController = GetComponent<CameratargetController>();
        base.Awake();
    }

    protected override void DoReaction(int id)
    {
        _cameratargetController.MaxPosition = _maxPos;
        _cameratargetController.MinPosition = _minPos;
        base.DoReaction(id);
    }
}