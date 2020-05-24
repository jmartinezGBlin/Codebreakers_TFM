using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    public enum Shooter
    {
        player,
        enemy
    }

    [HideInInspector] public Shooter shooter;

    [HideInInspector] public float bulletSpeed;
    private float bulletLifeSpawn;
    [HideInInspector] public float shootKnockback;
    [HideInInspector] public int shootDamage;
    private float bulletLife = 0f;

    // Start is called before the first frame update
    void Start()
    {
        bulletLifeSpawn = 10f;
        if (shooter == Shooter.player)
        {
            PlayerStats stats = FindObjectOfType<CharacterController2D>().stats;
            bulletSpeed = stats.bulletSpeed;
            bulletLifeSpawn = stats.rangedRange;
            shootKnockback = stats.rangeKnockback;
            shootDamage = stats.rangeDamage;
        }
       /* else if (shooter == Shooter.enemy)
        {
            EnemyStats stats = FindObjectOfType<EnemyAIController>().stats;
            bulletSpeed = stats.bulletSpeed;
            bulletLifeSpawn = 10f;
            shootKnockback = stats.rangeKnockback;
            shootDamage = stats.rangeDamage;
        }
        */
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * bulletSpeed;
    }

    private void Update()
    {
        if (bulletLife >= bulletLifeSpawn)
            Destroy(gameObject);
        else
            bulletLife += Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
            Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
            Destroy(gameObject);

        if (shooter == Shooter.player)
        {
            EnemyAIController enemy = collision.GetComponent<EnemyAIController>();
            if (enemy != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized * shootKnockback;
                enemy.TakeDamage(shootDamage, knockbackDirection);
                Destroy(gameObject);
            }

        /*    if (!collision.CompareTag("Player"))
                Destroy(gameObject);*/
        }
        else
        {
            PlayerCombat enemy = collision.GetComponent<PlayerCombat>();
            if (enemy != null)
            {
                Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized * shootKnockback;
                enemy.TakeDamage(shootDamage, knockbackDirection);
                Destroy(gameObject);
            }

       /*     if (!collision.CompareTag("Enemy"))
                Destroy(gameObject);*/
        }
    }

}
