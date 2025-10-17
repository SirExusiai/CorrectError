using UnityEngine;

public class InteractableNote : MonoBehaviour, IInteractable
{
    public DialogueData dialogueToTrigger; // 在 Inspector 中指定要觸發的對話數據
    public string flagToSetOnInteract;//互动旗帜

    public void Interact()
    {
        // 呼叫 DialogueManager 的單例，並開始對話
        Debug.Log("Interacted");
        DialogueManager.instance.StartDialogue(dialogueToTrigger);
        
        if (!string.IsNullOrEmpty(flagToSetOnInteract))
        {
            GameEventManager.instance.SetFlag(flagToSetOnInteract, true);
        }// 如果旗標名稱不為空，就在互動後設定它
    }
}