using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class CorrectObject : MonoBehaviour
{
    [Header("謎題完成後設定")]
    public DialogueData completionDialogue;    // 謎題完成後觸發的對話
    public GameObject eraserPuzzlePanel;       // 橡皮擦 UI 面板
    public Sprite completedSprite;             // 完成後物件要顯示的新圖案

    [Header("返回場景設定")]
    public string sceneToReturnTo;           // 要返回的場景名稱
    public string entryPointInReturnScene;     // 返回場景後的入口點名稱

    private bool hasBeenActivated = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // 在 Awake 中獲取 SpriteRenderer 元件
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        EraserController.OnEraseCompleted += HandleEraseCompleted;
    }

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

    private void HandleEraseCompleted()
    {
        EraserController.OnEraseCompleted -= HandleEraseCompleted;
        
        // 1. 隱藏 UI, 更換圖案
        if (eraserPuzzlePanel != null)
        {
            eraserPuzzlePanel.SetActive(false);
        }
        if (spriteRenderer != null && completedSprite != null)
        {
            spriteRenderer.sprite = completedSprite;
        }

        // 2. 訂閱對話結束事件，準備跳轉
        DialogueManager.OnDialogueEnd += PrepareToReturnToScene;
        
        // 3. 觸發對話
        if (completionDialogue != null && DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(completionDialogue);
        }
        else
        {
            // 如果沒有對話，也直接準備跳轉
            PrepareToReturnToScene();
        }
    }

    // 【修改】這個函式現在只負責啟動協程
    private void PrepareToReturnToScene()
    {
        DialogueManager.OnDialogueEnd -= PrepareToReturnToScene;
        // 啟動一個帶有延遲的場景跳轉協程
        StartCoroutine(ReturnToSceneCoroutine());
    }

    // 【新增】帶有延遲的場景跳轉協程
    private IEnumerator ReturnToSceneCoroutine()
    {
        // 等待 0.2 秒。這個緩衝時間給了編輯器足夠的時間來穩定
        yield return new WaitForSeconds(0.2f);

        Debug.Log("安全緩衝結束，正在返回場景: " + sceneToReturnTo);
        if (GameEventManager.instance != null)
        {
            GameEventManager.nextEntryPointName = entryPointInReturnScene;
        }
        SceneManager.LoadScene(sceneToReturnTo);
    }
}

