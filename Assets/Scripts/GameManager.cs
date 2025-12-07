using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject PauseScene;
    public bool isPausing;

    private void Start()
    {
        PauseScene.SetActive(false);
        isPausing = false;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            isPausing = !isPausing;
        }

        if(isPausing)
        {
            PauseScene.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            PauseScene.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void Continue()
    {
        PauseScene.SetActive(false);
        Time.timeScale = 1;
        isPausing = false;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }

    public void BacktoMenu()
    {
        SceneManager.LoadScene(0);
    }
}
