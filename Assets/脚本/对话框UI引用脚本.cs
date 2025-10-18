using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引用 TextMeshPro 命名空間

// 這個腳本是一個簡單的「容器」，負責持有對話 UI 元件的引用。
// 請將它掛載到您每個場景的  DialoguePanel 物件上。
public class DialogueUI_References : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject dialoguePanel;
    public Image avatarImage;
    public TextMeshProUGUI nameText; // 將類型從 Text 改為 TextMeshProUGUI
    public TextMeshProUGUI dialogueText; // 將類型從 T ext 改為 TextMeshProUGUI
}