using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public LayerMask enemyLayers;
    public Image healthBar;
    public float hitTime;

    private CharacterController2D characterController;

    private float attackCooldown;
    private float shootCooldown;
    private int actualHealth;
    private Rigidbody2D rb;
    private SceneController sceneController;
    //private Renderer rend;
    //private Color rendColor;
    private bool invulnerable = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        sceneController = FindObjectOfType<SceneController>();
       // rend = GetComponent<Renderer>();

        //rendColor = rend.material.color;
        attackCooldown = characterController.stats.meleeSpeedAttack;
        shootCooldown = characterController.stats.rangeAttackRate;
        actualHealth = characterController.stats.healthPoints;

        healthBar.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1") && shootCooldown >= characterController.stats.rangeAttackRate)
        {
            Shoot();
            shootCooldown = 0f;
        }
        else if (shootCooldown < characterController.stats.rangeAttackRate)
            shootCooldown += Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.E) && attackCooldown >= characterController.stats.meleeSpeedAttack)
        {
            Attack();
            attackCooldown = 0f;
        }
        else if (attackCooldown < characterController.stats.meleeSpeedAttack)
            attackCooldown += Time.deltaTime;
    }

    private void Shoot()
    {
        if (characterController.stats.shootType == PlayerStats.ShootType.bullet)
        {
            GameObject bullet = Instantiate(bulletPrefab, attackPoint.position, attackPoint.rotation);
            bullet.GetComponent<Bullet>().shooter = Bullet.Shooter.player;
        }
    }

    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, characterController.stats.meleeRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackVector = (enemy.transform.position - transform.position).normalized * characterController.stats.meleeKnockback;
            

            enemy.GetComponent<EnemyAIController>().TakeDamage(characterController.stats.meleeDamage, knockbackVector);
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (invulnerable)
            return;

        actualHealth -= damage;
        
        healthBar.fillAmount = (float) actualHealth/characterController.stats.healthPoints;

        StartCoroutine("InvulnerableFrames");
        rb.AddForce(knockback);

        if (actualHealth <= 0)
            Die();
    }

    IEnumerator InvulnerableFrames()
    {
        Physics2D.IgnoreLayerCollision(9, 10, true);
        Physics2D.IgnoreLayerCollision(9, 11, true);
       // rendColor.a = 0.5f;
       // rend.material.color = rendColor;
        invulnerable = true;
        yield return new WaitForSeconds(hitTime);
        Physics2D.IgnoreLayerCollision(9, 10, false);
        Physics2D.IgnoreLayerCollision(9, 11, false);
      // rendColor.a = 1f;
//rend.material.color = rendColor;
        invulnerable = false;
    }

    private void Die()
    {
        sceneController.GameOver();
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null || characterController == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, characterController.stats.meleeRange);
    }
}
