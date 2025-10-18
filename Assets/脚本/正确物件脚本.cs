using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class CorrectObject : MonoBehaviour
{
    [Header("謎題完成後設定")]
    public DialogueData completionDialogue;
    public string sceneToReturnTo;
    public string entryPointInReturnScene;
    public GameObject eraserPuzzlePanel; // 【新增】對擦除謎題UI面板的引用
    public Sprite completedSprite;       // 【新增】謎題完成後物件要變成的圖案

    private bool hasBeenActivated = false;
    private SpriteRenderer spriteRenderer; // 【新增】用於改變物件本身的圖案

    private void Awake()
    {
        // 【新增】在遊戲開始時獲取自身的 SpriteRenderer 元件
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 在物件啟用時，訂閱「擦除完成」事件
    private void OnEnable()
    {
        EraserController.OnEraseCompleted += HandleEraseCompleted;
    }

    // 在物件禁用時，取消訂閱，防止記憶體洩漏
    private void OnDisable()
    {
        EraserController.OnEraseCompleted -= HandleEraseCompleted;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (hasBeenActivated) return;
        
        if (ToolbarManager.instance == null)
        {
            Debug.LogError("場景中找不到 ToolbarManager！");
            return;
        }
        
        hasBeenActivated = true;
        ToolbarManager.instance.ShowToolbar();
    }

    // 當接收到「擦除完成」事件時，要執行的函式
    private void HandleEraseCompleted()
    {
        // 為了避免重複執行，可以立即取消訂閱
        EraserController.OnEraseCompleted -= HandleEraseCompleted;
        
        // 找到並禁用 EraserController，防止玩家繼續擦除
        EraserController controller = FindObjectOfType<EraserController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // 【修改】隱藏擦除謎題的UI面板
        if (eraserPuzzlePanel != null)
        {
            eraserPuzzlePanel.SetActive(false);
        }

        // 【修改】改變物件本身的圖案
        if (spriteRenderer != null && completedSprite != null)
        {
            spriteRenderer.sprite = completedSprite;
            Debug.Log("物件圖案已更新！");
        }

        // 訂閱「對話結束」事件，以便在對話結束後執行返回操作
        DialogueManager.OnDialogueEnd += ReturnToSceneAfterDialogue;

        // 開始播放完成對話
        if (completionDialogue != null && DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(completionDialogue);
        }
        else
        {
            Debug.LogWarning("未設定完成對話，將直接返回場景。");
            ReturnToSceneAfterDialogue();
        }
    }

    // 對話結束後要呼叫的函式
    private void ReturnToSceneAfterDialogue()
    {
        // 【重要】執行完後立刻取消訂閱，避免影響遊戲中其他的對話
        DialogueManager.OnDialogueEnd -= ReturnToSceneAfterDialogue;

        // 準備並跳轉回初始場景
        Debug.Log("對話結束，返回場景: " + sceneToReturnTo);
        if (GameEventManager.instance != null)
        {
            GameEventManager.nextEntryPointName = entryPointInReturnScene;
        }
        SceneManager.LoadScene(sceneToReturnTo);
    }
}

