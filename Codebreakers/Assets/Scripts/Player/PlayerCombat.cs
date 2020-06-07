using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public Transform shootingPoint;
    public Transform attackPoint;
    public Transform target;
    public GameObject bulletPrefab;
    public LayerMask enemyLayers;
    public Image healthBar;
    public float hitTime;
    public float damageTime;
    public float attackAnimTime;

    public Transform leftArm;
    public Transform rightArm;

    public Laser laserAim;

    public AudioSource shootingAudio;
    public AudioSource meleeAudio;
    public AudioSource shootDamage;

    [HideInInspector] public bool attacking;
    [HideInInspector] public bool aiming;
    [HideInInspector] public bool aimingRight;
    [HideInInspector] public bool aimingBuff;



    private CharacterController2D characterController;
    private float attackCooldown;
    private float shootCooldown;
    private int actualHealth;
    private Rigidbody2D rb;
    private SceneController sceneController;
    //private Renderer rend;
    private Color rendColor;
    private Animator anim;
    private Renderer[] rends;
    private bool invulnerable = false;


    private void Start()
    {
        characterController = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        sceneController = FindObjectOfType<SceneController>();
        anim = GetComponent<Animator>();
        rends = GetComponentsInChildren<Renderer>();
        // rend = GetComponent<Renderer>();
        
        attackCooldown = characterController.stats.meleeSpeedAttack;
        shootCooldown = characterController.stats.rangeAttackRate;
        if (GameController.instance.firstLevel)
        {
            GameController.instance.actualHealth = characterController.stats.healthPoints;
        }
        actualHealth = GameController.instance.actualHealth;        

        healthBar.fillAmount = (float)actualHealth/(float)characterController.stats.healthPoints;
        aimingBuff = GameController.instance.aimBuff;
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.dead || characterController.stopInput)
            return;

        if (Input.GetButton("Fire2") && aimingBuff)
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


        if (Input.GetKeyDown(KeyCode.X) && attackCooldown >= characterController.stats.meleeSpeedAttack)
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
            shootingAudio.Play();
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

        meleeAudio.Play();
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
        if (invulnerable || characterController.dead)
            return;

        actualHealth -= damage;
        healthBar.fillAmount = (float) actualHealth/characterController.stats.healthPoints;

        shootDamage.Play();

        StartCoroutine("InvulnerableFrames");
        rb.AddForce(knockback);

        if (actualHealth <= 0)
            Die();
    }

    public void Heal(int hp)
    {
        actualHealth += hp;
        healthBar.fillAmount = (float)actualHealth / characterController.stats.healthPoints;
    }

    IEnumerator InvulnerableFrames()
    {
        anim.SetTrigger("hit");
        Physics2D.IgnoreLayerCollision(9, 10, true);
        Physics2D.IgnoreLayerCollision(9, 11, true);
        foreach (Renderer rend in rends)
        {
            rendColor = rend.material.color;
            rendColor.a = 0.5f;
            rend.material.color = rendColor;
        }
        invulnerable = true;
        yield return new WaitForSeconds(hitTime);
        Physics2D.IgnoreLayerCollision(9, 10, false);
        Physics2D.IgnoreLayerCollision(9, 11, false);

        foreach (Renderer rend in rends)
        {
            rendColor = rend.material.color;
            rendColor.a = 1f;
            rend.material.color = rendColor;
        }
        invulnerable = false;
    }

    private void Die()
    {
        StopAllCoroutines();
        StartCoroutine(DyingTime());
    }

    IEnumerator DyingTime()
    {
        characterController.dead = true;
        yield return new WaitForSeconds(3f);
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
        
    }


    //Save data to game controller
    public void SavePlayer()
    {
        GameController.instance.firstLevel = false;
        GameController.instance.actualHealth = actualHealth;
    }
}
