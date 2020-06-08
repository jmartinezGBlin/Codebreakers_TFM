using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public bool reverseSprite; //CORREGIR LOS SPRITES; Esto es solo para salir del paso

    [HideInInspector] public Transform target;
    [HideInInspector] public bool facingRight = true;

    //Variables para control del movimiento y Pathfinding
    private Path path;
    private int currentWaypoint = 0;

    private EnemyAIController enemyAI;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator anim;


    private void Start()
    {
        enemyAI = GetComponent<EnemyAIController>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (enemyAI.stats.canFly)
            rb.gravityScale = 0f;
        else
            rb.gravityScale = 1f;

        if (enemyAI.stats.canMove)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        InvokeRepeating("UpdateMovement", 0f, .5f);
    }

    void UpdateMovement()
    {
        if (target == null)
            return;


        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }


    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void Move(float speed, float maxSpeed)
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
            return;

        //Para prevenir fuerzas negativas repentinas por "pasarse" el punto, estod ebe ir antes...
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < enemyAI.stats.nextWaypointDistance)
            currentWaypoint++;

        float targetDistance = Vector2.Distance(rb.position, target.position);

        if (targetDistance < enemyAI.stats.nextWaypointDistance || enemyAI.attacking)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //... Como consecuencia en ocasiones el currentWaypoint es de un valor superior al tamaño del vectorPath
        if (currentWaypoint < path.vectorPath.Count)
        {
            
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            //Si el enemigo no vuela; aplico la fuerza únicamente en el eje X
            if (!enemyAI.stats.canFly)
            {
                force.y *= 0f;
                if (rb.velocity.magnitude > maxSpeed && rb.velocity.y == 0)
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                else
                    rb.AddForce(force);

            }
            else
            {
                if (rb.velocity.magnitude > maxSpeed)
                    rb.velocity = rb.velocity.normalized * maxSpeed;
                else
                    rb.AddForce(force);
            }
            
            CheckDirection(force);
            GravityModifier();
        }
    }

    private void GravityModifier()
    {
        if (enemyAI.stats.canFly)
            return;

        if (Mathf.Abs(rb.velocity.y) > 0.1f)
            rb.gravityScale = 2f;
        else
            rb.gravityScale = 1f;
    }

    private void CheckDirection(Vector2 force)
    {
        if (!reverseSprite)
        {
            if (force.x >= 0.1f && facingRight)
            {
                Flip();
            }
            else if (force.x <= -0.1f & !facingRight)
            {
                Flip();
            }
        }
        else
        {
            if (force.x >= 0.1f && !facingRight)
            {
                Flip();
            }
            else if (force.x <= -0.1f & facingRight)
            {
                Flip();
            }
        }
    }

    public void Jump()
    {
        //Nos aseguramos de que solo añade el impulso cuando no tiene velocidad en y
        if (Mathf.Abs(rb.velocity.y) <= 0.1f)
            rb.AddForce(Vector2.up * enemyAI.stats.jumpHeight, ForceMode2D.Impulse);

    }
    
    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight  = !facingRight ;

        transform.Rotate(0f, 180f, 0f);
    }
}
