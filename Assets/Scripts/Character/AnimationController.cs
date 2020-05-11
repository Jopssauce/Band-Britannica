using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    public void MoveToAttackTrigger()
    {
        animator.SetBool ("MoveToAttack", true);
    }
    public void ReturnToOriginTrigger()
    {
        animator.SetBool ("ReturnToOrigin", true);
    }
    public void AttackingTrigger()
    {
        animator.SetBool ("Attacking", true);
    }
    public void NotAttackingTrigger()
    {
        animator.SetBool ("Attacking", false);
    }
}
