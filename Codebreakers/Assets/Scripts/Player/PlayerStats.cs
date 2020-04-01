using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/Player/PlayerStats")]
public class PlayerStats : ScriptableObject 
{
    public int healthPoints = 100;
    public float moveSpeed = 40f;
    public float jumpForce = 400f;
    
    public float meleeRange = .5f;
    public float meleeSpeedAttack = 1f;
    public int meleeDamage = 20;
    public float meleeKnockback = 1f;

    public enum ShootType
    {
        bullet,
        raycast
    }

    public ShootType shootType = ShootType.bullet;
    public float rangedRange = 5f;                 //Tiempo máximo de vida de la bala
    public float rangeAttackRate = .25f;
    public int rangeDamage = 25;
    public float rangeKnockback = 0f;
    public float bulletSpeed = 20f;

}
