using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("�o�g�]�w")]
    [SerializeField] private float maxDragDistance = 3f; // �̤j�즲�Z��
    [SerializeField] private float forceMultiplier = 5f; // �O�q����

    [Header("�y��w��")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    [Header("��m��m")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -3f); // �u�}��m

    [Header("�k�żƭ�")]
    [SerializeField] private int Act;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;

    private bool onReadyStatus = false;
    private Vector2 originPosition;
    private GameObject GameController;

    // �k�Ū��A
    private enum State
    {
        FollowingMouse,  // ���H�ƹ�
        Fire, // ����u�}�A�U�N��
        ReadyToLaunch,   // �ǳƵo�g
        Launched         // �w�o�g
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

        originPosition = transform.position;
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

        //�T�{�O�_���Ԩ�u�}�B
        if (Input.GetMouseButtonUp(0))
        {

            if (onReadyStatus)
            {
                transform.position = launchPosition;
                currentState = State.Fire;
                playAnimation();
                Debug.Log("�w����w��m");
            }
            else
            {
                rb.MovePosition(originPosition);
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

            // ����즲�Z��
            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = launchPosition - direction;

            // ��ܭy��
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

        Debug.Log("���Y");
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
            Debug.Log("位置就緒");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == State.Launched)
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
                    GameController.SendMessage("CreateEnemys");
                    Destroy(gameObject);
                    Destroy(collision.gameObject);
                    break;
            }
        }

    }
    
    public void update_origin_position(Vector3 targetPosition)
    {
        originPosition = targetPosition;
    }


    IEnumerator WaitAndDoAction()
    {
        yield return new WaitForSeconds(2f);
        currentState = State.ReadyToLaunch;
        transform.localScale = new Vector3(0.075f, 0.075f, 1);
        print("準備發射");
    }

    private void playAnimation()
    {

        StartCoroutine(WaitAndDoAction());
    }
}