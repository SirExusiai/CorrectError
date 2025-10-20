using UnityEngine;

[RequireComponent(typeof(Collider2D))] // 確保物件總是有一個碰撞體
public class FungusTrigger : MonoBehaviour
{
    [Header("Fungus 設定")]
    public Fungus.Flowchart targetFlowchart;
    public string targetBlockName;

    [Header("觸發設定")]
    [Tooltip("如果勾選，此觸發器在整個遊戲生命週期中只會觸發一次。狀態將被全局儲存。")]
    public bool triggerOnceGlobally = true; 
    [Tooltip("為此觸發器設定一個全局唯一的旗標名稱。")]
    public string triggerFlagName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查進入的是否是玩家
        if (other.CompareTag("Player"))
        {
            // 如果設定為全局觸發一次，先檢查全局旗標
            if (triggerOnceGlobally)
            {
                if (string.IsNullOrEmpty(triggerFlagName))
                {
                    Debug.LogError("FungusTrigger 已設定為全局觸發一次，但未提供唯一的 triggerFlagName！", gameObject);
                    return;
                }

                // 如果旗標已設定，則不執行任何操作
                if (GameEventManager.instance.GetFlag(triggerFlagName))
                {
                    return;
                }
            }

            // 執行 Fungus Block
            if (targetFlowchart != null && !string.IsNullOrEmpty(targetBlockName))
            {
                // 如果設定為全局觸發一次，則在觸發後立刻設定旗標
                if (triggerOnceGlobally)
                {
                    GameEventManager.instance.SetFlag(triggerFlagName, true);
                }

                targetFlowchart.ExecuteBlock(targetBlockName);
            }
        }
    }
}