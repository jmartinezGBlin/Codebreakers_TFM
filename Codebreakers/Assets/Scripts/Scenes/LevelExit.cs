using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField]
    private Animator transition;

    private enum Connection
    {
        previous,
        next,
        final
    };
    [SerializeField]

    private Connection jumpTo;
    private bool loading = false;
    private CharacterController2D player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (loading)
        {
            player.stopInput = true;
            return;
        }

        if (collision.CompareTag("Player"))
        {
            loading = true;
            collision.GetComponent<PlayerCombat>().SavePlayer();

            if (jumpTo == Connection.previous)
                GameController.instance.levelScene--;
            else if (jumpTo == Connection.next)
                GameController.instance.levelScene++;


            string sceneName;
            if (jumpTo == Connection.final)
                sceneName = "GameOver";
            else
                sceneName = "Level " + GameController.instance.gameLevel + "-" + GameController.instance.levelScene;

            StartCoroutine(LoadScene(sceneName));
        }
    }

    IEnumerator LoadScene(string scene)
    {
        transition.SetTrigger("fade");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(scene);
    }
}
