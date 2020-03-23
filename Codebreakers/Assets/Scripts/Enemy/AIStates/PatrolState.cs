using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : AIInterface
{
    private readonly EnemyAIController enemyAI;    

    public PatrolState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        Debug.Log("To Chase State");
        enemyAI.currentState = enemyAI.chaseState;
    }

    public void UpdateState()
    {
        if (FindPlayer())
        {
            ToChaseState();
        }
    }

    private bool FindPlayer()
    {
        Vector2 playerDirection = (Vector2)(enemyAI.player.transform.position - enemyAI.transform.position);
        Vector2 direction = playerDirection.normalized;

        float angle = Vector2.Angle(enemyAI.transform.right, direction);

        if (angle <= enemyAI.stats.maxAngle)
        {
            if (playerDirection.x <= enemyAI.stats.lookRange)
            {
                return true;
            }
        }

        return false;
    }


}
