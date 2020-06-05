using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouncingAttack : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(20, new Vector2(2000, 0f));
            Destroy(this.gameObject);
        }
    }
}
