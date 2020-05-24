using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{

    public GameObject enemySpawns;
    public GameObject door;

    private bool levelOn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enemySpawns.SetActive(true);
            levelOn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyAIController[] enemies = FindObjectsOfType<EnemyAIController>();

        if (enemies.Length == 0 && levelOn)
        {
            door.GetComponent<Animator>().SetTrigger("Open");
        }
        
    }
}
