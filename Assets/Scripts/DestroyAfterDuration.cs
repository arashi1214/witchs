using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 獲取附加在這個物件上的 ParticleSystem 元件
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // 判斷粒子是否已經設置了播放完自動停止 (Stop Action)
        if (ps != null)
        {
            // 等待粒子特效的總播放時長後銷毀自身
            Destroy(gameObject, ps.main.duration + 1f);
        }
        else
        {
            // 如果沒有 ParticleSystem 元件，則立即銷毀自身（防止錯誤）
            Destroy(gameObject);
        }
    }
}
