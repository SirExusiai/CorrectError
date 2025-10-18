using UnityEngine;

public class ConditionalInteraction : MonoBehaviour, IInteractable
{
    [Header("Fungus 設定")]
    public Fungus.Flowchart targetFlowchart;

    [Header("條件設定")]
    public string[] requiredFlags;
    public string successBlockName; // 成功時執行的 Block
    public string failureBlockName; // 失敗時執行的 Block

    public void Interact()
    {
        if (targetFlowchart == null) return;

        if (GameEventManager.instance.AreAllFlagsSet(requiredFlags))
        {
            if(!string.IsNullOrEmpty(successBlockName))
                targetFlowchart.ExecuteBlock(successBlockName);
        }
        else
        {
            if(!string.IsNullOrEmpty(failureBlockName))
                targetFlowchart.ExecuteBlock(failureBlockName);
        }
    }
}