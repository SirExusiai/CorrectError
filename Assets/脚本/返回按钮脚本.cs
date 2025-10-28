using UnityEngine;
using UnityEngine.SceneManagement; // 必须添加，用于场景管理

public class PersistentGlobalUI : MonoBehaviour
{
    [Header("要控制的UI元素")]
    [Tooltip("需要根据场景隐藏/显示的按钮")]
    public GameObject targetButton; // 我们将把 Button_Quit 拖到这里

    [Header("隐藏设置")]
    [Tooltip("在此列表中的场景，按钮将被隐藏")]
    public string[] scenesToHideIn; // 在 Inspector 中填入场景名字

    // 用于实现“贯穿全局”的单例
    public static PersistentGlobalUI Instance;

    void Awake()
    {
        // 1. 实现“贯穿全局” (Singleton 模式)
        if (Instance == null)
        {
            // 如果我是第一个实例，就将我设为全局实例
            Instance = this;
            // 并且跨场景时不要销毁我
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果全局实例已经存在 (说明我是从新场景加载的重复体)，
            // 立即销毁我自己。
            Destroy(gameObject);
            return;
        }

        // 2. 订阅“场景已加载”事件
        // 当一个新场景加载完成时，会调用 OnSceneLoaded 方法
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 记得在销毁时取消订阅，避免内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 当一个新场景被加载时，此方法会被自动调用
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 确保我们有关闭按钮的引用
        if (targetButton == null)
        {
            return;
        }

        // 默认先显示按钮
        bool shouldHide = false;

        // 遍历我们的“隐藏列表”
        foreach (string sceneName in scenesToHideIn)
        {
            if (scene.name == sceneName)
            {
                // 如果当前场景的名字在列表里，就设置“应该隐藏”
                shouldHide = true;
                break; // 找到一个匹配项就足够了
            }
        }

        // 3. 根据检查结果，显示或隐藏按钮
        targetButton.SetActive(!shouldHide);
    }
}