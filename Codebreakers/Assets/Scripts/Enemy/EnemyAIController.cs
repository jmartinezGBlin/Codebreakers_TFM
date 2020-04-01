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
    [HideInInspector] public SearchState searchState;
    [HideInInspector] public ShootState shootState;
    [HideInInspector] public MeleeState meleeState;

    private Rigidbody2D rb;
    private CircleCollider2D col;
    private int actualHealth;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
        searchState = new SearchState(this);
        shootState = new ShootState(this);
        meleeState = new MeleeState(this);
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();

        if (patrolWaypoints.Length == 0)
            enemyMovement.target = this.transform;     //ENEMIGO ESTÁTICO
        else
            enemyMovement.target = patrolWaypoints[0];
        
        currentState = patrolState;
        actualHealth = stats.healthPoints;
    }
    

    private void FixedUpdate()
    {
        currentState.UpdateState();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        actualHealth -= damage;
        rb.AddForce(knockback);

        if (actualHealth <= 0)
            Die();
    }
    

    public bool FindPlayer()
    {
        if (enemyMovement == null)
            return false;

       /* if (enemyMovement.facingRight)
                lookDirection = transform.right;
        else
                lookDirection = -transform.right;*/

        /*  if (PlayerMinDistance())
            return true;*/

        //Comprobamos si hay un muro o un obstáculo entre el enemigo y el jugador.
        if (CheckObstacleInBetween())
            return false;

        Vector2 playerDirection = (Vector2)(player.transform.position - transform.position);
        Vector2 direction = playerDirection.normalized;

        float angle = Vector2.Angle(direction, transform.right);

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
            
        Physics2D.OverlapCircleNonAlloc(transform.position, col.radius * 0.01f, colliders);

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

        Vector3 fovLine1 = Quaternion.AngleAxis(stats.maxAngle, transform.forward) * transform.right * stats.lookRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-stats.maxAngle, transform.forward) * transform.right * stats.lookRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.right * stats.lookRange);

        if (col != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, col.radius * 0.1f);
        }

        if (FindPlayer())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * stats.lookRange);

    }
}
