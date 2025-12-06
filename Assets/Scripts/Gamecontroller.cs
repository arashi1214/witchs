using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gamecontroller : MonoBehaviour
{
    public int GameTime = 180;
    private int temperature = 25;

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
        Debug.Log("¥Ø«e·Å«×" + temperature.ToString());
    }
}
