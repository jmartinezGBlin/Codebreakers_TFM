using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehaviour : MonoBehaviour
{
    public EnemyStats stats;
    public GameObject protectionOrbPrefab;
    public GameObject[] antennaProtectionPrefabs;
    public GameObject healAttackPrefab;
    public Transform centerPoint;
    public Image healthBar;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform[] wallPoints;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool isFlipped = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [HideInInspector] public bool inRage = false;
    [HideInInspector] public bool backing = false;
    [HideInInspector] public bool towardsPlayer = false;
    [HideInInspector] public bool inHeal = false;

    private int actualHealth;

    private float attackRate = .33f;
    private float shootRate = .67f;
    private float healRate = .25f;

    private float randomState;
    private bool idle = true;

    // Timer de decisión desde el estado IDLE
    private float decisionTime;
    private float rageDecisionTime;
    private float idleTime = 0f;
    private bool dead = false;

    //Timer de Spawn de ataques en estado HEAL
    private float spawnRate = 2f;
    private float spawnTime;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        actualHealth = stats.healthPoints;
        anim = GetComponent<Animator>();

        decisionTime = Random.Range(0.5f, 1.5f);
        rageDecisionTime = Random.Range(0f, 1f);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        healthBar.fillAmount = (float)actualHealth / (float)stats.healthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (actualHealth <= stats.healthPoints/2)
            inRage = true;
        else
            inRage = false;

        if (!inRage)
        {
            attackRate = .33f;
            shootRate = .67f;
            healRate = 0f;
        }
        else
        {
            attackRate = .25f;
            shootRate = .5f;
            healRate = .25f;
        }

        if (idle)
        {
            LookAtPlayer();
            
            if ((!inRage && idleTime >= decisionTime) || (inRage && idleTime >= rageDecisionTime))
            {
                idleTime = 0f;
                idle = false;
                if (Random.Range(0, .99f) < attackRate)
                {
                    towardsPlayer = true;
                    anim.SetTrigger("ToMove");
                }
                else if (Random.Range(0, .99f) < (attackRate + shootRate))
                {
                    anim.SetTrigger("ToShoot");
                }
                else
                {
                    towardsPlayer = false;
                    anim.SetTrigger("ToMove");
                }
            }
            else
                idleTime += Time.deltaTime;
        }
        
    }

    public void ResetIdle()
    {
        idle = true;
    }

    public void GoingBack()
    {
        backing = true;
    }


    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void Attack()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, stats.meleeRange, playerLayer);

        if (hitPlayer != null)
        {
            Vector2 knockbackVector = (hitPlayer.transform.position - transform.position).normalized * stats.meleeKnockback;

            hitPlayer.GetComponent<PlayerCombat>().TakeDamage(stats.meleeDamage, knockbackVector);
        }

    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        Bullet bulletController = bullet.GetComponent<Bullet>();

        bulletController.shooter = Bullet.Shooter.enemy;
        bulletController.bulletSpeed = stats.bulletSpeed;
        bulletController.shootKnockback = stats.rangeKnockback;
        bulletController.shootDamage = stats.rangeDamage;
    }

    public void Jump()
    {
        //Nos aseguramos de que solo añade el impulso cuando no tiene velocidad en y
        if (Mathf.Abs(rb.velocity.y) <= 0.5f)
            rb.AddForce(Vector2.up * stats.jumpHeight, ForceMode2D.Impulse);
    }

    public Transform nearestWall()
    {
        Transform wallPoint;

        if (isFlipped)
            wallPoint = wallPoints[0];
        else
            wallPoint = wallPoints[1];

        return wallPoint;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(10, new Vector2(stats.meleeKnockback, 0f));
        }
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        actualHealth -= damage;

        healthBar.fillAmount = (float)actualHealth / stats.healthPoints;
        //if (actualHealth <= 0)
        //Die();
    }

    public void InstantiateOrbAttack()
    {
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag("Spawnable");
        if (spawnTime >= spawnRate)
        {
            if (spawnedObjects.Length < 3)
            {
                GameObject bounceAttack = Instantiate(healAttackPrefab, transform.position, transform.rotation);
                bounceAttack.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-45, 45), -45).normalized * 15f, ForceMode2D.Impulse);

                Destroy(bounceAttack, 10f);
                spawnTime = 0f;
            }
        }
        else
            spawnTime += Time.deltaTime;
    }

    public void SpawnAntennas()
    {
        antennaProtectionPrefabs[0].SetActive(true);
        antennaProtectionPrefabs[1].SetActive(true);
    }

}
