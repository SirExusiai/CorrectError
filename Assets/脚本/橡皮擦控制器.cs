using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RawImage))]
public class EraserController : MonoBehaviour
{
    [Header("核心設定")]
    public RenderTexture maskRenderTexture;
    public Texture brushTexture;
    public float brushSize = 50f;

    [Header("完成度設定")]
    [Range(0.1f, 1f)]
    public float completionThreshold = 0.7f; // 完成度閾值 (例如 70%)
    public int gridResolution = 32; // 檢查網格的精細度，32x32 網格已足夠

    public static event Action OnEraseCompleted;

    private Camera uiCamera;
    private Canvas parentCanvas;
    private Vector2? lastMousePosition = null;
    private bool isCompleted = false;

    // --- 全新的、基於網格的完成度檢查變數 ---
    private bool[,] completionGrid;
    private int totalCells;
    private int completedCells;
    // ---------------------------------------------

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) { uiCamera = null; }
        else { uiCamera = parentCanvas.worldCamera; }
        
        // 初始化網格
        completionGrid = new bool[gridResolution, gridResolution];
        totalCells = gridResolution * gridResolution;
        completedCells = 0;

        ClearRenderTexture();
    }

    void Update()
    {
        if (isCompleted) return;

        Vector2 localPoint;
        bool isPointerInRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);

        if (Input.GetMouseButtonDown(0))
        {
            if (isPointerInRect) { lastMousePosition = localPoint; }
        }
        else if (Input.GetMouseButton(0))
        {
            if (isPointerInRect)
            {
                if (lastMousePosition != null) { DrawLine(lastMousePosition.Value, localPoint); }
                lastMousePosition = localPoint;
            }
            else { lastMousePosition = null; }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = null;
            // 【修改】不再使用協程，直接呼叫新的檢查函式
            CheckCompletion_GridBased();
        }
    }

    private void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.Max(1, (int)(distance / (brushSize * 0.25f))); 
        for (int i = 0; i <= steps; i++) // 使用 <= 確保終點也被繪製
        {
            Vector2 interpolatedPoint = Vector2.Lerp(from, to, (float)i / steps);
            Draw(interpolatedPoint);
        }
    }

    private void Draw(Vector2 localPoint)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 uv = new Vector2((localPoint.x + rt.rect.width / 2) / rt.rect.width,
                                 (localPoint.y + rt.rect.height / 2) / rt.rect.height);
        
        // 【新增】在繪製的同時，更新我們的 CPU 網格
        UpdateCompletionGrid(uv);

        // 視覺上的繪製邏輯保持不變
        RenderTexture.active = maskRenderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskRenderTexture.width, 0, maskRenderTexture.height);
        Rect brushRect = new Rect(uv.x * maskRenderTexture.width - brushSize / 2,
                                  uv.y * maskRenderTexture.height - brushSize / 2,
                                  brushSize,
                                  brushSize);
        Graphics.DrawTexture(brushRect, brushTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }
    
    // 【新增】更新 CPU 網格的函式
    private void UpdateCompletionGrid(Vector2 uv)
    {
        int gridX = Mathf.Clamp(Mathf.FloorToInt(uv.x * gridResolution), 0, gridResolution - 1);
        int gridY = Mathf.Clamp(Mathf.FloorToInt(uv.y * gridResolution), 0, gridResolution - 1);

        if (!completionGrid[gridX, gridY])
        {
            completionGrid[gridX, gridY] = true;
            completedCells++;
        }
    }
    
    // 【新增】全新的、基於網格的完成度檢查函式
    private void CheckCompletion_GridBased()
    {
        if (totalCells == 0) return; // 防止除以零
        float completion = (float)completedCells / totalCells;
        Debug.Log("當前擦除完成度 (基於網格): " + completion.ToString("P1"));

        if (completion >= completionThreshold && !isCompleted)
        {
            isCompleted = true;
            Debug.Log("謎題完成！觸發 OnEraseCompleted 事件。");
            OnEraseCompleted?.Invoke();
        }
    }

    public void ClearRenderTexture()
    {
        RenderTexture.active = maskRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
        
        // 同時也重設我們的 CPU 網格
        if(completionGrid != null)
        {
            Array.Clear(completionGrid, 0, completionGrid.Length);
        }
        completedCells = 0;
    }
}

