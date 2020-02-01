using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToonControl : MonoBehaviour
{
    public GameObject Character;
    public bool[] PlayMotion;
    public Animator animator;

    /* public GameObject[] teachFormObjects;
       public bool teachForm = false;
    */

    bool[] PlayedMotion;
    int _Talk = Animator.StringToHash("Talk");
    int _Pose = Animator.StringToHash("Pose");
    int _Bow = Animator.StringToHash("Bow");

    void Update()
    {
        /*
          for (int i = 0, j = teachFormObjects.Length; i < j; i++)
            teachFormObjects[i].SetActive(!teachForm);
         */

        if (PlayMotion[0] && !animator.GetBool(_Talk))
            animator.SetBool(_Talk, true);
        if (!PlayMotion[0] && animator.GetBool(_Talk))
            animator.SetBool(_Talk, false);

        if (PlayMotion[1] && !animator.GetBool(_Bow))
            animator.SetBool(_Bow, true);

        if (PlayMotion[2] && !animator.GetBool(_Pose))
            animator.SetBool(_Pose, true);
        if (!PlayMotion[2] && animator.GetBool(_Pose))
            animator.SetBool(_Pose, false);
    }
}
