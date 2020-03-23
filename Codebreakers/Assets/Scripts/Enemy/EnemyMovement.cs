using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyAIController enemyAI;
    [HideInInspector] public Transform target;

    //Variables para control del movimiento y Pathfinding
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;


    private void Start()
    {
        enemyAI = GetComponent<EnemyAIController>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
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

    public void Move()
    {
        if (path == null)
        {
            Debug.Log("null path");
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("end of Path");
            reachedEndOfPath = true;
            return;
        }
        else
            reachedEndOfPath = false;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Debug.Log(direction);
        Vector2 force = direction * enemyAI.stats.moveSpeed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < enemyAI.stats.nextWaypointDistance)
            currentWaypoint++;
    }
}
