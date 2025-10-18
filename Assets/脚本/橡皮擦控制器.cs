using UnityEngine;
using UnityEngine.UI;
using System; // 【新增】為了使用 Action 事件
using System.Collections; // 【新增】為了使用協程

[RequireComponent(typeof(RawImage))]
public class EraserController : MonoBehaviour
{
    public RenderTexture maskRenderTexture;
    public Texture brushTexture;
    public float brushSize = 50f;

    [Header("完成度設定")]
    [Range(0.1f, 1f)]
    public float completionThreshold = 0.8f; // 完成度閾值 (例如 80%)

    // 【新增】當擦除完成時觸發的靜態事件
    public static event Action OnEraseCompleted;

    private Camera uiCamera;
    private Canvas parentCanvas;
    private Vector2? lastMousePosition = null;
    private bool isCompleted = false; // 防止重複觸發完成事件

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = null;
        }
        else
        {
            uiCamera = parentCanvas.worldCamera;
        }
        
        ClearRenderTexture();
    }

    void Update()
    {
        // 如果謎題已完成，則不執行任何操作
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
                if (lastMousePosition != null)
                {
                    DrawLine(lastMousePosition.Value, localPoint);
                }
                lastMousePosition = localPoint;
            }
            else { lastMousePosition = null; }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = null;
            // 【新增】當滑鼠抬起時，檢查完成度
            StartCoroutine(CheckCompletion());
        }
    }

    // --- (DrawLine, Draw, ClearRenderTexture 函式保持不變) ---
    private void DrawLine(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        int steps = Mathf.Max(1, (int)(distance / (brushSize * 0.25f))); 
        for (int i = 0; i < steps; i++)
        {
            Vector2 interpolatedPoint = Vector2.Lerp(from, to, (float)i / steps);
            Draw(interpolatedPoint);
        }
        Draw(to);
    }
    private void Draw(Vector2 localPoint)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 uv = new Vector2((localPoint.x + rt.rect.width / 2) / rt.rect.width, (localPoint.y + rt.rect.height / 2) / rt.rect.height);
        RenderTexture.active = maskRenderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskRenderTexture.width, 0, maskRenderTexture.height);
        Rect brushRect = new Rect(uv.x * maskRenderTexture.width - brushSize / 2, uv.y * maskRenderTexture.height - brushSize / 2, brushSize, brushSize);
        Graphics.DrawTexture(brushRect, brushTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }
    public void ClearRenderTexture()
    {
        RenderTexture.active = maskRenderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }
    // --------------------------------------------------------

    // 【新增】檢查完成度的協程
    private IEnumerator CheckCompletion()
    {
        // 等待一幀，確保繪製操作已提交到 GPU
        yield return new WaitForEndOfFrame();

        Texture2D tex = new Texture2D(maskRenderTexture.width, maskRenderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = maskRenderTexture;
        tex.ReadPixels(new Rect(0, 0, maskRenderTexture.width, maskRenderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        Color[] pixels = tex.GetPixels();
        int erasedCount = 0;
        for (int i = 0; i < pixels.Length; i++)
        {
            // 我們在遮罩上畫的是不透明的白色，所以檢查 Alpha > 0.1
            if (pixels[i].a > 0.1f)
            {
                erasedCount++;
            }
        }

        // 釋放臨時 Texture2D 的記憶體
        Destroy(tex);

        float completion = (float)erasedCount / pixels.Length;
        Debug.Log("當前擦除完成度: " + completion.ToString("P1"));

        if (completion >= completionThreshold)
        {
            isCompleted = true;
            Debug.Log("謎題完成！觸發 OnEraseCompleted 事件。");
            OnEraseCompleted?.Invoke();
        }
    }
}

