using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIController : MonoBehaviour
{
    public Transform player;
    public EnemyStats stats;
    public Transform[] patrolWaypoints;
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public LayerMask playerLayer;

    [HideInInspector] public EnemyMovement enemyMovement;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform spawnPoint;

    //Acceso a los estados
    [HideInInspector] public AIInterface currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public PatrolState patrolState;
    [HideInInspector] public SearchState searchState;

    private CircleCollider2D col;
    private int actualHealth;
    private float shootingCooldown;
    private float meleeCooldown;

    private void Awake()
    {
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
        searchState = new SearchState(this);
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        spawnPoint = transform;

        if (patrolWaypoints.Length == 0)
            enemyMovement.target = this.transform;     //ENEMIGO ESTÁTICO
        else
            enemyMovement.target = patrolWaypoints[0];
        
        currentState = patrolState;
        actualHealth = stats.healthPoints;
        shootingCooldown = stats.rangeAttackRate;
        meleeCooldown = stats.meleeSpeedAttack;
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
        if (stats.canMove)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        actualHealth -= damage;
        rb.AddForce(knockback);

        currentState = chaseState;
        enemyMovement.target = player;

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

    public void Shoot()
    {
        if (shootingCooldown >= stats.rangeAttackRate)
        {
            Debug.Log("Enemy Shooting!!!");
            if (stats.shootType == EnemyStats.ShootType.bullet)
            {
                GameObject bullet = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
                bullet.GetComponent<Bullet>().shooter = Bullet.Shooter.enemy;

                shootingCooldown = 0f;
            }
        }
        else
            shootingCooldown += Time.deltaTime;
    }

    public void Attack()
    {
        if (meleeCooldown >= stats.meleeSpeedAttack)
        {
            Debug.Log("Enemy Attack!!!");
            meleeCooldown = 0f;

            Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, stats.meleeRange, playerLayer);
            if (hitPlayer == null)
                return;

            Vector2 knockbackVector = (hitPlayer.transform.position - transform.position).normalized * stats.meleeKnockback;
            
            hitPlayer.GetComponent<PlayerCombat>().TakeDamage(stats.meleeDamage, knockbackVector);
            meleeCooldown = 0f;
        }
        else
            meleeCooldown += Time.deltaTime;
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.rangedRange);
        Gizmos.DrawWireSphere(attackPoint.position, stats.meleeRange);

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
