using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MeleeState : AIInterface
{
    private readonly EnemyAIController enemyAI;

    private float timePlayerLost = 0f;

    public MeleeState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        throw new System.NotImplementedException();
    }

    public void ToMeleeState()
    {
        throw new System.NotImplementedException();
    }

    public void ToPatrolState()
    {
        Debug.Log("To Patrol State");
        enemyAI.currentState = enemyAI.patrolState;
    }

    public void ToSearchState()
    {
        throw new System.NotImplementedException();
    }

    public void ToShootState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
       
    }
}
