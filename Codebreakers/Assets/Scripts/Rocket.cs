using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody2D rb;

    public enum Shooter
    {
        player,
        enemy
    }

    [HideInInspector] public Shooter shooter;

    [HideInInspector] public float rocketSpeed;
    private float rocketLife;
    [HideInInspector] public float shootKnockback;
    [HideInInspector] public int shootDamage;
    private float time = 0f;
    private float aimingTime = 0.3f;
    private Transform target;
    private bool targetAcquired = false;

    // Start is called before the first frame update
    void Start()
    {
        rocketLife = 10f;
        if (shooter == Shooter.player)
        {
            PlayerStats stats = FindObjectOfType<CharacterController2D>().stats;
            rocketSpeed = stats.bulletSpeed;
            shootKnockback = stats.rangeKnockback;
            shootDamage = stats.rangeDamage;
        }
        else if (shooter == Shooter.enemy)
        {
           /* EnemyStats stats = FindObjectOfType<EnemyAIController>().stats;
            rocketSpeed = stats.bulletSpeed;
            rocketLife = 10f;
            shootKnockback = stats.rangeKnockback;
            shootDamage = stats.rangeDamage;*/
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (time >= rocketLife)
            Destroy(gameObject);
        else
        {
            if (time >= aimingTime &! targetAcquired)
                AimForTarget();


            time += Time.deltaTime;

        }

    }

    private void AimForTarget()
    {
        targetAcquired = true;
        Vector3 targetPos = target.position;
        targetPos.z = 0f;

        Vector3 objectPos = transform.position;
        targetPos.x = targetPos.x - objectPos.x;
        targetPos.y = targetPos.y - objectPos.y;

        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        rb.velocity = (target.position - transform.position).normalized * rocketSpeed;

    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);

        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);


        if (shooter == Shooter.player)
        {
            EnemyAIController enemy = collision.gameObject.GetComponent<EnemyAIController>();
            if (enemy != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized * shootKnockback;
                enemy.TakeDamage(shootDamage, knockbackDirection);
                Destroy(gameObject);
            }
        }
        else
        {
            PlayerCombat enemy = collision.gameObject.GetComponent<PlayerCombat>();
            if (enemy != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized * shootKnockback;
                enemy.TakeDamage(shootDamage, knockbackDirection);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);

        if (collision.gameObject.CompareTag("Projectile"))
            Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);

        if (collision.gameObject.CompareTag("Projectile"))
            Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Obstacle"))
            Destroy(gameObject);
    }


}
