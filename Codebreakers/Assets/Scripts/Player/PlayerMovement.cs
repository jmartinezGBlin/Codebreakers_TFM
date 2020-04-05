using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    
    private float horizontalMove = 0f;
    private bool jump = false;

    private void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * controller.stats.moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        //Character movement thorugh CharacterController2D
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);

        if (jump)
            jump = false;
    }
}
