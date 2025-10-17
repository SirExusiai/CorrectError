using UnityEngine;

// System.Serializable 讓這個類別的實例可以在 Inspector 中顯示
[System.Serializable]
public class DialogueLine
{
    public string characterName;  // 說話的角色名字
    public Sprite characterAvatar; // 角色的頭像
    [TextArea(3, 10)]
    public string sentence;       // 對話的具體內容
}

// 創建一個新的 Asset 選單項目，方便我們直接在 Project 視窗中創建對話數據
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines; // 一段完整的對話，由多句 DialogueLine 組成
}