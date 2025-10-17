using UnityEngine;
using UnityEngine.SceneManagement; // 必須引用場景管理命名空間

public class ScenePortal : MonoBehaviour
{
    [Header("傳送設定")]
    public string sceneToLoad; // 要載入的目標場景名稱
    // 在 public string sceneToLoad; 下方添加這一行
    public string entryPointName; // 指定玩家在目標場景中出現的入口點名稱

    [Header("條件設定")]
    public bool requiresCondition = false; // 是否需要滿足條件才能傳送
    public string[] requiredFlags; // 需要滿足的旗標列表
    public DialogueData lockedDialogue; // 條件不滿足時顯示的對話

    private bool isPlayerInRange = false;

    void Update()
    {
        // 檢查玩家是否在範圍內且按下了互動鍵
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        // 檢查是否需要條件
        if (requiresCondition)
        {
            // 如果需要條件，檢查所有旗標是否都已設定
            if (GameEventManager.instance.AreAllFlagsSet(requiredFlags))
            {
                // 條件滿足，進行傳送
                LoadTargetScene();
            }
            else
            {
                // 條件不滿足，顯示鎖定對話
                if (lockedDialogue != null)
                {
                    DialogueManager.instance.StartDialogue(lockedDialogue);
                }
            }
        }
        else
        {
            // 如果不需要條件，直接傳送
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        Debug.Log("正在傳送到場景: " + sceneToLoad);
        // 在載入場景之前，先記錄下目標入口點的名稱
        GameEventManager.nextEntryPointName = this.entryPointName;

        // 【新增偵錯日誌】
        Debug.Log("<color=orange>[傳送門]</color> 準備出發！設定下一個入口點為: " + GameEventManager.nextEntryPointName);
        SceneManager.LoadScene(sceneToLoad);
    }

    // 當有物件進入觸發器時
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查進入的是否是玩家
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 可以在這裡顯示一個 "按 E 互動" 的提示 UI
            Debug.Log("玩家進入傳送範圍");
        }
    }

    // 當有物件離開觸發器時
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 可以在這裡隱藏提示 UI
            Debug.Log("玩家離開傳送範圍");
        }
    }
}