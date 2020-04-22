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
    void Update()
    {
        animator.SetFloat("movementSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("jump", !pController.m_Grounded);
        Debug.Log(!pController.m_Grounded);
    }
}
