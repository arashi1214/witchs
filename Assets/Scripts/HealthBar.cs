using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxValue;
    public float currentValue;
    public Image HPbar;
    public GameObject GameController;


    private void Start()
    {
        currentValue = maxValue;
    }

    public void TakeDamage(float damage)
    {
        currentValue -= damage;
        UpdateThermometerVisual();

        if (currentValue <= 0)
        {
            GameController.SendMessage("gameOver");
        }
    }

    void UpdateThermometerVisual()
    {
        float fillRatio = Mathf.InverseLerp(0, maxValue, currentValue);
        HPbar.fillAmount = fillRatio;
    }
}
