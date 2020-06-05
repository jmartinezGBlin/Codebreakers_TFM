using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    BossBehaviour boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<BossBehaviour>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();
        Vector2 target;

        if (boss.towardsPlayer)
            target = new Vector2(player.position.x, rb.position.y);
        else
            target = new Vector2(boss.centerPoint.position.x, rb.position.y);

        Vector2 newPos = Vector2.MoveTowards(rb.position, target, boss.stats.moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);

        if (boss.towardsPlayer)
        {
            //Check distance to Player
            if (Vector2.Distance(player.position, rb.position) <= boss.stats.nextWaypointDistance)
            {
                animator.SetTrigger("ToAttack");
            }
        }
        else
        {
            //Check distance to Center
            if (Vector2.Distance(boss.centerPoint.position, rb.position) <= 0.5f)
            {
                animator.SetTrigger("ToHeal");
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("ToAttack");
        animator.ResetTrigger("ToHeal");
    }

   
}
