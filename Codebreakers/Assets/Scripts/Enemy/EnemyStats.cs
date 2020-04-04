using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject 
{
    //BASIC STATS
    public int healthPoints = 100;
    public float moveSpeed = 400;
    public float patrolSpeed = 200f;
    public float jumpHeight = 1f;
    public float maxAngle;
    public float lookRange = 40f;
    public float jumpRange = 15f;
    public float nextWaypointDistance = 3f;

    public float timeChaseLost = 3f;

    //
    public bool canMove = true;
    public bool canFly = false;
    public bool meleeAttack = true;
    public bool rangedAttack = false;

    //MELEE ATTACK STATS
    public float meleeRange = .5f;
    public float meleeSpeedAttack = 1f;
    public int meleeDamage = 10;
    public float meleeKnockback = 1f;

    //RANGED ATTACK STATS 

    public enum ShootType
    {
        bullet,
        raycast
    }

    public ShootType shootType = ShootType.bullet;
    public float bulletSpeed = 20f;
    public float rangedRange = 5f;
    public float rangeAttackRate = 1f;
    public int rangeDamage = 25;
    public float rangeKnockback = .1f;
}
