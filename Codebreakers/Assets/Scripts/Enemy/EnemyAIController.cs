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
    public Transform sightPoint;
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
    private Animator anim;
    private int actualHealth;
    private float shootingCooldown;
    private float meleeCooldown;
    [HideInInspector] public bool attacking = false;
    private bool dead = false;

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
        anim = GetComponent<Animator>();
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
        if (dead)
            return;

        currentState.UpdateState();
    }

    private void LateUpdate()
    {
        if (dead)
            return;

        if (anim != null && rb != null)
        {
            anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
            //Debug.Log(Mathf.Abs(rb.velocity.x));
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DyingTime());
    }

    IEnumerator DyingTime()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        dead = true;
        anim.SetTrigger("die");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (stats.canMove)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        anim.SetTrigger("hit");
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
        if (player.GetComponent<CharacterController2D>().dead)
            return false;

        //Comprobamos si hay un muro o un obstáculo entre el enemigo y el jugador.
        if (CheckObstacleInBetween())
            return false;

        Vector2 playerDirection = (Vector2)(player.transform.position - sightPoint.transform.position + Vector3.up * 1.5f);
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
        RaycastHit2D hit = Physics2D.Raycast(sightPoint.transform.position, (player.transform.position - sightPoint.transform.position + Vector3.up * 1.5f).normalized);

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
        if (meleeCooldown >= stats.meleeSpeedAttack &! attacking)
        {
            StartCoroutine("Attacking");
        }
        else
            meleeCooldown += Time.deltaTime;
    }

    IEnumerator Attacking()
    {
        attacking = true;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(0.5f);

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, stats.meleeRange, playerLayer);

        if (hitPlayer != null)
        {
            Vector2 knockbackVector = (hitPlayer.transform.position - transform.position).normalized * stats.meleeKnockback;

            hitPlayer.GetComponent<PlayerCombat>().TakeDamage(stats.meleeDamage, knockbackVector);
        }

        yield return new WaitForSeconds(0.5f);
        attacking = false;
        meleeCooldown = 0f;
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
        Gizmos.DrawWireSphere(sightPoint.transform.position, stats.lookRange);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, stats.jumpRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.transform.position, stats.rangedRange);
        Gizmos.DrawWireSphere(attackPoint.position, stats.meleeRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(stats.maxAngle, sightPoint.transform.forward) * sightPoint.transform.right * stats.lookRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-stats.maxAngle, sightPoint.transform.forward) * sightPoint.transform.right * stats.lookRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(sightPoint.transform.position, fovLine1);
        Gizmos.DrawRay(sightPoint.transform.position, fovLine2);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(sightPoint.transform.position, sightPoint.transform.right * stats.lookRange);

        if (col != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, col.radius * 0.1f);
        }

        if (FindPlayer())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        Gizmos.DrawRay(sightPoint.transform.position, (player.transform.position - sightPoint.transform.position + Vector3.up * 1.5f).normalized * stats.lookRange);

    }
}
