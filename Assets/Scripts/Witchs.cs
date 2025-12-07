using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witchs : MonoBehaviour
{
    [Header("投擲設定")]
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float forceMultiplier = 5f;

    [Header("發射後軌跡")] // 新增一個用於發射後軌跡的 LineRenderer (可選，但推薦分開)
    [SerializeField] private LineRenderer actualTrajectoryLine;



    [Header("放置位置")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -3f);

    [Header("女巫數值")]
    [SerializeField] private int Act;

    //軌跡相關
    private List<Vector3> trajectoryPoints = new List<Vector3>(); // 儲存實際路徑點
    private bool isTrackingTrajectory = false; // 追蹤開關


    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Vector2 dragPosition;

    private bool onReadyStatus = false;
    private Vector2 originPosition;
    private GameObject GameController;

    // 儲存所有子物件的 Rigidbody 和關節
    private List<Rigidbody2D> childRigidbodies = new List<Rigidbody2D>();
    private List<HingeJoint2D> hingeJoints = new List<HingeJoint2D>();
    private List<Collider2D> childColliders = new List<Collider2D>();
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

    private void FixedUpdate()
    {
        // 在 Launched 狀態下且追蹤開啟時才記錄位置
        if (currentState == State.Launched && isTrackingTrajectory)
        {
            RecordAndDrawTrajectory();
        }
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

        // 收集所有子物件的 Collider (不包含主物件的)
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        Collider2D mainCollider = GetComponent<Collider2D>();
        foreach (var collider in allColliders)
        {
            if (collider != mainCollider)
            {
                childColliders.Add(collider);
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

        // 禁用子物件的 Collider,避免阻擋主物件碰撞
        foreach (var collider in childColliders)
        {
            collider.enabled = false;
        }

        // 暫時禁用關節
        foreach (var joint in hingeJoints)
        {
            joint.enabled = false;
        }

        isRagdollActive = false;
        Debug.Log("布娃娃已禁用 - 子物件 Collider 已關閉");
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
        Debug.Log("布娃娃已啟用!");
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

        EnableRagdoll();

        // 【新增】啟動軌跡追蹤
        trajectoryPoints.Clear();
        isTrackingTrajectory = true;

        // 如果有實際軌跡 LineRenderer，啟用它
        if (actualTrajectoryLine)
        {
            actualTrajectoryLine.enabled = true;
        }

        Debug.Log("發射!");
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
        if (currentState == State.Launched)
        {
            switch (collision.gameObject.tag)
            {
                case "Ground":
                    Debug.Log("撞到地面!");
                    Destroy(gameObject);
                    if (GameController != null)
                    {
                        GameController.SendMessage("onward", gameObject, SendMessageOptions.DontRequireReceiver);
                    }
                    break;

                case "Enemy":
                    Debug.Log("撞到敵人!");
                    if (GameController != null)
                    {
                        GameController.SendMessage("increase_temperature", Act, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("onward", gameObject, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("CreateEnemys", SendMessageOptions.DontRequireReceiver);
                    }
                    Destroy(gameObject);
                    Destroy(collision.gameObject);
                    break;

                default:
                    Debug.Log($"撞到其他物體: {collision.gameObject.name}");
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

    void RecordAndDrawTrajectory()
    {
        // 限制每隔一段時間才記錄一個點，以避免線條過密，導致效能下降
        if (Time.fixedTime % 0.05f < Time.fixedDeltaTime) // 例如每 0.05 秒記錄一次
        {
            // 記錄當前世界位置
            trajectoryPoints.Add(transform.position);

            if (actualTrajectoryLine)
            {
                // 更新 LineRenderer 的點數和位置
                actualTrajectoryLine.positionCount = trajectoryPoints.Count;
                actualTrajectoryLine.SetPositions(trajectoryPoints.ToArray());
            }

            // 【停止追蹤條件】
            // 判斷速度是否過低（落地或停住）來停止追蹤
            if (rb.velocity.sqrMagnitude < 0.1f && rb.IsSleeping())
            {
                isTrackingTrajectory = false;
                Debug.Log("軌跡追蹤停止，女巫已靜止。");
            }
        }
    }
}