using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 如果您已解決 TMPro 的問題，請保留這行。如果沒有，請換回 using UnityEngine.UI; 並將下方的 TextMeshProUGUI 改為 Text。
using TMPro;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // --- 【重要】---
    // 這些是公開的，需要在每個場景的 Inspector 視窗中手動拖曳指定
    [Header("在此場景中手動設定 UI 元件")]
    public GameObject dialoguePanel;
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    // ----------------

    public static event Action OnDialogueEnd;

    private Queue<DialogueLine> dialogueQueue;
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    private bool isDialogueActive = false;
    private bool inputBlocked = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("場景中發現了多個 DialogueManager，已將此重複項刪除。");
            Destroy(gameObject);
        }
        dialogueQueue = new Queue<DialogueLine>();
    }

    void Start()
    {
        // 在 Start 中尋找玩家 (如果該場景有玩家的話)
        FindPlayerInScene();
        
        // 確保UI初始為隱藏狀態
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void Update()
    {
        if (isDialogueActive && !inputBlocked && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            DisplayNextSentence();
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    
    // 用於尋找玩家的輔助函式
    private void FindPlayerInScene()
    {
        SceneInfo sceneInfo = FindObjectOfType<SceneInfo>();
        // 如果場景被標記為無玩家，則不進行尋找
        if (sceneInfo != null && !sceneInfo.hasPlayer) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        StartCoroutine(StartDialogueCoroutine(dialogue));
    }

    private IEnumerator StartDialogueCoroutine(DialogueData dialogue)
    {
        // 【修改】不再呼叫 Find... 函式，而是直接檢查公開變數是否已在 Inspector 中設定
        if (dialoguePanel == null || nameText == null || dialogueText == null)
        {
            Debug.LogError("錯誤：DialogueManager 的 UI 元件未在 Inspector 中設定！無法開始對話。");
            yield break; // 終止協程
        }

        isDialogueActive = true;
        inputBlocked = true;

        if (playerMovement != null) playerMovement.SetCanMove(false);
        if (playerInteraction != null) playerInteraction.enabled = false;

        dialoguePanel.SetActive(true);
        dialogueQueue.Clear();

        foreach (DialogueLine line in dialogue.lines)
        {
            dialogueQueue.Enqueue(line);
        }
        
        DisplayNextSentence();

        yield return null; 
        
        inputBlocked = false;
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = dialogueQueue.Dequeue();
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.sentence;
        if (currentLine.characterAvatar != null)
        {
            avatarImage.sprite = currentLine.characterAvatar;
            avatarImage.enabled = true;
        }
        else if (avatarImage != null)
        {
            avatarImage.enabled = false;
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        inputBlocked = false; // 確保重設

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        if (playerMovement != null) playerMovement.SetCanMove(true);
        if (playerInteraction != null) playerInteraction.enabled = true;
        OnDialogueEnd?.Invoke();
    }
}
