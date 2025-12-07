using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BlinkButtonText : MonoBehaviour
{
    // 拖曳 Button 上的 Text 元件到 Inspector
    public TMP_Text targetText;

    // 控制閃爍的速度，數值越大，過渡越快
    public float blinkSpeed = 2f;

    // 儲存原始顏色 (完全不透明)
    private Color originalColor;

    // 目標透明顏色 (完全透明)
    private Color transparentColor;

    private bool isFadingOut = true;

    void Start()
    {
        // 嘗試從 Button 的子物件中獲取 TMP_Text 元件
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TMP_Text>();
        }

        if (targetText != null)
        {
            // 設定原始顏色和透明顏色
            originalColor = targetText.color;
            transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // 僅 Alpha 為 0

            // 啟動平滑閃爍協程
            StartCoroutine(SmoothBlinkEffect());
        }
        else
        {
            Debug.LogError("SmoothBlinkingText 腳本找不到 TextMeshPro 元件。");
        }
    }

    IEnumerator SmoothBlinkEffect()
    {
        while (true)
        {
            float t = 0; // 設置插值時間的起始值

            // 決定目標顏色
            Color startColor = isFadingOut ? originalColor : transparentColor;
            Color endColor = isFadingOut ? transparentColor : originalColor;

            // 在兩顏色之間平滑插值
            while (t < 1)
            {
                // 根據時間增加 t 的值，乘 Time.deltaTime 確保過渡速度與幀率無關
                t += Time.deltaTime * blinkSpeed;

                // 使用 Color.Lerp 在起始色和結束色之間平滑過渡
                targetText.color = Color.Lerp(startColor, endColor, t);

                yield return null; // 等待下一幀
            }

            // 插值完成後，確保顏色精確設定為目標顏色
            targetText.color = endColor;

            // 反轉狀態，準備進行下一次淡入或淡出
            isFadingOut = !isFadingOut;

            // 在轉換方向前可以加入一個短暫的暫停 (可選)
            // yield return new WaitForSeconds(0.1f);
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
