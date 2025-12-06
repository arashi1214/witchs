using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("祇甮砞﹚")]
    [SerializeField] private float maxDragDistance = 3f; // 程╈Σ禯瞒
    [SerializeField] private float forceMultiplier = 5f; // 秖计

    [Header("瓂格箇代")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    [Header("竚竚")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -3f); // 紆竚

    [Header("计")]
    [SerializeField] private int Act;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;

    private bool onReadyStatus = false;
    private Vector2 originPosition;
    private GameObject GameController;

    // 篈
    private enum State
    {
        FollowingMouse,  // 蛤繦菲公
        Fire, // 紆縐縉い
        ReadyToLaunch,   // 非称祇甮
        Launched         // 祇甮
    }

    private State currentState = State.FollowingMouse;


    void Awake()
    {
        GameController = GameObject.Find("GameController");
        launchPosition = GameObject.Find("Slingshot").transform.position;

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

        //絋粄琌Τ┰紆矪
        if (Input.GetMouseButtonUp(0))
        {
            originPosition = transform.position;

            if (onReadyStatus)
            {
                transform.position = launchPosition;
                currentState = State.Fire;
                playAnimation();
                Debug.Log("﹚竚");
            }
            else
            {
                transform.position = originPosition;
            }
        }
    }

 
    void HandleReadyToLaunch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }

        if (Input.GetMouseButton(0))
        {
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = launchPosition - dragPosition;

            // ╈Σ禯瞒
            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = launchPosition - direction;

            // 陪ボ瓂格
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

        Debug.Log("щ耏");
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
            Debug.Log("笷竚");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
                Destroy(gameObject);
                GameController.SendMessage("onward", gameObject);
                break;
            case "Enemy":
                GameController.SendMessage("increase_temperature", Act);
                GameController.SendMessage("onward", gameObject);
                Destroy(gameObject);
                break;
        }
    }
    

    IEnumerator WaitAndDoAction()
    {
        yield return new WaitForSeconds(2f);
        currentState = State.ReadyToLaunch;
        transform.localScale = new Vector3(0.075f, 0.075f, 1);
        print("щ耏");
    }

    private void playAnimation()
    {

        StartCoroutine(WaitAndDoAction());
    }
}