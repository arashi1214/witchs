using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamecontroller : MonoBehaviour
{
    [Header("�ū׭p")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 1000;

    private int currentTemp = 25;

    // �Ȱ�
    public GameObject PauseScene;
    public bool isPausing;

    public GameObject EndScene;

    // �ū׭p���p�ɾ�
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
        //定期降溫
        timer += Time.deltaTime;
        if (timer > repeatInterval)
        {
            if (currentTemp > 0)
            {
                currentTemp -= 1;
                UpdateThermometerVisual(currentTemp);
                timer -= repeatInterval;
            }
        }


        // 溫度達到最高
        if (currentTemp >= maxTemp)
        {
            Debug.Log("遊戲結束");
        }

        // 暫停按鈕
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
        Debug.Log("�ثe�ū�" + currentTemp.ToString());
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
