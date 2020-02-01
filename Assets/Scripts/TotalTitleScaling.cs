using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalTitleScaling : StateMachineBehaviour {

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<TitleBackgroundControl>().countScale++ > 3)
            animator.GetComponent<Animator>().enabled = false; 
    }
}
