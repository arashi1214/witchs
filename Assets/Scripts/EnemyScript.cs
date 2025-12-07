using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    // 公開變數：在 Inspector 面板中設定
    //[Tooltip("敵人要移動的目標點陣列 (Waypoints)")]
    private List<Transform> wayPoints = new List<Transform>();
    [Tooltip("敵人的移動速度")]
    public float speed = 5f;
    [Tooltip("敵人到達目標點後的等待時間 (射擊間隔)")]
    public float shootingInterval = 2f;

    [Header("子彈設定")]
    [Tooltip("子彈的預製件 (Prefab)")]
    public GameObject bulletPrefab; // 子彈物件的預製件
    [Tooltip("子彈發射點的 Transform")]
    public Transform firePoint;     // 敵人身上子彈發射的位置
    [Tooltip("子彈的飛行速度")]
    public float bulletSpeed = 10f; // 子彈的速度
    [Tooltip("目標位置")]
    public Transform targetPosition;

    [Header("射擊音效")]
    public AudioSource[] shootingAudio;

    // 私有變數
    private Rigidbody2D rb;
    private Transform targetPoint; // 當前目標點
    private int targetIndex; // 當前目標點的索引
    private bool isMoving = true; // 是否正在移動

    private void Awake()
    {
        // 取得 Rigidbody2D 元件
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        setWayPoints();
        
        // 檢查是否有設定 WayPoints
        if (wayPoints.Count == 0)
        {
            Debug.LogError("沒有生成點");
            return;
        }

        targetPosition = GameObject.Find("fire").transform;

        // 隨機選擇第一個目標點
        targetIndex = Random.Range(0, wayPoints.Count);
        targetPoint = wayPoints[targetIndex];

        // 假設敵人初始位置可能在 Start() 之前已設定 (例如，從生成器生成)
        // 或如果你想讓它立即開始移動，可以將 isMoving 設定為 true。

        // 使用協程控制移動啟動時機
        StartCoroutine(InitializeMovement());
    }

    private void setWayPoints(){
        var points = GameObject.FindGameObjectsWithTag("EnemyCreatePoint");
        
        for (int i=0; i < points.Length; i++){
            wayPoints.Add(points[i].transform);
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// 敵人朝著目標點移動
    /// </summary>
    public void Move()
    {
 
        // 計算朝向目標點的方向
        Vector2 direction = targetPoint.position - transform.position;
        // 正規化方向向量並乘以速度
        rb.velocity = direction.normalized * speed;

        // 檢查是否到達目標點附近
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            rb.velocity = Vector2.zero; // 停止移動
            isMoving = false; // 設定為不可移動
            StartCoroutine(WaitAndShoot());
        }
    }

    private IEnumerator InitializeMovement()
    {
        // 等待 3 秒後開始第一次移動
        yield return new WaitForSeconds(3f);
        //Debug.Log("開始移動到目標點：" + targetPoint.name);
        isMoving = true;
    }

    private IEnumerator WaitAndShoot()
    {
        //Debug.Log("到達目標點，準備射擊...");

        // 在射擊間隔時間內停止
        yield return new WaitForSeconds(shootingInterval);

        // 執行射擊邏輯
        ShootTarget();
        shootingAudio[Random.Range(0, shootingAudio.Length)].Play();

        // 重新選擇下一個目標點
        //targetIndex = (targetIndex + 1) % wayPoints.Length; // 依序移動
        targetIndex = Random.Range(0, wayPoints.Count); // 或隨機移動
        targetPoint = wayPoints[targetIndex];

        //Debug.Log("移動到下一個目標點：" + targetPoint.position);
        isMoving = true; // 重新啟動移動
    }

    private void ShootTarget()
    {
        if (bulletPrefab == null || firePoint == null || targetPosition == null)
        {
            Debug.LogError("子彈預製件、發射點或城堡目標點 (Castle Target) 未設定！");
            return;
        }

        // 1. 生成子彈
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        if (bulletRb != null)
        {
            // 2. 計算射擊方向：從發射點指向 castleTarget.position
            Vector3 directionToTarget = targetPosition.position - firePoint.position;

            // 3. 施加速度
            bulletRb.velocity = directionToTarget.normalized * bulletSpeed;

            // 4. (可選) 旋轉子彈使其朝向飛行方向
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
        else
        {
            Debug.LogError("子彈預製件上缺少 Rigidbody2D 元件！");
        }
    }
}

