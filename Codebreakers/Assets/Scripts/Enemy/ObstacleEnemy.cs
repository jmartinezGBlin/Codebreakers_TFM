using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleEnemy : MonoBehaviour
{

    [SerializeField] private Transform cielingDetection;
    [SerializeField] private Transform groundDetection;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private int damage;
    [SerializeField] private int knockbak;
    [SerializeField] private float speed;
    [SerializeField] private bool directionUp;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded


    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if (directionUp)
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        else
            transform.Translate(Vector3.down * speed * Time.deltaTime);

    }

    void CheckGround()
    {
        Transform detector;

        if (directionUp)
            detector = cielingDetection;
        else
            detector = groundDetection;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(detector.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                directionUp = !directionUp;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(damage, new Vector2(knockbak, 0f));
        }
    }
}
