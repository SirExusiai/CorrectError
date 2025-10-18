using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [Header("傳送設定")]
    public string sceneToLoad;
    public string entryPointName;

    [Header("條件設定")]
    public bool requiresCondition = false;
    public string[] requiredFlags;

    [Header("Fungus 設定 (條件不滿足時)")]
    public Fungus.Flowchart lockedFlowchart;
    public string lockedBlockName;

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryTeleport();
        }
    }

    private void TryTeleport()
    {
        if (requiresCondition)
        {
            if (GameEventManager.instance.AreAllFlagsSet(requiredFlags))
            {
                LoadTargetScene();
            }
            else
            {
                // 條件不滿足，執行 Fungus Block
                if (lockedFlowchart != null && !string.IsNullOrEmpty(lockedBlockName))
                {
                    lockedFlowchart.ExecuteBlock(lockedBlockName);
                }
            }
        }
        else
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        GameEventManager.nextEntryPointName = this.entryPointName;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}