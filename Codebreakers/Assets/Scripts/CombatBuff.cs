using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBuff : MonoBehaviour
{
    [SerializeField] private GameObject tutorialText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCombat>() != null)
        {
            if (tutorialText != null)
            {
                tutorialText.SetActive(true);
            }

            collision.GetComponent<PlayerCombat>().aimingBuff = true;
            GameController.instance.aimBuff = true;
            Destroy(gameObject);
        }
    }
}
