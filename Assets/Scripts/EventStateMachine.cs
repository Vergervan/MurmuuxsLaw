using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventStateMachine : StateMachineBehaviour
{
    [SerializeField] private string ExitTrigger;
    [SerializeReference] private GameObject obj;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!string.IsNullOrWhiteSpace(ExitTrigger))
            animator.SetTrigger(ExitTrigger);
        Debug.Log("State exit");
    }
}
