using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ChaseState : AIInterface
{
    private readonly EnemyAIController enemyAI;
 

    public ChaseState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        enemyAI.enemyMovement.Move();
    }
}
