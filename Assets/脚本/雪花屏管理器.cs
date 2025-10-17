using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("場景設定")]
    public DialogueData monologue;         // 要自動播放的獨白
    public string nextSceneName;          // 播放完畢後要跳轉到的場景名稱
    
    [Header("閃屏效果設定")]
    public CanvasGroup flashPanelCanvasGroup; // 閃屏 Panel 的 CanvasGroup
    public float flashDuration = 0.2f;      // 閃爍持續時間

    void Start()
    {
        // 訂閱「對話結束」事件，當事件發生時，呼叫 GoToNextScene 函式
        DialogueManager.OnDialogueEnd += GoToNextScene;
        
        // 開始播放過場
        StartCoroutine(PlayCutscene());
    }

    private void OnDestroy()
    {
        // 物件銷毀時，務必取消訂閱，防止記憶體洩漏
        DialogueManager.OnDialogueEnd -= GoToNextScene;
    }

    private IEnumerator PlayCutscene()
    {
        // 等待一小段時間，確保場景載入完成
        yield return new WaitForSeconds(0.5f);

        // 執行閃屏
        yield return StartCoroutine(FlashScreen());
        
        // 開始播放獨白
        DialogueManager.instance.StartDialogue(monologue);
    }

    private IEnumerator FlashScreen()
    {
        // 淡入 (從透明到不透明)
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration / 2)
        {
            flashPanelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / (flashDuration / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashPanelCanvasGroup.alpha = 1;

        // 淡出 (從不透明到透明)
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
        // 當收到對話結束的通知時，載入下一個場景
        Debug.Log("獨白結束，正在前往: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}