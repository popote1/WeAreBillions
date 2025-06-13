using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ASIdleBreak : StateMachineBehaviour
{
    [SerializeField] private int _idleBreakCount;
    [SerializeField] private float _minDelay =1;
    [SerializeField] private float _maxDelay =2;

    [SerializeField] private string _idleBreakTag = "IdleIndex";
    [SerializeField]private bool _isIdleBreak;
    [SerializeField]private float _delayToIdleBreak;
    [SerializeField]private float _timer;

    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        IntializeIldeBreakSystem(animator);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!_isIdleBreak) {
            _timer += Time.deltaTime;

            if (_timer > _delayToIdleBreak) {
                PickIdleBreak(animator);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98) {
            IntializeIldeBreakSystem(animator);
        }
        
    }

    private void PickIdleBreak(Animator animator) {
        int idleIndex = Random.Range(0, _idleBreakCount) + 1;
        animator.SetFloat(_idleBreakTag, idleIndex);
        
        _isIdleBreak = true;
    }

    private void IntializeIldeBreakSystem(Animator animator)
    {
        _timer = 0;
        _delayToIdleBreak = Random.Range(_minDelay, _maxDelay);
        animator.SetFloat(_idleBreakTag, 0);
        _isIdleBreak = false;
        Debug.Log("Initialize Idle Break");
    }
}
