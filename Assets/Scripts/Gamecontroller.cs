using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamecontroller : MonoBehaviour
{
    [Header("放璸")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 1000;

    private int currentTemp = 25;

    // 既氨
    public GameObject PauseScene;
    public bool isPausing;

    // 放璸璸竟
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
        timer += Time.deltaTime;
        if (timer > repeatInterval)
        {
            if (currentTemp > 0)
            {
                currentTemp -= 1;
                UpdateThermometerVisual(currentTemp);
                timer -= repeatInterval;
                Debug.Log("放" + currentTemp);
            }
        }


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
        Debug.Log("ヘ玡放" + currentTemp.ToString());
    }
        
    void UpdateThermometerVisual(int newTemp)
    {
        float fillRatio = Mathf.InverseLerp(minTemp, maxTemp, newTemp);
        mercury.fillAmount = Mathf.Lerp(mercury.fillAmount, fillRatio, Time.deltaTime * 1f);
    }

}
