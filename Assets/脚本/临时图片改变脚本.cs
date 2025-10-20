using UnityEngine;
using System.Collections;

/// <summary>
/// 這個腳本提供了多個可以被 Fungus 呼叫的函式，
/// 用於臨時或持續地改變物件的 Sprite，並管理 Animator 和移動狀態。
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class TemporarySpriteChanger : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("要顯示的 Sprite 圖片（臨時或持續狀態共用）")]
    public Sprite temporarySprite; // 您可以在 Inspector 中預先設定這張圖片

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerMovement playerMovement; // 新增 PlayerMovement 的引用
    private Coroutine runningCoroutine;

    void Awake()
    {
        // 獲取自身的 SpriteRenderer 元件
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 嘗試獲取 Animator 元件（如果有的話）
        animator = GetComponent<Animator>();
        // 嘗試獲取 PlayerMovement 元件（如果有的話）
        playerMovement = GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// 【臨時變更】Fungus 將呼叫這個函式，來觸發一個持續 1 秒的變更。
    /// </summary>
    public void TriggerChange()
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(ChangeAndRevertSprite());
    }

    private IEnumerator ChangeAndRevertSprite()
    {
        // 儲存當前（執行前）的圖片，以便稍後還原
        Sprite originalSprite = spriteRenderer.sprite;

        // 【關鍵】如果存在 Animator，暫時禁用它
        if (animator != null)
        {
            animator.enabled = false;
        }
        // 【關鍵】如果存在 PlayerMovement 腳本，暫時禁用移動
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        // 如果在 Inspector 中設定了臨時圖片，就進行更換
        if (temporarySprite != null)
        {
            spriteRenderer.sprite = temporarySprite;
        }

        // 等待 1 秒
        yield return new WaitForSeconds(1f);

        // 將圖片還原為原始的樣子
        spriteRenderer.sprite = originalSprite;

        // 【關鍵】重新啟用 Animator 和移動，讓動畫和操控恢復正常
        if (animator != null)
        {
            animator.enabled = true;
        }
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }

        // 重設協程追蹤器
        runningCoroutine = null;
    }

    /// <summary>
    /// 【新增：持續變更】Fungus 將呼叫這個函式，來將角色切換為背身狀態並保持。
    /// </summary>
    public void ChangeToPersistentSprite()
    {
        // 停止任何正在運行的臨時變更協程
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // 禁用 Animator 和移動
        if (animator != null)
        {
            animator.enabled = false;
        }
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        // 更換為指定的 Sprite
        if (temporarySprite != null)
        {
            spriteRenderer.sprite = temporarySprite;
        }
    }

    /// <summary>
    /// 【新增：恢復預設】Fungus 將呼叫這個函式，來結束背身狀態，恢復角色的正常動畫和移動。
    /// </summary>
    public void RevertToDefaultState()
    {
        // 停止任何正在運行的臨時變更協程
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        // 重新啟用 Animator 和移動。Animator 會自動將 Sprite 恢復為正確的閒置/走路動畫。
        if (animator != null)
        {
            animator.enabled = true;
        }
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }
}

