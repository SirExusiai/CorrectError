using UnityEngine;
using UnityEngine.EventSystems; // 【新增】引用 UI 事件系統

public class WrongObject : MonoBehaviour
{
    public DialogueData wrongDialogue;

    // 當滑鼠點擊到這個物件的碰撞體時，這個函式會被自動呼叫
    private void OnMouseDown()
    {
        Debug.Log("嘗試點擊 " + gameObject.name);
        // 【新增】檢查滑鼠指標是否在任何 UI 元件之上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // 如果是，就立刻停止執行，不觸發後續的互動
            return;
        }

        // --- 只有當滑鼠沒有點擊到 UI 時，才會執行到這裡 ---
        if (wrongDialogue != null && DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(wrongDialogue);
        }
        else
        {
            Debug.Log("這個物件沒有什麼特別的。");
        }
    }
}