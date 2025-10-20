using UnityEngine;
using System.Collections;

/// <summary>
/// 一個靈活的腳本，應掛載在一個空的遊戲物件上。
/// 它可以臨時或持續地改變指定目標物件的 Sprite。
/// 它能夠在運行時動態尋找持久化的玩家，以避免場景切換後的引用丟失問題。
/// </summary>
public class FlexibleSpriteChanger : MonoBehaviour
{
    [Header("目標設定")]
    [Tooltip("要改變其 Sprite 的目標物件。如果留空，將在運行時自動尋找帶有'Player'標籤的物件。")]
    public SpriteRenderer targetSpriteRenderer;

    [Tooltip("要顯示的 Sprite 圖片（臨時或持續狀態共用）。")]
    public Sprite temporarySprite;

    [Tooltip("臨時圖片顯示的持續時間（秒）。")]
    public float duration = 1f;

    private Coroutine runningCoroutine;

    /// <summary>
    /// 【新增】一個穩健的函式，用於在需要時獲取目標。
    /// </summary>
    private bool EnsureTargetIsValid()
    {
        // 如果引用有效，直接返回 true
        if (targetSpriteRenderer != null)
        {
            return true;
        }

        // 如果引用已丟失（例如場景重載後），則嘗試動態尋找玩家
        Debug.Log("目標 Sprite Renderer 為空，正在嘗試動態尋找玩家...");
        if (PlayerPersistence.instance != null)
        {
            targetSpriteRenderer = PlayerPersistence.instance.GetComponent<SpriteRenderer>();
            if (targetSpriteRenderer != null)
            {
                Debug.Log("成功動態找到玩家的 SpriteRenderer！");
                return true;
            }
        }

        // 如果連動態尋找都失敗，才報錯
        Debug.LogError("錯誤：在 FlexibleSpriteChanger 中無法找到目標 Sprite Renderer！", this.gameObject);
        return false;
    }

    /// <summary>
    /// 【臨時變更】Fungus 將呼叫這個函式，來觸發一個持續特定時間的變更。
    /// </summary>
    public void TriggerChange()
    {
        // 【修改】在執行任何操作前，先確保目標有效
        if (!EnsureTargetIsValid()) return;

        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(ChangeAndRevertSprite());
    }

    private IEnumerator ChangeAndRevertSprite()
    {
        Animator animator = targetSpriteRenderer.GetComponent<Animator>();
        PlayerMovement playerMovement = targetSpriteRenderer.GetComponent<PlayerMovement>();
        
        Sprite originalSprite = targetSpriteRenderer.sprite;

        if (animator != null) animator.enabled = false;
        if (playerMovement != null) playerMovement.SetCanMove(false);

        if (temporarySprite != null)
        {
            targetSpriteRenderer.sprite = temporarySprite;
        }

        yield return new WaitForSeconds(duration);

        targetSpriteRenderer.sprite = originalSprite;

        if (animator != null) animator.enabled = true;
        if (playerMovement != null) playerMovement.SetCanMove(true);

        runningCoroutine = null;
    }

    /// <summary>
    /// 【新增：持續變更】Fungus 將呼叫這個函式，來將角色切換為指定圖片的狀態並保持。
    /// </summary>
    public void ChangeToPersistent()
    {
        // 【修改】在執行任何操作前，先確保目標有效
        if (!EnsureTargetIsValid()) return;

        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        Animator animator = targetSpriteRenderer.GetComponent<Animator>();
        PlayerMovement playerMovement = targetSpriteRenderer.GetComponent<PlayerMovement>();

        if (animator != null) animator.enabled = false;
        if (playerMovement != null) playerMovement.SetCanMove(false);

        if (temporarySprite != null)
        {
            targetSpriteRenderer.sprite = temporarySprite;
        }
    }

    /// <summary>
    /// 【新增：恢復預設】Fungus 將呼叫這個函式，來結束持續變更狀態，恢復角色的正常動畫和移動。
    /// </summary>
    public void RevertToDefault()
    {
        // 【修改】在執行任何操作前，先確保目標有效
        if (!EnsureTargetIsValid()) return;

        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }

        Animator animator = targetSpriteRenderer.GetComponent<Animator>();
        PlayerMovement playerMovement = targetSpriteRenderer.GetComponent<PlayerMovement>();

        if (animator != null) animator.enabled = true;
        if (playerMovement != null) playerMovement.SetCanMove(true);
    }
}
