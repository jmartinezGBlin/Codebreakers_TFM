using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform GFX;
    public LayerMask obstacleLayer;

    [HideInInspector] public Transform target;
    [HideInInspector] public bool facingRight = true;

    //Variables para control del movimiento y Pathfinding
    private Path path;
    private int currentWaypoint = 0;

    EnemyAIController enemyAI;
    Seeker seeker;
    Rigidbody2D rb;


    private void Start()
    {
        enemyAI = GetComponent<EnemyAIController>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

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
        /*if (!enemyAI.FindPlayer())
            return;*/
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

    public void Move(float speed)
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
            return;

        //Para prevenir fuerzas negativas repentinas por "pasarse" el punto, estod ebe ir antes...
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < enemyAI.stats.nextWaypointDistance)
            currentWaypoint++;

        //... Como consecuencia en ocasiones el currentWaypoint es de un valor superior al tamaño del vectorPath
        if (currentWaypoint < path.vectorPath.Count)
        {
            
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed * Time.deltaTime;

            //Si el enemigo no vuela; aplico la fuerza únicamente en el eje X
            if (!enemyAI.stats.canFly)
                force.y *= 0f;

            rb.AddForce(force);

            CheckDirection(force);
            GravityModifier();
        }
    }

    private void GravityModifier()
    {
        if (enemyAI.stats.canFly)
            return;

        if (rb.velocity.y >= float.Epsilon)
            rb.gravityScale = 2f;
        else
            rb.gravityScale = 1f;
    }

    private void CheckDirection(Vector2 force)
    {
        if (force.x >= 0.1f && facingRight)
        {
            Flip();
        }
        else if (force.x <= -0.1f &! facingRight)
        {
            Flip();
        }
    }

    public void Jump()
    {
        //Nos aseguramos de que solo añade el impulso cuando no tiene velocidad en y
        if (rb.velocity.y == 0)
            rb.AddForce(Vector2.up * enemyAI.stats.jumpHeight, ForceMode2D.Impulse);
    }

    public bool CheckObstacleForward()
    {
        bool jump = false;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, enemyAI.stats.jumpRange);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Obstacle"))
                jump = true;
        }

        return jump;
    }
    
    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight  = !facingRight ;

        transform.Rotate(0f, 180f, 0f);
    }
}
