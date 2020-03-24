using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIController : MonoBehaviour
{
    public Transform player;
    public EnemyStats stats;
    public Transform[] patrolWaypoints;

    [HideInInspector] public EnemyMovement enemyMovement;

    //Acceso a los estados
    [HideInInspector] public AIInterface currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public PatrolState patrolState;

    private Rigidbody2D rb;
    private Vector3 lookDirection;
    private CircleCollider2D collider;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();

        if (patrolWaypoints.Length == 0)
            enemyMovement.target = this.transform;     //ENEMIGO ESTÁTICO
        else
            enemyMovement.target = patrolWaypoints[0];
        
        currentState = patrolState;
    }
    

    private void FixedUpdate()
    {
        currentState.UpdateState();
    }

    public bool FindPlayer()
    {
        if (enemyMovement == null)
            return false;

        if (enemyMovement.facingRight)
                lookDirection = transform.right;
        else
                lookDirection = -transform.right;

        /*
        if (PlayerMinDistance())
            return true;*/
        

        //Comprobamos si hay un muro o un obstáculo entre el enemigo y el jugador.
        if (CheckObstacleInBetween())
            return false;


        Vector2 playerDirection = (Vector2)(player.transform.position - transform.position);
        Vector2 direction = playerDirection.normalized;

        float angle = Vector2.Angle(direction, lookDirection);

        if (angle <= stats.maxAngle)
        {
            if (Mathf.Abs(playerDirection.x) <= stats.lookRange)
                return true;
        }
        return false;
    }

    private bool CheckObstacleInBetween()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Obstacle"))
                return true;
        }
        return false;
    }

    /*private bool PlayerMinDistance()
    {
        Collider2D[] colliders = new Collider2D[10];
            
        Physics2D.OverlapCircleNonAlloc(transform.position, collider.radius * 0.01f, colliders);

        foreach (Collider2D coll in colliders)
        {
            if (coll.gameObject.CompareTag("Player"))
                return true;
        }

        return false;
    }*/


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.lookRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, stats.jumpRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(stats.maxAngle, transform.forward) * lookDirection * stats.lookRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-stats.maxAngle, transform.forward) * lookDirection * stats.lookRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, lookDirection * stats.lookRange);

        if (collider != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, collider.radius * 0.1f);
        }

        if (FindPlayer())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * stats.lookRange);

    }
}
