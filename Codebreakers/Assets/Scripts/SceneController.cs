using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject controlsPanel;

    /// <summary>
    /// Carga una nueva partida.
    /// </summary>
	public void NewGame()
    {
        GameController.instance.actualHealth = 100;
        GameController.instance.firstLevel = true;
        GameController.instance.gameLevel = 1;
        GameController.instance.levelScene = 1;
        GameController.instance.aimBuff = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Level 1-1");
    }

    /// <summary>
    /// Cierra el juego y sale al escritorio.
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
    

    /// <summary>
    /// Carga el menú inicial.
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }
}
