using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBow : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Bow", animator.GetComponent<ToonControl>().PlayMotion[1] = false);
    }
}