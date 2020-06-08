using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scripts/Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject 
{
    //BASIC STATS
    public int healthPoints = 100;              //Vida del enemigo, al llegar a 0, muere
    public float moveSpeed = 400;               //Velocidad de movimiento
    public float maxRunSpeed = 5f;              //Velocidad máxima que puede alcanzar el Rgidbody
    public float patrolSpeed = 200f;            //Velocidad andando
    public float maxWalkSpeed = 2f;             //Velocidad máxima que puede alcanzar el Rgidbody andadno
    public float jumpHeight = 1f;               //Altura del salto
    public float maxAngle;                      //Ángulo de visión (Mitad del valor completo)
    public float lookRange = 40f;               //Alcance de la visión del enemigo
    public float jumpRange = 15f;               //Distancia a la que el enmigo empieza el salto para superar un obstáculo
    public float nextWaypointDistance = 3f;     //Distancia a la que se para el Rigidbody del target del movimiento

    public float timeChaseLost = 3f;            //Tiempo que debe pasar sin ver al jugador para cesar la persecución

    //TOGGLE CAPABILITES
    public bool canMove = true;                 //El enemigo puede moverse
    public bool canJump = true;                 //El enemigo puede saltar
    public bool canFly = false;                 //El enemigo puede volar
    public bool meleeAttack = true;             //El enemigo dispone de ataque cuerpo a cuerpo
    public bool rangedAttack = false;           //El enemigo dispone de ataque a distancia

    //MELEE ATTACK STATS
    public float meleeRange = .5f;
    public float meleeSpeedAttack = 1f;
    public int meleeDamage = 10;
    public float meleeKnockback = 1f;

    //RANGED ATTACK STATS 

    public enum ShootType
    {
        bullet,
        raycast,
        launcher
    }

    public ShootType shootType = ShootType.bullet;
    public float bulletSpeed = 20f;
    public float rangedRange = 5f;
    public float rangeAttackRate = 1f;
    public int rangeDamage = 25;
    public float rangeKnockback = .1f;
}
