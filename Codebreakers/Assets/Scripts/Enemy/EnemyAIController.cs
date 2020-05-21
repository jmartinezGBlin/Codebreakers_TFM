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

    private CapsuleCollider2D col;
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
        col = GetComponent<CapsuleCollider2D>();
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


        Physics2D.IgnoreLayerCollision(10, 10, false);
        Physics2D.IgnoreLayerCollision(10, 9, false);
        Physics2D.IgnoreLayerCollision(10, 11, false);
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
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        col.enabled = false;
        StartCoroutine(DyingTime());
    }

    IEnumerator DyingTime()
    {
        dead = true;
        anim.SetTrigger("die");

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (dead)
            return;

        if (stats.canMove)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        actualHealth -= damage;
        if (actualHealth <= 0)
            Die();
        else
        {
            anim.SetTrigger("hit");
            rb.AddForce(knockback);

            currentState = chaseState;
            enemyMovement.target = player;
        }

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
        RaycastHit2D hit = Physics2D.Raycast(sightPoint.transform.position, (player.transform.position - sightPoint.transform.position + Vector3.up * 1.5f).normalized * stats.jumpRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Obstacle"))
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
            else if (stats.shootType == EnemyStats.ShootType.launcher)
            {
                Debug.Log("Shooting");
                GameObject instantiatedProjectile = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
                instantiatedProjectile.GetComponent<Rocket>().shooter = Rocket.Shooter.enemy;

                instantiatedProjectile.GetComponent<Rigidbody2D>().velocity = instantiatedProjectile.transform.right * stats.bulletSpeed / 2;
                // Ignore collisions between the missile and the character controller
                Physics2D.IgnoreCollision(instantiatedProjectile.GetComponent<BoxCollider2D>(), transform.GetComponent<CapsuleCollider2D>());

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

        if (stats.rangedAttack)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.17f);

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, stats.meleeRange, playerLayer);

        if (hitPlayer != null)
        {
            Vector2 knockbackVector = (hitPlayer.transform.position - transform.position).normalized * stats.meleeKnockback;

            hitPlayer.GetComponent<PlayerCombat>().TakeDamage(stats.meleeDamage, knockbackVector);
        }

        if (stats.rangedAttack)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.25f);

        attacking = false;
        meleeCooldown = 0f;
    }

    public bool CheckObstacleForward()
    {
        bool jump = false;

        RaycastHit2D hit = Physics2D.Raycast(sightPoint.transform.position, transform.right, stats.jumpRange);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Obstacle"))
                jump = true;
        }

        return jump;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(10, new Vector2(stats.meleeKnockback,0f));
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(sightPoint.transform.position, stats.lookRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(sightPoint.transform.position, transform.right * stats.jumpRange);

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

        
        if (FindPlayer())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawRay(sightPoint.transform.position, (player.transform.position - sightPoint.transform.position + Vector3.up * 1.5f).normalized * stats.lookRange);
        
    }
}
