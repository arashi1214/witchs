using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("投擲設定")]
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float forceMultiplier = 5f;

    [Header("軌跡預測")]
    [SerializeField] private LineRenderer trajectoryLine;
    [SerializeField] private int trajectoryPointCount = 30;
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    [Header("放置位置")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -3f);

    [Header("女巫數值")]
    [SerializeField] private int Act;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;

    private bool onReadyStatus = false;
    private Vector2 originPosition;
    private GameObject GameController;

    // 儲存所有子物件的 Rigidbody 和關節
    private List<Rigidbody2D> childRigidbodies = new List<Rigidbody2D>();
    private List<HingeJoint2D> hingeJoints = new List<HingeJoint2D>();
    private bool isRagdollActive = false;

    private enum State
    {
        FollowingMouse,
        Fire,
        ReadyToLaunch,
        Launched
    }

    private State currentState = State.FollowingMouse;

    void Awake()
    {
        GameController = GameObject.Find("GameController");
        GameObject slingshot = GameObject.Find("Slingshot");
        if (slingshot != null)
        {
            launchPosition = slingshot.transform.position;
        }

        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.mass = 5f;  // 增加主物件質量
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;

        // 收集所有子物件和關節
        CollectChildComponents();

        // 初始時禁用布娃娃
        DisableRagdoll();

        originPosition = transform.position;
    }

    void CollectChildComponents()
    {
        // 收集所有子物件的 Rigidbody2D
        Rigidbody2D[] allRbs = GetComponentsInChildren<Rigidbody2D>();
        foreach (var childRb in allRbs)
        {
            if (childRb != rb)  // 不包含主物件
            {
                childRigidbodies.Add(childRb);

                // 設定子物件物理屬性
                childRb.gravityScale = 0;
                childRb.mass = 0.8f;
                childRb.drag = 0.3f;
                childRb.angularDrag = 0.5f;
                childRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // 收集所有關節
        HingeJoint2D[] allJoints = GetComponentsInChildren<HingeJoint2D>();
        foreach (var joint in allJoints)
        {
            hingeJoints.Add(joint);
            joint.enableCollision = false;
        }
    }

    void DisableRagdoll()
    {
        // 將所有子物件設為 Kinematic,讓它們跟著主物件移動
        foreach (var childRb in childRigidbodies)
        {
            childRb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 暫時禁用關節
        foreach (var joint in hingeJoints)
        {
            joint.enabled = false;
        }

        isRagdollActive = false;
    }

    void EnableRagdoll()
    {
        // 啟用所有子物件的物理
        foreach (var childRb in childRigidbodies)
        {
            childRb.bodyType = RigidbodyType2D.Dynamic;
        }

        // 啟用關節
        foreach (var joint in hingeJoints)
        {
            joint.enabled = true;
        }

        isRagdollActive = true;
    }

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

        if (Input.GetMouseButtonUp(0))
        {
            if (onReadyStatus)
            {
                transform.position = launchPosition;
                currentState = State.Fire;
                playAnimation();
                Debug.Log("已到達就位位置");
            }
            else
            {
                rb.MovePosition(originPosition);
            }
        }
    }

    void HandleReadyToLaunch()
    {
        if (Input.GetMouseButton(0))
        {
            dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = launchPosition - dragPosition;

            if (direction.magnitude > maxDragDistance)
            {
                direction = direction.normalized * maxDragDistance;
            }

            transform.position = launchPosition - direction;

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

        // 發射時保持布娃娃為剛體(不啟用關節)
        rb.gravityScale = 1;
        rb.constraints = RigidbodyConstraints2D.None;  // 允許旋轉
        rb.AddForce(launchForce, ForceMode2D.Impulse);

        if (trajectoryLine)
        {
            trajectoryLine.enabled = false;
        }

        // 延遲啟用布娃娃效果
        StartCoroutine(EnableRagdollAfterDelay(0.3f));

        Debug.Log("發射!");
    }

    IEnumerator EnableRagdollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 碰撞後才啟用布娃娃
        // 在 OnCollisionEnter2D 中啟用
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
            return hit.collider.gameObject == gameObject;
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
        // 第一次碰撞時才啟用布娃娃
        if (currentState == State.Launched && !isRagdollActive)
        {
            EnableRagdoll();
        }

        if (currentState == State.Launched)
        {
            switch (collision.gameObject.tag)
            {
                case "Ground":
                    Destroy(gameObject, 2f);  // 延遲銷毀
                    if (GameController != null)
                    {
                        GameController.SendMessage("onward", gameObject, SendMessageOptions.DontRequireReceiver);
                    }
                    break;

                case "Enemy":
                    if (GameController != null)
                    {
                        GameController.SendMessage("increase_temperature", Act, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("onward", gameObject, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("CreateEnemys", SendMessageOptions.DontRequireReceiver);
                    }
                    Destroy(gameObject, 1f);
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
        Debug.Log("準備發射");
    }

    private void playAnimation()
    {
        StartCoroutine(WaitAndDoAction());
    }
}