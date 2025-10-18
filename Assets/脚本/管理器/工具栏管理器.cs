using System.Collections;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    public static ToolbarManager instance;

    [Header("UI 設定")]
    public RectTransform toolbarPanel;
    public float animationSpeed = 5f;

    [Header("謎題設定")]
    public string correctToolName;
    public GameObject eraserPuzzlePanel;

    private Vector2 onScreenPosition;
    private Vector2 offScreenPosition;
    private Coroutine activeAnimationCoroutine; // 【修改】用於儲存正在運行的動畫協程

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 更穩健地計算螢幕內外的位置
        onScreenPosition = new Vector2(toolbarPanel.anchoredPosition.x, 0);
        offScreenPosition = new Vector2(toolbarPanel.anchoredPosition.x, -toolbarPanel.rect.height);
        
        // 確保初始位置在螢幕外
        toolbarPanel.anchoredPosition = offScreenPosition;
    }

    public void ShowToolbar()
    {
        // 【修改】在開始新動畫前，先停止任何正在運行的舊動畫
        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
        }
        activeAnimationCoroutine = StartCoroutine(AnimateToolbar(onScreenPosition));
    }

    public void HideToolbar()
    {
        // 【修改】在開始新動畫前，先停止任何正在運行的舊動畫
        if (activeAnimationCoroutine != null)
        {
            StopCoroutine(activeAnimationCoroutine);
        }
        activeAnimationCoroutine = StartCoroutine(AnimateToolbar(offScreenPosition));
    }

    private IEnumerator AnimateToolbar(Vector2 targetPosition)
    {
        while (Vector2.Distance(toolbarPanel.anchoredPosition, targetPosition) > 0.1f)
        {
            toolbarPanel.anchoredPosition = Vector2.Lerp(toolbarPanel.anchoredPosition, targetPosition, animationSpeed * Time.deltaTime);
            yield return null;
        }
        toolbarPanel.anchoredPosition = targetPosition;
        activeAnimationCoroutine = null; // 動畫結束後，清除引用
    }
    
    // 這個函式將由工具按鈕呼叫
    public void OnToolSelected(string toolName)
    {
        Debug.Log("選擇了工具: " + toolName);

        if (toolName == correctToolName)
        {
            Debug.Log("選擇了正確的工具！");
            HideToolbar();

            if (eraserPuzzlePanel != null)
            {
                eraserPuzzlePanel.SetActive(true);  
            }
        }
        else
        {
            Debug.Log("選擇了錯誤的工具。");
            // 可以在這裡給出錯誤提示，例如播放一個音效
        }
    }
}
