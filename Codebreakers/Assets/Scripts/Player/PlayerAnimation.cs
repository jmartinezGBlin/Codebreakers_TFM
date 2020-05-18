using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private CharacterController2D pController;
    private PlayerMovement pMovement;
    private PlayerCombat pCombat;
    private Animator animator;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        pController = GetComponent<CharacterController2D>();
        pMovement = GetComponent<PlayerMovement>();
        pCombat = GetComponent<PlayerCombat>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (pController.stopInput)
            return;

        if (pController.dead)
            animator.SetTrigger("die");
        else
        {
            animator.SetFloat("movementSpeed", Mathf.Abs(rb.velocity.x));

            if (!pCombat.attacking)
            {
                animator.SetBool("jump", !pController.m_Grounded);
                animator.SetBool("crouch", pMovement.crouching);
            }
            else
            {
                animator.SetBool("jump", false);
                animator.SetBool("crouch", false);
            }
        }
    }
}
