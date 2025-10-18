using UnityEngine;

public class TestRunner : MonoBehaviour
{
    // 讓我們可以在 Inspector 中指定一個對話數據來測試
    public DialogueData testDialogue;

    void Start()
    {
        Debug.Log("--- 開始測試 ---");

        if (testDialogue == null)
        {
            Debug.LogError("錯誤：測試用的對話數據是空的 (null)！"); 
            return;
        }

        // 檢查對話數據中的 lines 陣列是否為 null
        if (testDialogue.lines == null)
        {
            Debug.LogError("錯誤：對話數據 '" + testDialogue.name + "' 的 Lines 陣列是空的 (null)！腳本可能已失聯。");
        }
        else
        {
            Debug.Log("<color=green>成功：</color> 對話數據 '" + testDialogue.name + "' 載入正常，包含 " + testDialogue.lines.Length + " 句話。");
        }

        Debug.Log("--- 測試結束 ---");
    }
}