using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamecontroller : MonoBehaviour
{
    [Header("遊戲設置")]
    public int GameTime = 180;

    [Header("溫度計")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 100;

    private int currentTemp = 25;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
