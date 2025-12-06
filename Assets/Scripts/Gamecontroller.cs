using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamecontroller : MonoBehaviour
{
    [Header("溫度計")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 1000;

    private int currentTemp = 25;

    // 暫停
    public GameObject PauseScene;
    public bool isPausing;

    public GameObject EndScene;

    // 溫度計的計時器
    private float timer = 0f;
    private float repeatInterval = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        PauseScene.SetActive(false);
        isPausing = false;
        UpdateThermometerVisual(currentTemp);

    }

    // Update is called once per frame
    void Update()
    {
        // 定時溫度下降
        timer += Time.deltaTime;
        if (timer > repeatInterval)
        {
            if (currentTemp > 0)
            {
                currentTemp -= 1;
                UpdateThermometerVisual(currentTemp);
                timer -= repeatInterval;
                //Debug.Log("溫度下降" + currentTemp);
            }
        }


        // 溫度達到最高點
        if (currentTemp >= maxTemp)
        {
            Debug.Log("遊戲結束");
        }

        // 暫停畫面
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
        currentTemp += degree;
        UpdateThermometerVisual(currentTemp);
        Debug.Log("目前溫度" + currentTemp.ToString());
    }
        
    void UpdateThermometerVisual(int newTemp)
    {
        float fillRatio = Mathf.InverseLerp(minTemp, maxTemp, newTemp);
        mercury.fillAmount = Mathf.Lerp(mercury.fillAmount, fillRatio, Time.deltaTime * 1f);
    }

    public void gameOver()
    {
        Debug.Log("Game finsh");
    }

}
