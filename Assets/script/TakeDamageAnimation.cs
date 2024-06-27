using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageAnimation : StateMachineBehaviour
{

    [SerializeField] private float _recoilTime = 0.3f;
    [SerializeField] private AnimationCurve _animationCurve;
    private float _timer=0;
    
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (animator.GetBool("TakeDamage")) {
            _timer = 0;
            animator.SetBool("TakeDamage", false);
        }
        if (_timer < _recoilTime) {
            _timer += Time.deltaTime;
            if (_timer > _recoilTime) _timer = _recoilTime;
                animator.SetLayerWeight(1,_animationCurve.Evaluate(_timer/_recoilTime));
        }
        
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
