using UnityEngine;
using UnityEngine.EventSystems;

public class WrongObject : MonoBehaviour
{
    [Header("Fungus 設定")]
    [Tooltip("要觸發的 Fungus Flowchart 物件")]
    public Fungus.Flowchart targetFlowchart;

    [Tooltip("要執行的 Block 的名稱")]
    public string targetBlockName;

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 檢查 Flowchart 和 Block 名稱是否已設定
        if (targetFlowchart != null && !string.IsNullOrEmpty(targetBlockName))
        {
            // 執行指定的 Block
            targetFlowchart.ExecuteBlock(targetBlockName);
        }
        else
        {
            Debug.LogWarning("此物件未指定要執行的 Fungus Flowchart 或 Block 名稱。");
        }
    }
}