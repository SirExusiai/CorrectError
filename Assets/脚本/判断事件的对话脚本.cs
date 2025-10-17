using UnityEngine;

public class ConditionalInteraction : MonoBehaviour, IInteractable
{
    [Header("條件設定")]
    public string[] requiredFlags; // 需要滿足的旗標列表

    [Header("滿足條件時的互動")]
    public DialogueData successDialogue; // 滿足條件時觸發的對話
    public string flagToSetOnSuccess;    // 滿足條件互動後，可設定的新旗標

    [Header("不滿足條件時的互動")]
    public DialogueData failureDialogue; // 不滿足條件時觸發的對話

    public void Interact()
    {
        // 檢查是否所有必需的旗標都已被設定
        if (GameEventManager.instance.AreAllFlagsSet(requiredFlags))
        {
            // 條件滿足
            TriggerDialogue(successDialogue);
            if (!string.IsNullOrEmpty(flagToSetOnSuccess))
            {
                GameEventManager.instance.SetFlag(flagToSetOnSuccess, true);
            }
        }
        else
        {
            // 條件不滿足
            TriggerDialogue(failureDialogue);
        }
    }

    private void TriggerDialogue(DialogueData dialogue)
    {
        if (dialogue != null)
        {
            DialogueManager.instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning("沒有指定對話數據！");
        }
    }
}