using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerCombat player;
    private CharacterController2D stats;

    private float bulletLife = 0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerCombat>();
        stats = GameObject.FindObjectOfType<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * stats.bulletSpeed;
    }

    private void Update()
    {
        if (bulletLife >= stats.shootRange)
        {
            Destroy(gameObject);
        }
        else
        {
            bulletLife += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAIController enemy = collision.GetComponent<EnemyAIController>();

        if (enemy != null)
        {
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized * stats.shootKnockback;
            enemy.TakeDamage(stats.shootDamage, knockbackDirection);
        }

        if (!collision.CompareTag("Player"))
            Destroy(gameObject);
    }

}
