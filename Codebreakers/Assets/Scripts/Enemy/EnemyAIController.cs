using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIController : MonoBehaviour
{
    public Transform player;
    public EnemyStats stats;

    [HideInInspector] public EnemyMovement enemyMovement;
    //Acceso a los estados
    [HideInInspector] public AIInterface currentState;
    [HideInInspector] public ChaseState chaseState;
    [HideInInspector] public PatrolState patrolState;


    private void Awake()
    {
        chaseState = new ChaseState(this);
        patrolState = new PatrolState(this);
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.target = player;     //PARA PROBAR
        currentState = patrolState;
    }

    private void FixedUpdate()
    {
        currentState.UpdateState();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.lookRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(stats.maxAngle, transform.forward) * transform.right * stats.lookRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-stats.maxAngle, transform.forward) * transform.right * stats.lookRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * stats.lookRange);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position,transform.right * stats.lookRange);
    }
}
