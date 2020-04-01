using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public LayerMask enemyLayers;

    private CharacterController2D stats;

    private float attackCooldown;
    private float shootCooldown;

    private void Start()
    {
        stats = GetComponent<CharacterController2D>();
        attackCooldown = stats.attackRate;
        shootCooldown = stats.shootRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && shootCooldown >= stats.shootRate)
        {
            Shoot();
            shootCooldown = 0f;
        }
        else if (shootCooldown < stats.shootRate)
            shootCooldown += Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.E) && attackCooldown >= stats.attackRate)
        {
            Attack();
            attackCooldown = 0f;
        }
        else if (attackCooldown < stats.attackRate)
            attackCooldown += Time.deltaTime;
    }

    private void Shoot()
    {
        if (stats.shootType == PlayerStats.ShootType.bullet)
        {
            Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, stats.meleeRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackVector = (enemy.transform.position - transform.position).normalized * stats.meleeKnockback;
            

            enemy.GetComponent<EnemyAIController>().TakeDamage(stats.meleeDamage, knockbackVector);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null || stats == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, stats.meleeRange);
    }
}
