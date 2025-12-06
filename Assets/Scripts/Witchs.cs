using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("發射設定")]
    [SerializeField] private float maxDragDistance = 3f; // 最大拖曳距離
    [SerializeField] private float forceMultiplier = 5f; // 力量倍數

    [Header("軌跡預測")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    [Header("放置位置")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -3f); // 彈弓位置
    [SerializeField] private float snapDistance = 0.5f; // 吸附距離

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;

    private bool onReadyStatus = false;

    // 女巫狀態
    private enum State
    {
        FollowingMouse,  // 跟隨滑鼠
        ReadyToLaunch,   // 準備發射(在彈弓位置)
        Launched         // 已發射
    }

    private State currentState = State.FollowingMouse;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
    }


    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.FollowingMouse:
                HandleFollowingMouse();
                break;

            case State.ReadyToLaunch:
                HandleReadyToLaunch();
                break;

            case State.Launched:
                break;
        }
    }
    
    void HandleFollowingMouse()
    {
        if (Input.GetMouseButton(0) && checkMouseClick())
        {
            
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }

        //確認是否有拉到彈弓處
        if (Input.GetMouseButtonUp(0))
        {
            if (onReadyStatus)
            {
                transform.position = launchPosition;
                currentState = State.ReadyToLaunch;
                Debug.Log("已到指定位置");
            }
            else
            {
                //Debug.Log("需放到指定位置");
            }
        }
    }

 
    void HandleReadyToLaunch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 確認是否點選物體
            if (Vector2.Distance(mousePos, transform.position) < 1f)
            {
                // 開始拖曳
            }
        }

        if (Input.GetMouseButton(0))
        {
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = launchPosition - dragPosition;

            // 限制拖曳距離
            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = launchPosition - direction;

            // 顯示軌跡
            if (trajectoryLine)
            {
                ShowTrajectory(direction * forceMultiplier);
                trajectoryLine.enabled = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Launch();
        }
    }

    void Launch()
    {
        currentState = State.Launched;

        Vector2 launchForce = (launchPosition - (Vector2)transform.position) * forceMultiplier;

        rb.gravityScale = 1;
        rb.AddForce(launchForce, ForceMode2D.Impulse);

        if (trajectoryLine)
        {
            trajectoryLine.enabled = false;
        }

        Debug.Log("投擲");
    }

    void ShowTrajectory(Vector2 velocity)
    {
        Vector2 position = launchPosition;

        for (int i = 0; i < trajectoryPointCount; i++)
        {
            float time = i * trajectoryTimeStep;
            Vector2 point = position + velocity * time + 0.5f * Physics2D.gravity * time * time;
            trajectoryLine.SetPosition(i, point);
        }
    }


    bool checkMouseClick()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distance = 0.001f;
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, distance);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;
            //Debug.Log("你點擊了 2D 物件: " + clickedObject.name);

            if (clickedObject == gameObject)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.name == "Slingshot")
        {
            onReadyStatus = true;
            Debug.Log("到達位置");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                Destroy(gameObject);
                break;
        }
    }
}