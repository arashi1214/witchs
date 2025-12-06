using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damageAmount;
    public Transform targetTransform;
    private HealthBar healthbar;

    private void Awake()
    {
        GameObject HP = GameObject.FindWithTag("Target");
        healthbar = HP.GetComponent<HealthBar>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Target"))
        {
            healthbar.TakeDamage(damageAmount);
        }

        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail != null)
        {
            // 設定 Trail Renderer 在尾巴消失後自動銷毀整個 GameObject
            trail.autodestruct = true;
        }

        // 2. 禁用子彈的可視性和碰撞，讓它假裝已經消失
        // 如果有 SpriteRenderer:
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // 禁用碰撞體，避免繼續影響遊戲
        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null) coll.enabled = false;

        // 3. (重要) 如果沒有 Trail Renderer，則直接銷毀
        if (trail == null)
        {
            Destroy(gameObject);
        }
    }
}
