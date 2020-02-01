using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeAfterAnimation : StateMachineBehaviour {

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Animator>().enabled = false;
    }

}
