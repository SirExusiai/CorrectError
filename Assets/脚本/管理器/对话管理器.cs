using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance; // 單例模式，方便其他腳本呼叫

    // UI 元素的引用
    public GameObject dialoguePanel;
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public static event Action OnDialogueEnd; // 對話結束時觸發的事件

    // 儲存對話數據的佇列 (Queue) 
    private Queue<DialogueLine> dialogueQueue;

    // 玩家的引用，用來鎖定/解鎖移動
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    private bool isDialogueActive = false;

    void Awake()
    {
        // 單例模式設定
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        dialogueQueue = new Queue<DialogueLine>();
        // 找到玩家物件並獲取其腳本
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }
    }

    void Update()
    {
        // 在對話進行中，監聽空格或滑鼠點擊
        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            DisplayNextSentence();
        }
    }

    // 在 DialogueManager.cs 中加入這個函式

    private void OnDestroy()
    {
        // 當這個物件被銷毀時，檢查 instance 是否還指向自己
        if (instance == this)
        {
            // 如果是，就將 instance 清空，這樣下一個場景的 DialogueManager 才能正常初始化
            instance = null;
        }
    }
    public void StartDialogue(DialogueData dialogue)
    {
        isDialogueActive = true;

        // 鎖定玩家操作
        if (playerMovement != null) playerMovement.SetCanMove(false);
        if (playerInteraction != null) playerInteraction.enabled = false;

        // 顯示對話框
        dialoguePanel.SetActive(true);

        // 清空之前的對話佇列
        dialogueQueue.Clear();

        // 將新的對話內容加入佇列
        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueQueue.Enqueue(line);
        }

        // 顯示第一句話
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // 如果佇列中沒有對話了，就結束對話
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // 從佇列中取出下一句話
        DialogueLine currentLine = dialogueQueue.Dequeue();

        // 更新 UI
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.sentence;
        if (currentLine.characterAvatar != null)
        {
            avatarImage.sprite = currentLine.characterAvatar;
            avatarImage.enabled = true;
        }
        else
        {
            avatarImage.enabled = false; // 如果沒有頭像，就隱藏 Image
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        // 隱藏對話框
        dialoguePanel.SetActive(false);

        // 解鎖玩家操作
        if (playerMovement != null) playerMovement.SetCanMove(true);
        if (playerInteraction != null) playerInteraction.enabled = true;
        OnDialogueEnd?.Invoke();
    }
}