using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_BackRun : StateMachineBehaviour
{
    Transform wallTarget;
    Rigidbody2D rb;
    BossBehaviour boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<BossBehaviour>();
        wallTarget = boss.nearestWall();
        boss.backing = false;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 target = new Vector2(wallTarget.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, boss.stats.moveSpeed * Time.fixedDeltaTime);

        if (boss.backing)
        {
            rb.MovePosition(newPos);


            //Check distance to target Point
            if (Vector2.Distance(wallTarget.position, rb.position) <= boss.stats.nextWaypointDistance)
            {
                animator.SetTrigger("FinishAttack");
            }
        }
        else
            boss.LookAtPlayer();
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.ResetIdle();
        animator.ResetTrigger("FinishAttack");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
