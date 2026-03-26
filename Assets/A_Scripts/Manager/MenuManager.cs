using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI 组件")]
    public Button startButton;

    void Start()
    {
        // 绑定按钮点击事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClick);
        }
    }

    void OnStartButtonClick()
    {
        Debug.Log("开始游戏！加载场景...");
        // 跳转到加载场景
        SceneManager.LoadScene("LoadingScene");
    }
}