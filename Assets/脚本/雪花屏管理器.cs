using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneController : MonoBehaviour
{
    [Header("Fungus 設定")]
    public Fungus.Flowchart cutsceneFlowchart;
    public string startBlockName = "StartCutscene";

    [Header("場景跳轉設定")]
    public string nextSceneName;
    public string entryPointInNextScene;

    void Start()
    {
        if (cutsceneFlowchart != null)
        {
            // 直接執行 Fungus 的起始 Block
            cutsceneFlowchart.ExecuteBlock(startBlockName);
        }
    }

    // 這個公開函式將由 Fungus 的 Call Method 指令來呼叫
    public void GoToNextScene()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // 短暫延遲
        if (GameEventManager.instance != null)
        {
            GameEventManager.nextEntryPointName = this.entryPointInNextScene;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}