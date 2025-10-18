using UnityEngine;

public class InteractableNote : MonoBehaviour
{
    [Header("Fungus 設定")]
    public Fungus.Flowchart targetFlowchart;
    public string targetBlockName;
    
    [Header("旗標設定")]
    public string flagToSetOnInteract;

    // 我們將互動方式改回 IInteractable，以便玩家靠近按 E
    void Start()
    {
        // 為了讓這個腳本能被 PlayerInteraction 偵測到，
        // 我們需要一個碰撞體和一個實現 IInteractable 的元件。
        // 我們動態地添加一個簡單的轉發器。
        gameObject.AddComponent<FungusInteractableForwarder>().parentNote = this;
    }

    public void TriggerInteraction()
    {
        if (targetFlowchart != null && !string.IsNullOrEmpty(targetBlockName))
        {
            targetFlowchart.ExecuteBlock(targetBlockName);
        }

        if (!string.IsNullOrEmpty(flagToSetOnInteract))
        {
            GameEventManager.instance.SetFlag(flagToSetOnInteract, true);
        }
    }
}

// 這是一個輔助類別，用來接收 IInteractable 的呼叫
public class FungusInteractableForwarder : MonoBehaviour, IInteractable
{
    public InteractableNote parentNote;
    public void Interact()
    {
        if (parentNote != null)
        {
            parentNote.TriggerInteraction();
        }
    }
}