using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class CorrectObject : MonoBehaviour
{
    [Header("謎題完成後設定")]
    public GameObject eraserPuzzlePanel;
    public Sprite completedSprite;

    [Header("Fungus 設定")]
    public Fungus.Flowchart targetFlowchart;
    public string puzzleCompleteBlockName;

    [Header("返回場景設定")]
    public string sceneToReturnTo;
    public string entryPointInReturnScene;

    private bool hasBeenActivated = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // OnMouseDown 的邏輯保持不變，只是觸發的目標變了
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (hasBeenActivated) return;
        
        if (ToolbarManager.instance != null)
        {
            hasBeenActivated = true;
            ToolbarManager.instance.ShowToolbar();
        }
    }

    // 這個函式由 EraserController 的完成事件觸發
    private void OnEnable() { EraserController.OnEraseCompleted += HandleEraseCompleted; }
    private void OnDisable() { EraserController.OnEraseCompleted -= HandleEraseCompleted; }

    private void HandleEraseCompleted()
    {
        // 1. 處理 UI 和物件外觀
        if (eraserPuzzlePanel != null) eraserPuzzlePanel.SetActive(false);
        if (spriteRenderer != null && completedSprite != null) spriteRenderer.sprite = completedSprite;
        
        // 2. 觸發 Fungus 對話
        if (targetFlowchart != null && !string.IsNullOrEmpty(puzzleCompleteBlockName))
        {
            targetFlowchart.ExecuteBlock(puzzleCompleteBlockName);
        }
    }

    // 【新增】這個函式將由 Fungus 的 Call Method 指令來呼叫
    public void ReturnToPreviousScene()
    {
        // 【偵錯日誌】檢查此函式是否被成功呼叫
        Debug.Log("<color=green>Fungus 成功呼叫 ReturnToPreviousScene()！</color> 準備返回場景...");
        StartCoroutine(ReturnToSceneCoroutine());
    }

    private IEnumerator ReturnToSceneCoroutine()
    {
        // 檢查場景名稱是否為空
        if (string.IsNullOrEmpty(sceneToReturnTo))
        {
            Debug.LogError("錯誤：要返回的場景名稱為空！請在 Inspector 中設定。");
            yield break; // 終止協程
        }

        yield return new WaitForSeconds(0.2f);
        if (GameEventManager.instance != null)
        {
            GameEventManager.nextEntryPointName = entryPointInReturnScene;
        }
        SceneManager.LoadScene(sceneToReturnTo);
    }
}
