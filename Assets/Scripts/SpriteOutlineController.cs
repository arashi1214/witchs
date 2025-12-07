using UnityEngine;
using UnityEngine.UI; // 引入 UI 命名空間

// 確保遊戲物件上必須有 Image 元件
[RequireComponent(typeof(Image))]
public class SpriteOutlineController : MonoBehaviour
{
    // 將 SpriteRenderer 替換為 Image 
    private Image uiImage;
    private Material outlineMaterial;

    [Header("輪廓設定")]
    [Tooltip("輪廓顏色")]
    public Color outlineColor = Color.yellow;

    [Tooltip("輪廓寬度 (0.0 - 1.0)")]
    [Range(0.0f, 1.0f)]
    public float outlineWidth = 0.05f;

    // Shader 中對應的屬性名稱 (與前一個腳本相同)
    private readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
    private readonly int OutlineWidthID = Shader.PropertyToID("_OutlineWidth");


    void Awake()
    {
        uiImage = GetComponent<Image>();

        // 獲取 Image 元件正在使用的材質實例。
        // **【重要差異】** // 1. 確保 Image 已經有一個包含輪廓功能的 Material 賦予給它。
        // 2. 為了避免修改共享材質，我們使用 Instantiate 複製一份。
        if (uiImage.material != null)
        {
            outlineMaterial = Instantiate(uiImage.material);
            uiImage.material = outlineMaterial; // 將複製的材質賦予給 Image
        }

        // 初始化輪廓設定
        UpdateOutlineProperties();
    }

    void OnValidate()
    {
        // 確保在 Inspector 中調整參數時，效果立即更新
        if (Application.isPlaying && outlineMaterial != null)
        {
            UpdateOutlineProperties();
        }
    }

    /// <summary>
    /// 更新 Shader 中的輪廓顏色和寬度屬性。
    /// </summary>
    private void UpdateOutlineProperties()
    {
        if (outlineMaterial == null)
        {
            // 如果材質仍為空 (可能未賦予自定義材質)，則返回
            Debug.LogError("Image 元件上沒有自定義材質可以控制輪廓！");
            return;
        }

        // 將 C# 腳本中的公共變數值傳遞給 Shader
        outlineMaterial.SetColor(OutlineColorID, outlineColor);
        outlineMaterial.SetFloat(OutlineWidthID, outlineWidth);
    }

    /// <summary>
    /// 啟用或禁用輪廓發光效果。
    /// </summary>
    public void SetOutlineActive(bool isActive)
    {
        if (outlineMaterial != null)
        {
            // 禁用輪廓時，將寬度設為 0
            outlineMaterial.SetFloat(OutlineWidthID, isActive ? outlineWidth : 0f);
        }
    }
}