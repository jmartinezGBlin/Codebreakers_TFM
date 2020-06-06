using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Heal : StateMachineBehaviour
{
    Rigidbody2D rb;
    BossBehaviour boss;
    GameObject protectionOrb;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<BossBehaviour>();

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.simulated = false;

        boss.inHeal = true;
        boss.SpawnAntennas();
        protectionOrb = Instantiate(boss.protectionOrbPrefab, boss.transform.position + new Vector3(0,1.5f,0) , boss.transform.rotation);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.InstantiateOrbAttack();
        if (GameObject.FindGameObjectsWithTag("Protection").Length == 0)
        {
            animator.SetTrigger("FinishHeal");
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.simulated = true;

        Destroy(protectionOrb);

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Spawnable"))
        {
            Destroy(item);
        }

        boss.ResetIdle();
        animator.ResetTrigger("FinishHeal");
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
