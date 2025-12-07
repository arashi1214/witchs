using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropMoving : MonoBehaviour
{
    public float speed = 5;
    public float distance = 2;
    private float StartPoint;
    private float EndPoint;
    private SpriteRenderer spriteRenderer;

    [Header("擊中震動設定")]
    public float shakeDuration = 0.1f;    // 震動總時間
    public float shakeMagnitude = 0.1f;   // 震動強度 (位移最大距離)
    public float dampening = 10f;         // 震動衰減速度

    private Vector3 initialPosition;      // 角色初始位置
    private bool isShaking = false;

    public AudioSource[] popeAudios;


    // Start is called before the first frame update
    void Start()
    {
        StartPoint = transform.position.x;
        EndPoint = transform.position.x - distance;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = !spriteRenderer.flipX;

        initialPosition = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (transform.position.x >= StartPoint || transform.position.x <= EndPoint)
        {
            speed = speed * -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        float moveAmount = speed * Time.deltaTime;

        Vector3 currentPosition = transform.position;
        currentPosition.x += moveAmount;
        transform.position = currentPosition;
    }

    public void TakeHit()
    {
        if (!isShaking)
        {
            // 可以在這裡處理血量減少、音效等
            StartCoroutine(HitShakeCoroutine());

            popeAudios[Random.Range(0, popeAudios.Length)].Play();
        }
    }

    private IEnumerator HitShakeCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        // 儲存原始位置 (相對於父物件的位置)
        Vector3 originalPos = transform.localPosition;

        while (elapsed < shakeDuration)
        {
            // 計算一個隨機的偏移量
            // Random.insideUnitCircle 會返回圓心在 (0,0)、半徑為 1 的圓內的一個隨機點
            Vector2 randomOffset = Random.insideUnitCircle * shakeMagnitude;

            // 應用偏移，使用原始位置加上隨機偏移
            transform.localPosition = originalPos + (Vector3)randomOffset;

            // 讓震動強度隨著時間衰減 (可選)
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, dampening * Time.deltaTime);

            elapsed += Time.deltaTime;
            yield return null; // 等待下一幀
        }

        // 震動結束，確保角色回到原始位置
        transform.localPosition = originalPos;
        isShaking = false;
    }
}
