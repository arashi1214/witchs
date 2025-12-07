using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float maxValue;
    public float currentValue;

    private void Start()
    {
        currentValue = maxValue;
    }

    public void TakeDamage(float damage)
    {
        currentValue -= damage;
        Debug.Log(currentValue);
    }
}
