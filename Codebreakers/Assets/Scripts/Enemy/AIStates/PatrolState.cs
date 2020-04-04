using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : AIInterface
{
    private readonly EnemyAIController enemyAI;

    private int currentPatrolPoint = 0;
    private bool arrivedOnTarget;
    private float waitingOnPoint = 0f;
    private float waitingTime = .5f;

    public PatrolState(EnemyAIController enemyAIController)
    {
        enemyAI = enemyAIController;
    }

    public void ToChaseState()
    {
        Debug.Log("To Chase State");
        enemyAI.enemyMovement.target = enemyAI.player.transform;
        enemyAI.currentState = enemyAI.chaseState;
    }

    public void ToPatrolState()
    {
        throw new System.NotImplementedException();
    }

    public void ToSearchState()
    {
        throw new System.NotImplementedException();
    }
    

    public void UpdateState()
    {
        if (enemyAI.stats.canMove && enemyAI.patrolWaypoints.Length > 0)
        {
            enemyAI.enemyMovement.Move(enemyAI.stats.patrolSpeed);


            float distance = Vector2.Distance(enemyAI.transform.position, enemyAI.enemyMovement.target.transform.position);

            if (distance <= enemyAI.stats.nextWaypointDistance - 0.01f)
                arrivedOnTarget = true;
            

            if (arrivedOnTarget)
            {
                if (waitingOnPoint >= waitingTime)
                {
                    currentPatrolPoint++;

                    if (currentPatrolPoint >= enemyAI.patrolWaypoints.Length)
                        currentPatrolPoint = 0;

                    waitingOnPoint = 0f;
                    arrivedOnTarget = false;

                    enemyAI.enemyMovement.target = enemyAI.patrolWaypoints[currentPatrolPoint];
                }
                else
                    waitingOnPoint += Time.deltaTime;
            }

        }

        if (enemyAI.FindPlayer())
            ToChaseState();
    }

   


}
