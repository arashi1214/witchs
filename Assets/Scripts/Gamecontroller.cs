using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gamecontroller : MonoBehaviour
{
    [Header("溫度計")]
    public Image mercury;
    public int minTemp = 0;
    public int maxTemp = 100;
    public GameObject[] BuildFire;

    private int currentTemp = 25;
    public GameObject winScene;

    [Header("UI")]
    // 暫停畫面
    public GameObject PauseScene;
    public bool isPausing;

    public GameObject EndScene;

    // 溫度下降
    private float timer = 0f;
    private float repeatInterval = 3.0f;


    //敵人
    public GameObject Enemys;

    // Start is called before the first frame update
    void Start()
    {
        PauseScene.SetActive(false);
        winScene.SetActive(false);
        isPausing = false;
        UpdateThermometerVisual(currentTemp);
        CreateEnemys();

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
            gameWin();
            Debug.Log("遊戲結束");
        }
        else if (currentTemp >= 75)
        {
            BuildFire[0].SetActive(false);
            BuildFire[1].SetActive(false);
            BuildFire[2].SetActive(true);
        }
        else if (currentTemp >= 50 && currentTemp < 75)
        {
            BuildFire[0].SetActive(false);
            BuildFire[1].SetActive(true);
            BuildFire[2].SetActive(false);
        }
        else if (currentTemp >= 30 && currentTemp < 50)
        {
            BuildFire[0].SetActive(true);
            BuildFire[1].SetActive(false);
            BuildFire[2].SetActive(false);
        }

        else
        {
            BuildFire[0].SetActive(false);
            BuildFire[1].SetActive(false);
            BuildFire[2].SetActive(false);
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
    }
        
    void UpdateThermometerVisual(int newTemp)
    {
        float fillRatio = Mathf.InverseLerp(minTemp, maxTemp, newTemp);
        mercury.fillAmount = fillRatio;
    }

    void CreateEnemys(){
        StartCoroutine(WaitAndDoAction());
    }

    IEnumerator WaitAndDoAction()
    {
        yield return new WaitForSeconds(1f);
        var groups = GameObject.Find("EnemysCreatePoint");
        GameObject newWitch = Instantiate(Enemys, groups.transform);
        SpriteRenderer witchRenderer = newWitch.transform.Find("Image").GetComponent<SpriteRenderer>();

        if (witchRenderer != null)
        {
            // 3. 設置 Mask Interaction 屬性
            // 這是關鍵步驟！告訴這個 Sprite Renderer 要被場景中的 Sprite Mask 裁剪

            // 選項 A: 只在遮罩範圍內顯示
            //witchRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            // 選項 B: 只在遮罩範圍外顯示
           // witchRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask; 
        }

        Debug.Log("敵人生成");
    }

    public void gameOver()
    {
        Debug.Log("Game finsh");
        EndScene.SetActive(true);
        Time.timeScale = 0;
    }

    public void gameWin()
    {
        winScene.SetActive(true);
        Time.timeScale = 0;
    }

}
