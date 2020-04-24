using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ChaseState : AIInterface
{
    private readonly EnemyAIController enemyAI;

    private float timePlayerLost = 0f;

    public ChaseState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        Debug.Log("To Chase State");
        enemyAI.currentState = enemyAI.chaseState;
    }

    public void ToPatrolState()
    {
        Debug.Log("To Patrol State");
        enemyAI.currentState = enemyAI.patrolState;
    }

    public void ToSearchState()
    {
        Debug.Log("To Search State");
        enemyAI.currentState = enemyAI.searchState;
    }

    public void UpdateState()
    {
        if (CheckMeleeDistance() && enemyAI.stats.meleeAttack && enemyAI.FindPlayer())
        {
            enemyAI.Attack();
        }
        else
        {
            if (CheckRangedDistance() && enemyAI.stats.rangedAttack && enemyAI.FindPlayer())
            {
                enemyAI.Shoot();
            }
            else
            {
               
                if (enemyAI.stats.canMove)
                { 
                    enemyAI.enemyMovement.Move(enemyAI.stats.moveSpeed, enemyAI.stats.maxRunSpeed);

                    if (enemyAI.enemyMovement.CheckObstacleForward())
                        enemyAI.enemyMovement.Jump();
                }

                
                if (!enemyAI.FindPlayer())
                {
                    if (timePlayerLost >= enemyAI.stats.timeChaseLost)
                        ToSearchState();
                    else
                        timePlayerLost += Time.deltaTime;
                }
                else
                    timePlayerLost = 0f;
            }
        }
        
    }

    private bool CheckRangedDistance()
    {
        float distance = (enemyAI.player.transform.position - enemyAI.transform.position).x;
        float height = (enemyAI.player.transform.position - enemyAI.transform.position).y;

        if (Mathf.Abs(distance) <= enemyAI.stats.rangedRange && height <= 0.85f)
            return true;
        else
            return false;
    }

    private bool CheckMeleeDistance()
    {
        float distance = (enemyAI.player.transform.position - enemyAI.attackPoint.transform.position).x;   

        if (Mathf.Abs(distance) <= enemyAI.stats.meleeRange)
            return true;
        else
            return false;
    }
}
