using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Carga una nueva partida.
    /// </summary>
	public void NewGame()
    {
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
}
