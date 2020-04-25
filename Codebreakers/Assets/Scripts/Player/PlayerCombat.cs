using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public Transform shootingPoint;
    public Transform attackPoint;
    public GameObject bulletPrefab;
    public LayerMask enemyLayers;
    public Image healthBar;
    public float hitTime;
    public float damageTime;
    public float attackAnimTime;

    public Transform leftArm;
    public Transform rightArm;

    public Laser laserAim;

    [HideInInspector] public bool attacking;
    [HideInInspector] public bool aiming;
    [HideInInspector] public bool aimingRight;
    [HideInInspector] private bool invulnerable = false;


    private CharacterController2D characterController;
    private float attackCooldown;
    private float shootCooldown;
    private int actualHealth;
    private Rigidbody2D rb;
    private SceneController sceneController;
    //private Renderer rend;
    //private Color rendColor;
    private Animator anim;
    

    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        sceneController = FindObjectOfType<SceneController>();
        anim = GetComponent<Animator>();
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
        if (Input.GetButton("Fire2"))
        {
            aiming = true;
            laserAim.EnableLaser();
            Aim();
        }
        else
        {
            aiming = false;
            laserAim.DisableLaser();
            anim.SetFloat("aimAngle", 0f);
        }
            


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
            GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);
            bullet.GetComponent<Bullet>().shooter = Bullet.Shooter.player;
        }
    }

    private void Aim()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = -Camera.main.transform.position.z;

        Vector3 aimingPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        if (aimingPoint.x - transform.position.x > 0)
            aimingRight = true;
        else
            aimingRight = false;
        
        anim.SetFloat("aimAngle", aimingPoint.y - attackPoint.position.y);

    }

    private void Attack()
    {
        if (!attacking)
            StartCoroutine("Attacking");
        
    }

    IEnumerator Attacking()
    {
        attacking = true;
        anim.SetTrigger("attack");
        yield return new WaitForSeconds(damageTime);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, characterController.stats.meleeRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackVector = (enemy.transform.position - transform.position).normalized * characterController.stats.meleeKnockback;


            enemy.GetComponent<EnemyAIController>().TakeDamage(characterController.stats.meleeDamage, knockbackVector);
        }
        yield return new WaitForSeconds(attackAnimTime);
        attacking = false;
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
        anim.SetTrigger("hit");
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

        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = -Camera.main.transform.position.z;

        Vector3 aimingPoint = Camera.main.ScreenToWorldPoint(screenPoint);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(aimingPoint, 0.2f);

        Gizmos.DrawLine(rightArm.position, shootingPoint.position);
        Gizmos.DrawLine(rightArm.position, aimingPoint);
        Gizmos.DrawLine(shootingPoint.position, aimingPoint);
    }
}
