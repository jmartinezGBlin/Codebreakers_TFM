using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionAntenna : MonoBehaviour
{
    public BossBehaviour boss;

    private int maxHealth = 30;
    private int actualHealth;
    private bool destroyed;
    private bool spawned = false;

    private void Awake()
    {
        gameObject.SetActive(false);    
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.inHeal && !destroyed)
        {
            gameObject.SetActive(true);
            if (!spawned)
            {
                actualHealth = maxHealth;
                spawned = true;
            }
        }
        else
        {
            spawned = false;
            destroyed = false;
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (destroyed)
            return;

        actualHealth -= damage;

        if (actualHealth <= 0)
            destroyed = true;
    }
}
