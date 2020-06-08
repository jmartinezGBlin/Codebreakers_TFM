using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseScreen;

    private CharacterController2D player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ChangeScreenStatus();
    }

    public void ChangeScreenStatus()
    {
        pauseScreen.SetActive(!pauseScreen.activeSelf);

        if (pauseScreen.activeSelf)
        {
            Time.timeScale = 0f;
            player.stopInput = true;
        }
        else
        {
            Time.timeScale = 1f;
            player.stopInput = false;
        }
    }
}
