using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LaunchingAudioManager : MonoBehaviour
{
    // 讓其他腳本可以輕鬆存取這個實例
    public static LaunchingAudioManager Instance { get; private set; }

    // 播放音頻的來源陣列
    public AudioSource[] launchingAudio;
    // private Witchs witches; // 如果不需要，可以移除或保留

    // 1. 定義公共事件（作為訂閱點）
    // 外部腳本將訂閱這個事件來觸發音頻播放
    public event Action OnPlayLaunchAudio;


    void Awake()
    {
        // 確保只有一個實例存在 (單例模式)
        if (Instance == null)
        {
            Instance = this;
            // 如果希望它在場景切換時不被銷毀，可以加上 DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 2. 在 Awake 時，讓自己的事件訂閱自己的播放方法
        // 這樣外部腳本只需要觸發 OnPlayLaunchAudio 事件即可
        OnPlayLaunchAudio += PlayAudio;
    }

    void OnDestroy()
    {
        // 3. 在銷毀時取消訂閱，避免空引用錯誤
        OnPlayLaunchAudio -= PlayAudio;
    }

    /// <summary>
    /// 從陣列中隨機播放一個音源
    /// </summary>
    void PlayAudio()
    {
        if (launchingAudio != null && launchingAudio.Length > 0)
        {
            launchingAudio[UnityEngine.Random.Range(0, launchingAudio.Length)].Play();
            Debug.Log("音頻控制器：隨機音頻已播放。");
        }
        else
        {
            Debug.LogWarning("AudioController 的 launchingAudio 陣列為空！");
        }
    }

    /// <summary>
    /// 【供外部腳本呼叫】觸發音頻播放事件的方法
    /// </summary>
    public void TriggerLaunchAudioSignal()
    {
        // 使用問號 (?. ) 確保只有在有訂閱者時才執行 Invoke
        OnPlayLaunchAudio?.Invoke();
    }
}
