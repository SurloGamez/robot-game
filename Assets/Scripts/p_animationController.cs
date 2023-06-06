using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_animationController : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateAnimation(bool run, bool grounded, bool jumptrig, bool fall, bool roll, bool attacktrig, float xdir, float ydir, bool attackend, bool walljump, bool wallSlide, bool attacking)
    {
        anim.SetBool("roll", roll);
        anim.SetBool("run", run);
        anim.SetBool("grounded", grounded);
        anim.SetBool("jump", jumptrig);
        anim.SetBool("fall", fall);

        anim.SetBool("attackbool", attacktrig);
        
        anim.SetFloat("xdir", xdir);
        anim.SetFloat("ydir", ydir);

        anim.SetBool("attackend", attackend);

        anim.SetBool("wallJump", walljump);
        anim.SetBool("wallSlide", wallSlide);

        anim.SetBool("attacking", attacking);
    }

    public void Reset()
    {
        anim.Play("p_idle");
    }
}
