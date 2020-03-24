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
        throw new System.NotImplementedException();
    }

    public void ToPatrolState()
    {
        Debug.Log("To Patrol State");
        enemyAI.currentState = enemyAI.patrolState;
    }

    public void UpdateState()
    {
        if (enemyAI.stats.canMove)
            enemyAI.enemyMovement.Move(enemyAI.stats.moveSpeed);

        if (enemyAI.enemyMovement.CheckObstacleForward())
            enemyAI.enemyMovement.Jump();

        if (!enemyAI.FindPlayer())
        {
            if (timePlayerLost >= enemyAI.stats.timeChaseLost)
                ToPatrolState();
            else
                timePlayerLost += Time.deltaTime;
        }
        else
            timePlayerLost = 0f;
    }
}
