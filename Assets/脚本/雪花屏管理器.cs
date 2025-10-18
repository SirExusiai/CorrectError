using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("場景設定")]
    public DialogueData monologue;
    public string nextSceneName;
    // 【新增】指定下一個場景的入口點名稱
    public string entryPointNameInNextScene; 
    
    [Header("閃屏效果設定")]
    public CanvasGroup flashPanelCanvasGroup;
    public float flashDuration = 0.2f;

    void Start()
    {
        DialogueManager.OnDialogueEnd += GoToNextScene;
        StartCoroutine(PlayCutscene());
    }

    private void OnDestroy()
    {
        DialogueManager.OnDialogueEnd -= GoToNextScene;
    }

    private IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(0.5f);

        // 【修正】這裡的寫法是只有一個 yield return
        yield return StartCoroutine(FlashScreen());
    
        // 檢查 DialogueManager 實例是否存在
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(monologue);
        }
        else
        {
            Debug.LogError("場景中找不到 DialogueManager 的實例！");
        }
    }

    private IEnumerator FlashScreen()
    {
        // ... (閃屏的程式碼保持不變)
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            flashPanelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / (flashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashPanelCanvasGroup.alpha = 1;

        elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            flashPanelCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / (flashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashPanelCanvasGroup.alpha = 0;
    }

    private void GoToNextScene()
    {
        // 【修改】在載入場景之前，先將入口點名稱記錄到 GameEventManager
        if (GameEventManager.instance != null)
        {
            GameEventManager.nextEntryPointName = this.entryPointNameInNextScene;
        }
        
        Debug.Log("獨白結束，正在前往: " + nextSceneName + "，入口點: " + entryPointNameInNextScene);
        SceneManager.LoadScene(nextSceneName);
    }
}