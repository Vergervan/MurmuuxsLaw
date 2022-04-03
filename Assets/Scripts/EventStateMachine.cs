using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class EventStateMachine : StateMachineBehaviour
{
    [SerializeField] private float delayBefore = 0f;
    [SerializeField] private List<string> triggersOnStart;
    [SerializeField] private float delayAfter = 0f;
    [SerializeField] private List<string> triggersOnEnd;
    private ActionTrigger actionTrigger;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actionTrigger = animator.gameObject.GetComponent<ActionTrigger>();
        CallActions(actionTrigger, triggersOnStart, delayBefore);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actionTrigger = animator.gameObject.GetComponent<ActionTrigger>();
        CallActions(actionTrigger, triggersOnEnd, delayAfter);
    }
    private async void CallActions(ActionTrigger actionTrigger, IEnumerable<string> triggers, float delayTime = 0f)
    {
        if(delayTime > 0)
            await Task.Delay((int)(delayTime*1000));
        if (triggersOnEnd != null && triggers.Count() > 0)
        {
            foreach (var trigger in triggers)
            {
                actionTrigger.TryToInvokeEvent(trigger);
            }
        }
    }
}
