using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlasher : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine; // 用于存储当前正在运行的协程

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 关键：存储启动时的任意原始颜色
        originalColor = spriteRenderer.color;
    }

    // 停止任何正在运行的闪烁协程，并恢复原色
    private void StopCurrentFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        // 确保恢复原始颜色
        spriteRenderer.color = originalColor;
    }

    /// <summary>
    /// (新) Fungus 调用：开始呼吸式闪烁 (Non-HDR 版本)
    /// </summary>
    /// <param name="speed">呼吸速度 (例如 2.0)</param>
    /// <param name="pulseIntensity">脉冲强度 (0.0 到 1.0)。0=无效果, 1=呼吸到全黑。</param>
    public void StartBreathe(float speed, float pulseIntensity)
    {
        // 确保在开始新协程前停止旧的
        StopCurrentFlash();
        // 启动呼吸协程
        flashCoroutine = StartCoroutine(BreatheCoroutine(speed, pulseIntensity));
    }

    private IEnumerator BreatheCoroutine(float speed, float pulseIntensity)
    {
        // 1. 设置强度
        // 确保强度在 0-1 范围，1.0 代表原始亮度
        float maxIntensity = 1.0f; 
        
        // 我们将旧的 'maxIntensity' 参数重新解释为脉冲强度
        // 并将其限制在 0-1 之间
        float clampedPulse = Mathf.Clamp01(pulseIntensity);
        
        // 0.0=呼吸到100%亮度 (无效果)
        // 1.0=呼吸到 0% 亮度 (全黑)
        float minIntensity = 1.0f - clampedPulse; 
        
        // 存储原始的 Alpha 值，我们只修改 RGB
        float originalAlpha = originalColor.a;

        while (true)
        {
            // 2. 计算 Sine 波 (在 0 和 1 之间平滑振荡)
            float normalizedWave = (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f; 

            // 3. 计算当前亮度
            // 根据波形，在 minIntensity 和 maxIntensity 之间平滑插值
            // 例如：在 0.5 和 1.0 之间来回变化
            float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedWave);

            // 4. 应用 Non-HDR 颜色
            // 将原始颜色的 R,G,B 分量乘以计算出的亮度
            spriteRenderer.color = new Color(
                originalColor.r * currentIntensity,
                originalColor.g * currentIntensity,
                originalColor.b * currentIntensity,
                originalAlpha // 始终保持原始的 Alpha (透明度) 不变
            );

            yield return null; // 等待下一帧, 使循环平滑
        }
    }

    /// <summary>
    /// (新) Fungus 调用：停止所有闪烁并恢复原色
    /// </summary>
    public void StopFlash()
    {
        StopCurrentFlash();
    }
    
    // --- (保留了旧的闪烁方法，它们仍然可以工作) ---
    // ... (StartWhiteBlink, StartWhiteLoop, etc.) ...
    
    public void StartWhiteBlink(int count)
    {
        StartFlash(Color.white, 0.1f, count);
    }
    
    public void StartWhiteLoop(float duration)
    {
        StopCurrentFlash();
        flashCoroutine = StartCoroutine(ContinuousFlashCoroutine(Color.white, duration));
    }
    
    public void StartFlash(Color flashColor, float flashDuration, int flashCount)
    {
        StopCurrentFlash();
        flashCoroutine = StartCoroutine(FlashCoroutine(flashColor, flashDuration, flashCount));
    }

    private IEnumerator FlashCoroutine(Color flashColor, float flashDuration, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            // 注意：这个旧方法会覆盖Alpha。
            // 如果你需要保持alpha，也需要修改这里。
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private IEnumerator ContinuousFlashCoroutine(Color flashColor, float flashDuration)
    {
        while (true)
        {
            // 同样，这个旧方法会覆盖Alpha
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
    }
}