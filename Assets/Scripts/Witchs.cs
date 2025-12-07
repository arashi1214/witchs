using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Witchs : MonoBehaviour
{
    [Header("投擲設定")]
    [SerializeField] private float maxDragDistance = 3f;
    [SerializeField] private float forceMultiplier = 5f;

    [Header("火焰軌跡")]
    [SerializeField] private TrailRenderer fireTrailRenderer;
    [SerializeField] private GameObject fire;


    [Header("放置位置")]
    [SerializeField] private Vector2 launchPosition = new Vector2(-6f, -2f);

    [Header("女巫數值")]
    [SerializeField] private int Act;

    [Header("女巫聲音")]
    [SerializeField] private AudioClip[] scream;
    [SerializeField] public AudioSource Effectplayer;

    [Header("碰撞特效")]
    [SerializeField] private ParticleSystem collisionParticlesPrefab; // 存放爆炸特效的 Prefab 引用

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

    public event Action<bool> onLaunching;

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

        if (fireTrailRenderer != null)
        {
            fireTrailRenderer.enabled = false;
        }

        originPosition = transform.position;
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



    void HandleFollowingMouse()
    {
        if (Input.GetMouseButtonUp(0) && checkMouseClick())
        {
            transform.position = launchPosition;
            currentState = State.Fire;
            playAnimation();
            Debug.Log("已到達就位位置");
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
            if(LaunchingAudioManager.Instance != null)
            {
                LaunchingAudioManager.Instance.TriggerLaunchAudioSignal();
            }
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

        int randeffect = UnityEngine.Random.Range(0, scream.Length);
        playEffects(scream[randeffect]);

        if (fireTrailRenderer != null)
        {
            // 確保先清除舊有的軌跡殘留 (如果有的話)
            fireTrailRenderer.Clear();
            fireTrailRenderer.enabled = true; // 啟用 Trail Renderer
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

        if (collision.gameObject.CompareTag("Pope"))
        {
            PropMoving pope = collision.gameObject.GetComponent<PropMoving>();
            if (pope != null)
            {
                pope.TakeHit(); // 呼叫震動
                Debug.Log("Shaking");
            }
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

                    if (collisionParticlesPrefab != null)
                    {
                        // 1. 在碰撞點生成粒子系統的實例 (Instance)
                        ParticleSystem instance = Instantiate(
                            collisionParticlesPrefab,
                            transform.position, // 使用女巫的當前位置作為生成點
                            Quaternion.identity
                        );

                        instance.Play();
                    }


                    if (GameController != null)
                    {
                        GameController.SendMessage("increase_temperature", Act, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("onward", gameObject, SendMessageOptions.DontRequireReceiver);
                        GameController.SendMessage("CreateEnemys", SendMessageOptions.DontRequireReceiver);
                    }
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

        fire.SetActive(true);
        Debug.Log("準備發射");
    }

    private void playAnimation()
    {
        StartCoroutine(WaitAndDoAction());
    }

    public void playEffects(AudioClip clipName)
    {
        Effectplayer.clip = clipName;
        Effectplayer.Play();
    }

}