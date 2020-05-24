using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnText : MonoBehaviour
{
    public GameObject tutorial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterController2D>() != null)
        {
            tutorial.SetActive(true);
        }
    }
}
