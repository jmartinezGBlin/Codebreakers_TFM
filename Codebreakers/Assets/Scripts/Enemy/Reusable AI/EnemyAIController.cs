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

    public AudioSource shootAudio;
    public AudioSource meleeAudio;
    public AudioSource hitAudio;

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
        //Inicializamos variables
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();

        //Inicializamos las estadísticas según el ScriptableObject EnemyStats
        actualHealth = stats.healthPoints;
        shootingCooldown = stats.rangeAttackRate;
        meleeCooldown = stats.meleeSpeedAttack;

        spawnPoint = transform;

        if (patrolWaypoints.Length == 0)
            enemyMovement.target = this.transform;     //ENEMIGO ESTÁTICO
        else
            enemyMovement.target = patrolWaypoints[0];  //ENEMIGO QUE PATRULLA
        
        //Seleccionamos el primer estado como el estado PATRULLA (Estado base de nuestro enemigo)
        currentState = patrolState;
    }
    

    private void FixedUpdate()
    {
        //Si el enemigo está vivo, ejecutamos la función Update del estado en el que se encuentre.
        if (dead)
            return;

        currentState.UpdateState();
    }

    private void LateUpdate()
    {
        //Por diversas pruebas: La animación se ejecuta mejor al realizarse en el LateUpdate.
        if (dead)
            return;

        if (anim != null && rb != null)
            anim.SetFloat("moveSpeed", Mathf.Abs(rb.velocity.x));
    }

    private void Die()
    {
        //Paramos cualquier subrutina en marcha
        StopAllCoroutines();

        //anulamos el movimiento a través del Rigidbody y el collider.
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        col.enabled = false;

        //Inicializamos un tiempo para que desaparezca el cuerpo
        StartCoroutine(DyingTime());
    }

    IEnumerator DyingTime()
    {
        //Esto indicará al resto de funciones que no se ejecuten
        dead = true;

        anim.SetTrigger("die");

        yield return new WaitForSeconds(2f);    //Tras 2 segundos, destruimos el objeto.
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (dead)
            return;

        if (hitAudio != null)
            hitAudio.Play();

        //Los enemigos estáticos no pueden ser desplazados con nuestros ataques.
        if (stats.canMove)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        else
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //Si al recibir el daño, el enemigo cae a 0 o menos, ejecutamos la función de morir (Die());
        // en caso contrario aplicamos el knockback y la animación pertinente; y seleccionamos que pase al estado Chase (Persecución)
        actualHealth -= damage;

        if (actualHealth <= 0)
            Die();
        else
        {
            anim.SetTrigger("hit");
            rb.AddForce(knockback);

            enemyMovement.target = player;
            currentState = chaseState;
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

        //Comprobamos si el jugador se encuentra dentro del ángulo de visión y dentro de la distancia de visión del Enemigo.
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
            if (shootAudio != null)
                shootAudio.Play();

            //Dependiendo del tipo de disparo configurado; deberá instanciar el proyectil de una manera u otra.
            if (stats.shootType == EnemyStats.ShootType.bullet)
            {
                GameObject bullet = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
                Bullet bulletController = bullet.GetComponent<Bullet>(); 
                    
                bulletController.shooter = Bullet.Shooter.enemy;
                bulletController.bulletSpeed = stats.bulletSpeed;
                bulletController.shootKnockback = stats.rangeKnockback;
                bulletController.shootDamage = stats.rangeDamage;

                shootingCooldown = 0f;
            }
            else if (stats.shootType == EnemyStats.ShootType.launcher)
            {
                GameObject instantiatedProjectile = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
                Rocket rocketController = instantiatedProjectile.GetComponent<Rocket>();
                
                rocketController.shooter = Rocket.Shooter.enemy;
                rocketController.rocketSpeed = stats.bulletSpeed;
                rocketController.shootKnockback = stats.rangeKnockback;
                rocketController.shootDamage = stats.rangeDamage;

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
            StartCoroutine("Attacking");
        else
            meleeCooldown += Time.deltaTime;
    }

    IEnumerator Attacking()
    {
        //El ataque se ejecuta junto a la animación; ha habido que timear correctamente el tiempo. (DEBERÍA CAMBIARSE Y HACERSE POR EVENTO EN EL ANIMATOR)
        attacking = true;
        anim.SetTrigger("attack");

        if (stats.rangedAttack)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.17f);

        if (meleeAudio != null)
        {
            meleeAudio.Play();
        }

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
