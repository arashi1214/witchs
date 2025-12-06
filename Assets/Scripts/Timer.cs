using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float maximumTime = 180f;
    float currentTime;

    public Slider slider;

    private void Start()
    {
        currentTime = maximumTime;
        slider.maxValue = maximumTime;
        slider.value = slider.maxValue;
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        slider.value = currentTime;

        if(currentTime == 0)
        {
            Debug.Log("GameOver");
        }
    }
}
