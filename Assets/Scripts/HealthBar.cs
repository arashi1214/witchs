using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxValue;
    public float currentValue;
    public Image HPbar;
    

    private void Start()
    {
        currentValue = maxValue;
    }

    public void TakeDamage(float damage)
    {
        currentValue -= damage;
        UpdateThermometerVisual();
    }

    void UpdateThermometerVisual()
    {
        float fillRatio = Mathf.InverseLerp(0, maxValue, currentValue);
        HPbar.fillAmount = fillRatio;
    }
}
