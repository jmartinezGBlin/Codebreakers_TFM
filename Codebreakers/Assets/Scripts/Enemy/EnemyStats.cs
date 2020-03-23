using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject 
{
    public float healthPoints = 100f;
    public float moveSpeed = 1;
    public float maxAngle;
    public float lookRange = 40f;
    public float nextWaypointDistance = 3f;


    public bool canMove = true;
    public bool canFly = false;
    public bool meleeAttack = true;
    public bool rangedAttack = false;

    public float meleeRange = .5f;
    public float meleeSpeedAttack = 1f;
    public float meleeDamage = 10f;
    public float meleeKnockback = 1f;

    public float rangedRange = 5f;
    public float rangeAttackRate = 1f;
    public float rangeDamage = 25f;
    public float rangeKnockback = .1f;

}
