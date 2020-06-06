using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBuff : MonoBehaviour
{
    [SerializeField] private GameObject tutorialText;

    public enum Buff
    {
        laserAim,
        health
    }

    public Buff buffEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCombat>() != null)
        {
            if (tutorialText != null)
            {
                tutorialText.SetActive(true);
            }

            if (buffEffect == Buff.laserAim)
            {
                collision.GetComponent<PlayerCombat>().aimingBuff = true;
                GameController.instance.aimBuff = true;
            }
            else if (buffEffect == Buff.health)
            {
                collision.GetComponent<PlayerCombat>().Heal(50);
            }


            Destroy(gameObject);
        }
    }
}
