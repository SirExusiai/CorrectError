using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    private GameObject dialoguePanel;
    private Image avatarImage;
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    private PlayerMovement playerMovement;
    private PlayerInteraction playerInteraction;
    
    private Queue<DialogueLine> dialogueQueue;
    private bool isDialogueActive = false;
    private bool inputBlocked = false; 
    public static event Action OnDialogueEnd;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueQueue = new Queue<DialogueLine>();
    }

    private void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    private void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dialoguePanel = null;
        playerMovement = null;
        playerInteraction = null;
    }

    private bool FindAndCacheUIReferences()
    {
        if (dialoguePanel != null) return true;
        DialogueUI_References uiReferences = FindObjectOfType<DialogueUI_References>(true); // true 代表「也尋找被禁用的物件」
        if (uiReferences != null)
        {
            dialoguePanel = uiReferences.dialoguePanel;
            avatarImage = uiReferences.avatarImage;
            nameText = uiReferences.nameText;
            dialogueText = uiReferences.dialogueText;
            return true;
        }
        return false;
    }

    private void FindPlayer()
    {
        if (playerMovement != null) return;
        SceneInfo sceneInfo = FindObjectOfType<SceneInfo>();
        if (sceneInfo != null && !sceneInfo.hasPlayer) return;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            playerInteraction = playerObj.GetComponent<PlayerInteraction>();
        }
    }

    public void StartDialogue(DialogueData dialogue) { StartCoroutine(StartDialogueCoroutine(dialogue)); }

    private IEnumerator StartDialogueCoroutine(DialogueData dialogue)
    {
        if (!FindAndCacheUIReferences()) 
        {
            Debug.LogError("無法開始對話，因為在當前場景中未找到 DialogueUI_References！請檢查該場景。");
            yield break;
        }
        FindPlayer();

        isDialogueActive = true;
        inputBlocked = true; 

        if (playerMovement != null) playerMovement.SetCanMove(false);
        if (playerInteraction != null) playerInteraction.enabled = false;

        dialoguePanel.SetActive(true);
        dialogueQueue.Clear();

        foreach (DialogueLine line in dialogue.lines) { dialogueQueue.Enqueue(line); }
        
        DisplayNextSentence();
        yield return null; 
        inputBlocked = false;
    }

    void Update()
    {
        if (isDialogueActive && !inputBlocked && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            DisplayNextSentence();
        }
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0) { EndDialogue(); return; }
        DialogueLine currentLine = dialogueQueue.Dequeue();
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.sentence;
        if (currentLine.characterAvatar != null)
        {
            avatarImage.sprite = currentLine.characterAvatar;
            avatarImage.enabled = true;
        }
        else { avatarImage.enabled = false; }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        inputBlocked = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (playerMovement != null) playerMovement.SetCanMove(true);
        if (playerInteraction != null) playerInteraction.enabled = true;
        OnDialogueEnd?.Invoke();
    }
}