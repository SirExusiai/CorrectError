using UnityEngine;

[RequireComponent(typeof(Collider2D))] // 確保物件總是有一個碰撞體
public class FungusTrigger : MonoBehaviour
{
    [Header("Fungus 設定")]
    public Fungus.Flowchart targetFlowchart;
    public string targetBlockName;

    [Header("觸發設定")]
    public bool triggerOnce = true; // 是否只觸發一次

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查是否已被觸發過 (如果設定為只觸發一次)
        if (triggerOnce && hasBeenTriggered)
        {
            return;
        }

        // 檢查進入的是否是玩家
        if (other.CompareTag("Player"))
        {
            if (targetFlowchart != null && !string.IsNullOrEmpty(targetBlockName))
            {
                hasBeenTriggered = true;
                targetFlowchart.ExecuteBlock(targetBlockName);
            }
        }
    }
}