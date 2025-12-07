using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float maximumTime = 180f;
    float currentTime;
    private Image timerImage;
    private GameObject GameController;

    public AudioSource gameOverAudio;

    private void Start()
    {
        timerImage = GetComponent<Image>();
        currentTime = maximumTime;
        timerImage.fillAmount = 1;
        GameController = GameObject.Find("GameController");
    }

    private void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        UpdateThermometerVisual();
    
        if(currentTime == 0)
        {
            Debug.Log("GameOver");
            GameController.SendMessage("gameOver");
            gameOverAudio.Play();
        }
    }

    void UpdateThermometerVisual()
    {
        float fillRatio = Mathf.InverseLerp(0, maximumTime, currentTime);
        timerImage.fillAmount = fillRatio;
    }

    public void TakeDamage(float damage)
    {

        currentTime -= damage;
        UpdateThermometerVisual();
    }
}
