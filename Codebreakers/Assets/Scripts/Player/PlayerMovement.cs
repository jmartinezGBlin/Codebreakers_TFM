using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    private PlayerCombat playerCombat;

    
    private float horizontalMove = 0f;
    private bool jump = false;
    [HideInInspector] public bool crouching = false;

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()
    {
        if (controller.dead || controller.stopInput)
            return;

        horizontalMove = Input.GetAxisRaw("Horizontal") * controller.stats.moveSpeed;

        if (Input.GetButtonDown("Jump"))
            jump = true;

        crouching = Input.GetAxisRaw("Vertical") < -0.3f;
    }

    private void FixedUpdate()
    {
        if (controller.dead || controller.stopInput)
            return;

        //Character movement thorugh CharacterController2D
        if (!playerCombat.attacking)
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouching, jump);

        if (jump)
            jump = false;
    }
}
