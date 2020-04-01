using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SearchState : AIInterface
{
 //************************************ IMPLEMENTACIÓN DEL INTERFAZ **********************************
    private readonly EnemyAIController enemyAI;

    public SearchState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        Debug.Log("To Chase State");
        enemyAI.currentState = enemyAI.chaseState;
    }

    public void ToMeleeState()
    {
        Debug.Log("To Melee State");
        enemyAI.currentState = enemyAI.meleeState;
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

    public void ToShootState()
    {
        Debug.Log("To Shoot State");
        enemyAI.currentState = enemyAI.shootState;
    }
    //******************************************************************************************************

    private float lookTime = 2f;
    private float searchTimer = 0f;

    private int lookSides = 0;
    
    public void UpdateState()
    {
        if (enemyAI.FindPlayer())
        {
            ToChaseState();
        }

        if (lookSides < 2)
        {
            if (searchTimer >= lookTime)
            {
                enemyAI.enemyMovement.Flip();
                lookSides++;
                searchTimer = 0f;
            }
            else
                searchTimer += Time.deltaTime;
        }
        else
        {
            ToPatrolState();
        }
    }
}
