using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamecontroller : MonoBehaviour
{
    [Header("溫度計")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 100;

    private int currentTemp = 25;

    public GameObject PauseScene;
    public bool isPausing;


    // Start is called before the first frame update
    void Start()
    {
        PauseScene.SetActive(false);
        isPausing = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPausing = !isPausing;
        }

        if (isPausing)
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

    public void increase_temperature(int degree)
    {
        Debug.Log("目前溫度" + currentTemp.ToString());
        currentTemp += degree;
        UpdateThermometerVisual(currentTemp);
    }
        
    void UpdateThermometerVisual(int newTemp)
    {
        float fillRatio = Mathf.InverseLerp(minTemp, maxTemp, newTemp);
        mercury.fillAmount = Mathf.Lerp(mercury.fillAmount, fillRatio, Time.deltaTime * 5f);
    }

}
