using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI 组件")]
    public Slider progressBar;
    //public TextMeshProUGUI progressText;
    //public TextMeshProUGUI loadingTipText;

    [Header("加载设置")]
    public string gameSceneName = "Song1";
    public float minLoadTime = 1f;  // 最小加载时间（避免一闪而过）

    void Start()
    {
        // 开始异步加载游戏场景
        StartCoroutine(LoadGameSceneAsync());
    }

    IEnumerator LoadGameSceneAsync()
    {
        // 记录开始时间
        float startTime = Time.time;

        // 开始异步加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false;  // 禁止自动激活

        // 等待加载完成
        while (!asyncLoad.isDone)
        {
            // 获取加载进度 (0-0.9)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // 更新进度条
            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            // 更新进度文字
            // if (progressText != null)
            // {
            //     progressText.text = $"加载中... {Mathf.RoundToInt(progress * 100)}%";
            // }

            // // 更新提示文字（根据进度变化）
            // if (loadingTipText != null)
            // {
            //     if (progress < 0.3f)
            //         loadingTipText.text = "正在加载资源...";
            //     else if (progress < 0.6f)
            //         loadingTipText.text = "正在初始化场景...";
            //     else
            //         loadingTipText.text = "即将进入游戏...";
            // }

            // // 检查是否加载完成且达到最小加载时间
            // if (asyncLoad.progress >= 0.9f && Time.time - startTime >= minLoadTime)
            // {
            //     // 显示完成文字
            //     if (progressText != null)
            //     {
            //         progressText.text = "加载完成！";
            //     }

            // 等待 0.5 秒后进入游戏
            yield return new WaitForSeconds(0.5f);

            // 激活场景
            asyncLoad.allowSceneActivation = true;
            yield break;
        }

        yield return null;
    }
}
