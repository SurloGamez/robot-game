using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAnimationController : MonoBehaviour
{
    Animator anim;
    float stuncounter = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateAnimation(bool stunned, float stunDur, bool grounded)
    {
        if (stunned) stuncounter++;
        else stuncounter = 0;
        anim.SetBool("stun trigger", stuncounter == 1);
        anim.SetFloat("stun dur", stunDur);
        anim.SetBool("grounded", grounded);
    }

    public void ForcePlay(string name)
    {
        anim.Play(name);
    }

    public void Reset()
    {

    }
}

