using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float maximumTime = 180f;
    float currentTime;
    private Image timerImage;

    private void Start()
    {
        timerImage = GetComponent<Image>();
        currentTime = maximumTime;
        timerImage.fillAmount = 1;    
    }

    private void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        UpdateThermometerVisual();
    
        if(currentTime == 0)
        {
            Debug.Log("GameOver");
        }
    }

    void UpdateThermometerVisual()
    {
        float fillRatio = Mathf.InverseLerp(0, maximumTime, currentTime);
        timerImage.fillAmount = fillRatio;
    }
}
